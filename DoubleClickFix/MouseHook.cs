using DoubleClickFix.Properties;
using Microsoft.Win32;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix;

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

        if (!RegisterRawInputDevices(device, (uint)device.Length, (uint)Marshal.SizeOf<RAWINPUTDEVICE>()))
        {
            logger.Log("Failed to register raw input device."); // TODO translate.
        }
    }

    public void ProcessRawInput(IntPtr rawInputHandle)
    {
        uint rawInputSize = 0;
        // Determine buffer size
        _ = GetRawInputData(rawInputHandle, RID_INPUT, IntPtr.Zero, ref rawInputSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>());

        IntPtr rawInputBuffer = Marshal.AllocHGlobal((int)rawInputSize);
        try
        {
            if (GetRawInputData(rawInputHandle, RID_INPUT, rawInputBuffer, ref rawInputSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>()) != rawInputSize)
                return;

            var rawInput = Marshal.PtrToStructure<RAWINPUT>(rawInputBuffer)!;
            if (rawInput.Header.Type == RIM_TYPEMOUSE)
            {
                IntPtr deviceHandle = rawInput.Header.Device;
                if (currentDevice != deviceHandle)
                {
                    currentDevice = deviceHandle;
                    LogRawInputDeviceName(deviceHandle);
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(rawInputBuffer);
        }
    }

    private void LogRawInputDeviceName(IntPtr deviceHandle)
    {
        // 1) Get required buffer size for the raw name
        uint nameBufferSize = 0;
        GetRawInputDeviceInfo(deviceHandle, RIDI_DEVICENAME, IntPtr.Zero, ref nameBufferSize);
        if (nameBufferSize == 0)
        {
            logger.Log($"{Resources.SwitchedDevice} Unknown ({deviceHandle})", true);
            return;
        }

        // 2) Read the raw device path
        var pathBuilder = new System.Text.StringBuilder((int)nameBufferSize);
        if (GetRawInputDeviceInfo(deviceHandle, RIDI_DEVICENAME, pathBuilder, ref nameBufferSize) == 0)
        {
            logger.Log($"{Resources.SwitchedDevice} Unknown ({deviceHandle})", true);
            return;
        }
        string devicePath = pathBuilder.ToString();

        try
        {
            // 3) Convert raw path to PNPDeviceID:
            //    - Trim leading '\\' and '?'
            //    - Replace each '#' with '\'
            string pnpId = devicePath
                .TrimStart('\\', '?')
                .Replace('#', '\\');

            //    - Remove any "&Col..." suffix
            int suffixPos = pnpId.IndexOf("&Col", StringComparison.OrdinalIgnoreCase);
            if (suffixPos >= 0)
                pnpId = pnpId.Substring(0, suffixPos);

            // 4) Escape for WQL:
            //    - Double backslashes
            //    - Escape single quotes
            string escapedPnpId = pnpId
                .Replace("\\", "\\\\")
                .Replace("'", "''");

            // 5) Query WMI for friendly Name
            string wql = $"SELECT Name FROM Win32_PnPEntity WHERE PNPDeviceID LIKE '{escapedPnpId}%'";
            logger.Log($"{wql}", true);
            using var searcher = new ManagementObjectSearcher(wql);
            foreach (ManagementObject mo in searcher.Get())
            {
                if (mo["Name"] is string friendly && !string.IsNullOrEmpty(friendly))
                {
                    logger.Log($"{Resources.SwitchedDevice} {friendly}", true);
                    return;
                }
            }
        }
        catch
        {
            // WMI failed, fall back
        }

        // 6) Fallback: log the raw device path
        logger.Log($"{Resources.SwitchedDevice} {devicePath}", true);
    }

    // P/Invoke declarations for RawInput info
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, System.Text.StringBuilder pData, ref uint pcbSize);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

    private const uint RIDI_DEVICENAME = 0x20000007;

    // P/Invoke for SendInput
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U4)]
    private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint Type;
        public InputUnion Union;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT MouseInput;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Timestamp;
        public UIntPtr ExtraInfo;
    }

    internal IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (ProcessMouseEvent(nCode, wParam))
        {
            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam)!;
            bool buttonUp = false;
            bool buttonDown = false;
            int threshold = 50;
            MouseButtons button = MouseButtons.None;
            switch (wParam)
            {
                case WM_LBUTTONDOWN:
                    buttonDown = true;
                    button = MouseButtons.Left;
                    threshold = settings.LeftThreshold;
                    break;
                case WM_LBUTTONUP:
                    buttonUp = true;
                    button = MouseButtons.Left;
                    break;
                case WM_RBUTTONDOWN:
                    buttonDown = true;
                    button = MouseButtons.Right;
                    threshold = settings.RightThreshold;
                    break;
                case WM_RBUTTONUP:
                    buttonUp = true;
                    button = MouseButtons.Right;
                    break;
                case WM_MBUTTONDOWN:
                    buttonDown = true;
                    button = MouseButtons.Middle;
                    threshold = settings.MiddleThreshold;
                    break;
                case WM_MBUTTONUP:
                    buttonUp = true;
                    button = MouseButtons.Middle;
                    break;
                case WM_XBUTTONDOWN:
                    buttonDown = true;
                    button = GetXButton(hookStruct.mouseData);
                    threshold = button == MouseButtons.XButton1 ? settings.X1Threshold : settings.X2Threshold;
                    break;
                case WM_XBUTTONUP:
                    buttonUp = true;
                    button = GetXButton(hookStruct.mouseData);
                    break;
            }
            if (button != MouseButtons.None)
            {
                if (buttonDown)
                {
                    // We take the elapsed time between the last mouse up and the current mouse down event.
                    // If it's smaller than the minimal delay, we ignore the current mouse down event.
                    long timeDifference = hookStruct.time - previousUpTime[button];
                    bool belowMinDelay = settings.MinDelay >= 0 && timeDifference <= settings.MinDelay;
                    bool ignore = timeDifference < threshold && !belowMinDelay;
                    if (ignore)
                    {
                        ignoredClicks++;
                        logger.Log($"{ignoredDoubleClickText} ({buttonTextLookup[button]}): {timeDifference} ms (#{ignoredClicks})");
                        previousUpTime[button] = 0;
                        return (IntPtr)1;
                    }
                    else
                    {
                        if (timeDifference < settings.WindowsDoubleClickTimeMilliseconds)
                        {
                            logger.Log($"{timeDifference} ms ({buttonTextLookup[button]})", true);
                        }
                    }
                }
                else if (buttonUp)
                {
                    previousUpTime[button] = hookStruct.time;
                }
            }
        }
        return nativeMethods.CallNextHook(IntPtr.Zero, nCode, wParam, lParam);
    }

    private bool ProcessMouseEvent(int nCode, nint wParam)
    {
        return nCode >= 0 && settings.IgnoredDevice != currentDevice && observedMessages.Contains(wParam);
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