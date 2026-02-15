using DoubleClickFix.Properties;
using Microsoft.Win32;
using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix;

/// <summary>
/// The method HookCallback(..) implements low-level mouse interception to both suppress erroneous double-clicks and
/// enable robust drag-and-drop even on “bouncy” mice. The algorithm proceeds as follows:
/// 
/// 1. Marshal the MSLLHOOKSTRUCT for every event.
/// 2. On WM_MOUSEMOVE:
///    • Update lastMoveTime for each genuinely pressed button.
///    • Compute distance from initialDownPosition and time since initialDownTime.
///    • If the cursor has moved more than MovementThresholdPixels _and_ at least
///      DragReleaseTimeMilliseconds have elapsed since the real down, enter drag-lock:
///      further spurious Up/Down for that button will be suppressed until the drag ends.
/// 3. For button messages (Down/Up):
///    • Identify which MouseButtons value is active and load its threshold setting.
///    • If that button is in drag-lock:
///        – Suppress any Down events.
///        – Suppress Up events until movement has paused for at least DragReleaseTimeMilliseconds,
///          then release, exit drag-lock, and forward the genuine Up.
///    • Otherwise, apply the normal double-click suppression logic:
///        – On Down: compare time since previousUpTime to the per-button threshold and MinDelay.
///          Ignore if too soon; log otherwise.
///        – On Up: record previousUpTime for future comparisons.
/// 4. Track the set of currentlyDownButtons to know which buttons are genuinely pressed.
/// 5. Forward any non-suppressed events to CallNextHook to let the system handle them.
///
/// This ensures that during a true drag gesture—holding down, moving, and releasing—the user can
/// drag and drop even if the physical switch chatters, while still suppressing unwanted rapid clicks.
/// </summary>
internal class MouseHook : IDisposable
{
    // to make sure the hook returns as fast as possible, we cache some texts here.
    private static readonly FrozenDictionary<MouseButtons, string> buttonTextLookup = new Dictionary<MouseButtons, string> {
            { MouseButtons.Left, Resources.Left },
            { MouseButtons.Right, Resources.Right },
            { MouseButtons.Middle, Resources.Middle },
            { MouseButtons.XButton1, Resources.X1 },
            { MouseButtons.XButton2, Resources.X2 },
        }.ToFrozenDictionary();

    private static readonly string ignoredDoubleClickText = Resources.IgnoredDoubleClick;

    private readonly ISettings settings;
    private readonly ILogger logger;
    private readonly INativeMethods nativeMethods;

    // make sure we keep a reference so it's not garbage collected
    private LowLevelMouseProc? mouseProc;
    private nint hookHandle = nint.Zero;
    private const nint InvalidDevice = -1;
    private nint currentDevice = InvalidDevice;

    private readonly Dictionary<MouseButtons, bool> isLogicallyDown = [];
    private readonly Dictionary<MouseButtons, uint> logicalDownTime = [];

    private readonly Dictionary<MouseButtons, uint> previousUpTime = new() { {MouseButtons.Left , 0 }, {MouseButtons.Right , 0}, {MouseButtons.Middle , 0}, {MouseButtons.XButton1 , 0}, {MouseButtons.XButton2 , 0} };
    private FrozenSet<nint> observedMessages = [];
    private uint ignoredClicks = 0;
    private readonly Dictionary<nint, uint> previousWheelTime = [];
    private readonly Dictionary<nint, int> lastWheelDirection = [];

    // for drag‐lock handling
    private readonly HashSet<MouseButtons> currentlyDownButtons = [];
    private readonly Dictionary<MouseButtons, POINT> initialDownPosition = [];
    private readonly Dictionary<MouseButtons, long> lastMoveTime = [];
    private readonly Dictionary<MouseButtons, bool> isDragLocked = [];
    private readonly Dictionary<MouseButtons, long> initialDownTime = [];

    public MouseHook(ISettings settings, ILogger logger, INativeMethods nativeMethods)
    {
        this.settings = settings;
        SettingsChanged();
        settings.RegisterSettingsChangedListener(SettingsChanged);
        this.logger = logger;
        this.nativeMethods = nativeMethods;
    }

    private void SettingsChanged()
    {
        HashSet<nint> messages = [];
        if (settings.LeftThreshold >= 0) {
            messages.Add(WM_LBUTTONDOWN);
            messages.Add(WM_LBUTTONUP);
        }
        if (settings.RightThreshold >= 0)
        {
            messages.Add(WM_RBUTTONDOWN);
            messages.Add(WM_RBUTTONUP);
        }
        if (settings.MiddleThreshold >= 0)
        {
            messages.Add(WM_MBUTTONDOWN);
            messages.Add(WM_MBUTTONUP);
        }
        if (settings.X1Threshold >= 0 || settings.X2Threshold >= 0)
        {
            messages.Add(WM_XBUTTONDOWN);
            messages.Add(WM_XBUTTONUP);
        }
        if (settings.WheelThreshold >= 0)
        {
            messages.Add(WM_MOUSEWHEEL);
            messages.Add(WM_MOUSEHWHEEL);
        }
        if (!settings.IsDragCorrectionEnabled && currentlyDownButtons.Count > 0)
        {
            ResetDragLockState();
        }
        observedMessages = messages.ToFrozenSet();
    }

    internal void ResetDragLockState()
    {
        currentlyDownButtons.Clear();
        initialDownPosition.Clear();
        lastMoveTime.Clear();
        isDragLocked.Clear();
        initialDownTime.Clear();
        isLogicallyDown.Clear();
        logicalDownTime.Clear();
    }

    public bool Install()
    {
        if (settings.UseHook && hookHandle == nint.Zero)
        {
            try
            {
                mouseProc = HookCallback;
                hookHandle = nativeMethods.SetHook(mouseProc);
            }
            catch (Win32Exception ex)
            {
                logger.Log($"{Resources.HookNotInstalled}: {ex.Message}");
                return false;
            }
        }
        return hookHandle != nint.Zero;
    }
    public void Uninstall()
    {
        if (settings.UseHook && hookHandle != nint.Zero)
        {
            try
            {
                nativeMethods.UnhookWindowsHook(hookHandle);
            }
            catch (Win32Exception ex)
            {
                logger.Log($"Failed to uninstall hook: {ex.Message}");
            }
            finally
            {
                hookHandle = nint.Zero;
                mouseProc = null;
            }
        }
    }

    

    public void RegisterForRawInput(nint hwnd)
    {
        try
        {
            nativeMethods.RegisterForRawInput(hwnd);
        }
        catch (Win32Exception ex)
        {
            logger.Log($"Failed to register raw input device. {ex.Message}");  // TODO translate.
        }
    }

    public void ProcessRawInput(nint hRawInput)
    {
        if (nativeMethods.TryProcessRawInput(hRawInput, out var device))
        {
            if (currentDevice != device)
            {
                logger.Log($"{Resources.SwitchedDevice} {device}", true);
                currentDevice = device;
            }
        }
    }

        internal nint HookCallback(int nCode, nint wParam, nint lParam)
    {
        try
        {
            if (wParam == WM_MOUSEMOVE)
            {
                HandleMouseMove(lParam);
            }
            else if (wParam == WM_MOUSEWHEEL || wParam == WM_MOUSEHWHEEL)
            {
                return HandleWheel(nCode, wParam, lParam);
            }
            else
            {
                return HandleMouseButton(nCode, wParam, lParam);
            }
            return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
        }
        catch (Exception ex)
        {
            logger.Log($"Error in hook callback: {ex}");
            return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
        }
    }

    private nint HandleWheel(int nCode, nint wParam, nint lParam)
    {
        var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam)!;
        const nint IgnoreMouseEvent = 1;

        if (!ProcessMouseEvent(nCode, wParam))
        {
            return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
        }

        short wheelDelta = (short)(hookStruct.mouseData >> 16);
        int currentDirection = Math.Sign(wheelDelta);

        if (!previousWheelTime.TryGetValue(wParam, out var prevTime))
        {
            prevTime = 0;
        }

        if (!lastWheelDirection.TryGetValue(wParam, out var lastDirection))
        {
            lastDirection = 0;
        }

        if (prevTime != 0 && currentDirection != lastDirection)
        {
            uint diff = unchecked(hookStruct.time - prevTime);
            long delta = diff;

            if (delta < settings.WheelThreshold)
            {
                ignoredClicks++;
                logger.Log($"{Resources.IgnoredMouseWheel} ({GetDirectionArrow(wParam, currentDirection)}): {delta} ms (#{ignoredClicks})");
                previousWheelTime[wParam] = 0; // Reset to avoid false positives
                lastWheelDirection[wParam] = 0;
                return IgnoreMouseEvent;
            }
        }

        if (logger.IsAppVisible && prevTime != 0)
        {
            uint diff = unchecked(hookStruct.time - prevTime);
            long delta = diff;
            logger.Log($"{Resources.MouseWheelDelta} ({GetDirectionArrow(wParam, currentDirection)}): {delta} ms");
        }

        previousWheelTime[wParam] = hookStruct.time;
        lastWheelDirection[wParam] = currentDirection;

        return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
    }

    private static string GetDirectionArrow(nint wParam, int currentDirection)
    {
        return wParam == WM_MOUSEWHEEL ? (currentDirection > 0 ? "↑" : "↓") : (currentDirection > 0 ? "→" : "←");
    }

    private void HandleMouseMove(nint lParam)
    {
        if (!settings.IsDragCorrectionEnabled || currentlyDownButtons.Count == 0)
        {
            return;
        }

        var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam)!;
        const int MovementThresholdPixels = 5;

        foreach (var button in currentlyDownButtons.ToList())
        {
            lastMoveTime[button] = hookStruct.time;

            if (!isDragLocked.GetValueOrDefault(button, false))
            {
                var start = initialDownPosition[button];
                int dx = hookStruct.pt.x - start.x;
                int dy = hookStruct.pt.y - start.y;
                int distSq = dx * dx + dy * dy;

                long elapsedSinceDown = hookStruct.time - initialDownTime[button];

                if (distSq >= MovementThresholdPixels * MovementThresholdPixels &&
                    elapsedSinceDown >= settings.DragStartTimeMilliseconds)
                {
                    isDragLocked[button] = true;
                    logger.Log(string.Format(Resources.EnterDragLock, buttonTextLookup[button], elapsedSinceDown), true);
                }
            }
        }
    }

    private nint HandleMouseButton(int nCode, nint wParam, nint lParam)
    {
        var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam)!;
        const nint IgnoreMouseEvent = 1;

        if (!ProcessMouseEvent(nCode, wParam))
        {
            return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
        }

        if (!TryGetMouseButton(wParam, hookStruct.mouseData, out var activeButton, out var buttonDown, out var buttonUp))
        {
            return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
        }

        int threshold = GetThreshold(activeButton);

        // If we're in drag‐lock, suppress spurious downs/ups
        if (settings.IsDragCorrectionEnabled && isDragLocked.GetValueOrDefault(activeButton, false))
        {
            if (buttonDown)
            {
                // drop any extra presses
                return IgnoreMouseEvent;
            }
            else if (buttonUp)
            {
                // only allow the real release once movement has paused long enough
                long elapsedSinceMove = hookStruct.time - lastMoveTime[activeButton];
                if (elapsedSinceMove >= settings.DragStopTimeMilliseconds)
                {
                    // exit drag‐lock, forward genuine release
                    isDragLocked[activeButton] = false;
                    currentlyDownButtons.Remove(activeButton);
                    isLogicallyDown[activeButton] = false;
                    previousUpTime[activeButton] = hookStruct.time;
                    logger.Log(string.Format(Resources.ExitDragLock, buttonTextLookup[activeButton]), true);

                    return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
                }
                // still jittering: suppress
                return IgnoreMouseEvent;
            }
        }

        if (buttonDown)
        {
            // If button is already logically down, suppress duplicate DOWN (bounce)
            if (isLogicallyDown.GetValueOrDefault(activeButton, false))
            {
                // logger.Log($"{ignoredDoubleClickText} ({buttonTextLookup[activeButton]}): bounce (#{ignoredClicks})");
                return IgnoreMouseEvent;
            }

            // MSLLHOOKSTRUCT.time is a 32‑bit millisecond timer that wraps roughly every 49 days.
            // Subtracting two uint values and storing the result in a long can produce a negative result after wrap‑around.
            // Compute the difference as an unsigned subtraction first, then cast to long
            uint diff = unchecked(hookStruct.time - previousUpTime[activeButton]);
            long delta = diff;

            bool belowMin = settings.MinDelay >= 0 && delta <= settings.MinDelay;
            bool ignore = delta < threshold && !belowMin;

            if (ignore)
            {
                ignoredClicks++;
                logger.Log($"{ignoredDoubleClickText} ({buttonTextLookup[activeButton]}): {delta} ms (#{ignoredClicks})");
                previousUpTime[activeButton] = 0;
                return IgnoreMouseEvent;
            }

            // Forward this DOWN - update logical state
            isLogicallyDown[activeButton] = true;
            logicalDownTime[activeButton] = hookStruct.time;

            // Drag tracking
            currentlyDownButtons.Add(activeButton);
            initialDownPosition[activeButton] = hookStruct.pt;
            initialDownTime[activeButton] = hookStruct.time;
            lastMoveTime[activeButton] = hookStruct.time;

            if (delta < settings.WindowsDoubleClickTimeMilliseconds)
            {
                logger.Log($"{delta} ms ({buttonTextLookup[activeButton]})", true);
            }
        }
        else if (buttonUp)
        {
            // If button is not logically down, suppress duplicate UP
            if (!isLogicallyDown.GetValueOrDefault(activeButton, false))
            {
                return IgnoreMouseEvent;
            }

            // Check if UP came too quickly after DOWN (bounce during press)
            uint downTime = logicalDownTime.GetValueOrDefault(activeButton, 0u);
            uint diff = unchecked(hookStruct.time - downTime);
            long delta = diff;

            if (delta < threshold)
            {
                ignoredClicks++;
                logger.Log($"{ignoredDoubleClickText} ({buttonTextLookup[activeButton]}): {delta} ms (#{ignoredClicks})");
                return IgnoreMouseEvent;
            }

            // Legitimate release - forward and update state
            isLogicallyDown[activeButton] = false;
            currentlyDownButtons.Remove(activeButton);
            previousUpTime[activeButton] = hookStruct.time;
        }

        // forward everything else
        return nativeMethods.CallNextHook(hookHandle, nCode, wParam, lParam);
    }

    private bool TryGetMouseButton(nint wParam, uint mouseData, out MouseButtons activeButton, out bool buttonDown, out bool buttonUp)
    {
        (activeButton, buttonDown, buttonUp) = wParam switch
        {
            WM_LBUTTONDOWN => (MouseButtons.Left, true, false),
            WM_LBUTTONUP => (MouseButtons.Left, false, true),
            WM_RBUTTONDOWN => (MouseButtons.Right, true, false),
            WM_RBUTTONUP => (MouseButtons.Right, false, true),
            WM_MBUTTONDOWN => (MouseButtons.Middle, true, false),
            WM_MBUTTONUP => (MouseButtons.Middle, false, true),
            WM_XBUTTONDOWN => (GetXButton(mouseData), true, false),
            WM_XBUTTONUP => (GetXButton(mouseData), false, true),
            _ => (MouseButtons.None, false, false)
        };

        return activeButton != MouseButtons.None;
    }

    private int GetThreshold(MouseButtons button)
    {
        return button switch
        {
            MouseButtons.Left => settings.LeftThreshold,
            MouseButtons.Right => settings.RightThreshold,
            MouseButtons.Middle => settings.MiddleThreshold,
            MouseButtons.XButton1 => settings.X1Threshold,
            MouseButtons.XButton2 => settings.X2Threshold,
            _ => -1,
        };
    }

    private bool ProcessMouseEvent(int nCode, nint wParam)
    {
        // TODO fix this workaround when running in x64 as well.
        // Compare against the 32‑bit value of currentDevice; on 64‑bit OSes, the upper bits
        // of currentDevice are ignored for this comparison.
        int currentDeviceId = unchecked((int)currentDevice);

        return nCode >= 0
            && settings.IgnoredDevice != currentDeviceId
            && observedMessages.Contains(wParam);
    }

    private MouseButtons GetXButton(uint mouseData)
    {
        const int XBUTTON1 = 0x0001;
        const int XBUTTON2 = 0x0002;

        ushort hiWord = unchecked((ushort)((ulong)mouseData >> 16));
        if (settings.X1Threshold >= 0 && (hiWord & XBUTTON1) != 0)
        {
            return MouseButtons.XButton1;
        }
        else if (settings.X2Threshold >= 0 && (hiWord & XBUTTON2) != 0)
        {
            return MouseButtons.XButton2;
        }
        return MouseButtons.None;
    }

    public void Dispose()
    {
        Uninstall();
    }
}