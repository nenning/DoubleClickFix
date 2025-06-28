using DoubleClickFix.Properties;
using Microsoft.Win32;
using System.Collections.Frozen;
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
    private IntPtr hookHandle = IntPtr.Zero;
    private IntPtr currentDevice = -1;

    private readonly Dictionary<MouseButtons, uint> previousUpTime = new() { {MouseButtons.Left , 0 }, {MouseButtons.Right , 0}, {MouseButtons.Middle , 0}, {MouseButtons.XButton1 , 0}, {MouseButtons.XButton2 , 0} };
    private FrozenSet<IntPtr> observedMessages = [];
    private uint ignoredClicks = 0;

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
        SystemEvents.PowerModeChanged += OnPowerModeChanged;
    }

    private void SettingsChanged()
    {
        HashSet<IntPtr> messages = [];
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
        observedMessages = messages.ToFrozenSet();
    }

    public bool Install()
    {
        if (settings.UseHook && hookHandle == IntPtr.Zero)
        {
            mouseProc = this.HookCallback;
            hookHandle = SetHook(mouseProc);
        }
        return hookHandle != IntPtr.Zero;
    }
    public void Uninstall()
    {
        if (settings.UseHook && hookHandle != IntPtr.Zero)
        {
            UnhookWindowsHookEx(hookHandle);
            hookHandle = IntPtr.Zero;
            mouseProc = null;
        }
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        // not strictly necessary, but in case of a mouse hook timeout, at least we get the hook back on resume. 
        switch (e.Mode)
        {
            case PowerModes.Suspend:
                Uninstall();
                break;
            case PowerModes.Resume:
                if (!Install())
                {
                    logger.Log("Failed to reinstall mouse hook after Windows resume."); // TODO translate.
                }
                break;
            default:
                break;
        }
    }

    public void RegisterForRawInput(IntPtr hwnd)
    {
        RAWINPUTDEVICE[] device = [
            new() {
                UsagePage = HID_USAGE_PAGE_GENERIC,
                Usage = HID_USAGE_GENERIC_MOUSE,
                Flags = RIDEV_INPUTSINK,
                Target = hwnd
            }
        ];

        if (!RegisterRawInputDevices(device, (uint)device.Length, (uint)Marshal.SizeOf(device[0])))
        {
            logger.Log("Failed to register raw input device."); // TODO translate.
        }
    }

    public void ProcessRawInput(IntPtr hRawInput)
    {
        uint dwSize = 0;
        _ = GetRawInputData(hRawInput, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>());
        IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
        try
        {
            if (GetRawInputData(hRawInput, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>()) != dwSize)
                return;

            RAWINPUT raw = Marshal.PtrToStructure<RAWINPUT>(buffer);

            if (raw.Header.Type == RIM_TYPEMOUSE)
            {
                var device = raw.Header.Device;
                if (currentDevice != device)
                {
                    logger.Log($"{Resources.SwitchedDevice} {device}", true);
                    currentDevice = device;
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    internal IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        // Always marshal the hook structure first
        MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam)!;
        const int WM_MOUSEMOVE = 0x0200;
        const int MovementThresholdPixels = 5;

        // Handle mouse‐move: enter drag‐lock only after moving & holding for ≥ DragStartTimeMilliseconds
        if (settings.IsDragCorrectionEnabled && wParam == (IntPtr)WM_MOUSEMOVE)
        {
            foreach (var button in currentlyDownButtons.ToList())
            {
                // update last‐movement timestamp
                lastMoveTime[button] = hookStruct.time;

                // compute squared distance from initial down
                var start = initialDownPosition[button];
                int dx = hookStruct.pt.x - start.x;
                int dy = hookStruct.pt.y - start.y;
                int distSq = dx * dx + dy * dy;

                // time since genuine down
                long elapsedSinceDown = hookStruct.time - initialDownTime[button];

                if (!isDragLocked.GetValueOrDefault(button, false)
                    && distSq >= MovementThresholdPixels * MovementThresholdPixels
                    && elapsedSinceDown >= settings.DragStartTimeMilliseconds)
                {
                    isDragLocked[button] = true;
                    logger.Log(string.Format(Resources.EnterDragLock, buttonTextLookup[button], elapsedSinceDown), true);
                }
            }
        }

        // Only intercept button messages we care about
        if (!ProcessMouseEvent(nCode, wParam))
        {
            return nativeMethods.CallNextHook(IntPtr.Zero, nCode, wParam, lParam);
        }

        bool buttonDown = false;
        bool buttonUp = false;
        MouseButtons activeButton;
        int threshold = 0;

        // Identify which button and whether it's down or up
        switch (wParam)
        {
            case WM_LBUTTONDOWN:
                buttonDown = true;
                activeButton = MouseButtons.Left;
                threshold = settings.LeftThreshold;
                break;

            case WM_LBUTTONUP:
                buttonUp = true;
                activeButton = MouseButtons.Left;
                break;

            case WM_RBUTTONDOWN:
                buttonDown = true;
                activeButton = MouseButtons.Right;
                threshold = settings.RightThreshold;
                break;

            case WM_RBUTTONUP:
                buttonUp = true;
                activeButton = MouseButtons.Right;
                break;

            case WM_MBUTTONDOWN:
                buttonDown = true;
                activeButton = MouseButtons.Middle;
                threshold = settings.MiddleThreshold;
                break;

            case WM_MBUTTONUP:
                buttonUp = true;
                activeButton = MouseButtons.Middle;
                break;

            case WM_XBUTTONDOWN:
                buttonDown = true;
                activeButton = GetXButton(hookStruct.mouseData);
                threshold = (activeButton == MouseButtons.XButton1)
                                ? settings.X1Threshold
                                : settings.X2Threshold;
                break;

            case WM_XBUTTONUP:
                buttonUp = true;
                activeButton = GetXButton(hookStruct.mouseData);
                break;

            default:
                return nativeMethods.CallNextHook(IntPtr.Zero, nCode, wParam, lParam);
        }

        // If we're in drag‐lock, suppress spurious downs/ups
        if (settings.IsDragCorrectionEnabled && isDragLocked.GetValueOrDefault(activeButton, false))
        {
            if (buttonDown)
            {
                // drop any extra presses
                return (IntPtr)1;
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
                    previousUpTime[activeButton] = hookStruct.time;
                    logger.Log(string.Format(Resources.ExitDragLock, buttonTextLookup[activeButton]), true);

                    return nativeMethods.CallNextHook(IntPtr.Zero, nCode, wParam, lParam);
                }
                // still jittering: suppress
                return (IntPtr)1;
            }
        }

        // Normal double‐click suppression and down/up tracking
        // We take the elapsed time between the last mouse up and the current mouse down event.
        // If it's smaller than the minimal delay, we ignore the current mouse down event.
        if (buttonDown)
        {
            // record genuine down
            currentlyDownButtons.Add(activeButton);
            initialDownPosition[activeButton] = hookStruct.pt;
            initialDownTime[activeButton] = hookStruct.time;   // ensure Dictionary<MouseButtons,long> exists
            lastMoveTime[activeButton] = hookStruct.time;

            long delta = hookStruct.time - previousUpTime[activeButton];
            bool belowMin = settings.MinDelay >= 0 && delta <= settings.MinDelay;
            bool ignore = delta < threshold && !belowMin;

            if (ignore)
            {
                ignoredClicks++;
                logger.Log(
                    $"{ignoredDoubleClickText} ({buttonTextLookup[activeButton]}): {delta} ms (#{ignoredClicks})"
                );
                previousUpTime[activeButton] = 0;
                return (IntPtr)1;
            }
            else if (delta < settings.WindowsDoubleClickTimeMilliseconds)
            {
                logger.Log($"{delta} ms ({buttonTextLookup[activeButton]})", true);
            }
        }
        else if (buttonUp)
        {
            // genuine up
            currentlyDownButtons.Remove(activeButton);
            previousUpTime[activeButton] = hookStruct.time;
        }

        // forward everything else
        return nativeMethods.CallNextHook(IntPtr.Zero, nCode, wParam, lParam);
    }




    private bool ProcessMouseEvent(int nCode, nint wParam)
    {
        return nCode >= 0
            && settings.IgnoredDevice != currentDevice
            && observedMessages.Contains(wParam);
    }

    private MouseButtons GetXButton(uint mouseData)
    {
        const int MK_XBUTTON1_DOWN = 0x0020;
        const int MK_XBUTTON2_DOWN = 0x0040;
        const int XBUTTON1_UP = 0x0001;
        const int XBUTTON2_UP = 0x0002;

        ushort loWord = unchecked((ushort)(ulong)mouseData);
        ushort hiWord = unchecked((ushort)((ulong)mouseData >> 16));
        if (settings.X1Threshold >=0 && ((loWord & MK_XBUTTON1_DOWN) != 0 || (hiWord & XBUTTON1_UP) != 0))
        {
            return MouseButtons.XButton1;
        }
        else if (settings.X2Threshold >= 0 && ((loWord & MK_XBUTTON2_DOWN) != 0 || (hiWord & XBUTTON2_UP) != 0))
        {
            return MouseButtons.XButton2;
        }
        return MouseButtons.None;
    }
      
    private static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using ProcessModule currentModule = Process.GetCurrentProcess().MainModule!;
        return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
    }

    public void Dispose()
    {
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        Uninstall();
    }
}