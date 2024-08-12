using DoubleClickFix.Properties;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix;

internal class MouseHook : IDisposable
{
    private const int WH_MOUSE_LL = 14;

    private const IntPtr WM_LBUTTONDOWN = 0x0201;
    private const IntPtr WM_LBUTTONUP = 0x0202;
    private const IntPtr WM_RBUTTONDOWN = 0x0204;
    private const IntPtr WM_RBUTTONUP = 0x0205;
    private const IntPtr WM_MBUTTONDOWN = 0x0207;
    private const IntPtr WM_MBUTTONUP = 0x0208;
    private const IntPtr WM_XBUTTONDOWN = 0x020B;
    private const IntPtr WM_XBUTTONUP = 0x020C;

    private readonly Settings settings;
    private static ILogger logger;

    // make sure we keep a reference so it's not garbage collected
    private LowLevelMouseProc? mouseProc;
    private IntPtr hookHandle = IntPtr.Zero;

    private readonly HashSet<IntPtr> observedMessages = [];
    readonly Dictionary<MouseButtons, uint> previousUpTime = new() { {MouseButtons.Left , 0 }, {MouseButtons.Right , 0}, {MouseButtons.Middle , 0}, {MouseButtons.XButton1 , 0}, {MouseButtons.XButton2 , 0} };
    private uint ignoredClicks = 0;

    public MouseHook(Settings settings, ILogger logger)
    {
        this.settings = settings;
        SettingsChanged();
        settings.RegisterSettingsChangedListener(SettingsChanged);
        MouseHook.logger = logger;
        SystemEvents.PowerModeChanged += OnPowerModeChanged;
    }

    private void SettingsChanged()
    {
        observedMessages.Clear();
        if (settings.LeftThreshold >= 0) { 
            observedMessages.Add(WM_LBUTTONDOWN);
            observedMessages.Add(WM_LBUTTONUP);
        }
        if (settings.RightThreshold >= 0)
        {
            observedMessages.Add(WM_RBUTTONDOWN);
            observedMessages.Add(WM_RBUTTONUP);
        }
        if (settings.MiddleThreshold >= 0)
        {
            observedMessages.Add(WM_MBUTTONDOWN);
            observedMessages.Add(WM_MBUTTONUP);
        }
        if (settings.X1Threshold >= 0 || settings.X2Threshold >= 0)
        {
            observedMessages.Add(WM_XBUTTONDOWN);
            observedMessages.Add(WM_XBUTTONUP);
        }
    }

    public bool Install(nint handle)
    {
        RegisterForRawInput(handle);

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
                if (!Install(IntPtr.Zero))
                {
                    logger.Log("Failed to reinstall mouse hook after Windows resume."); // TODO translate.
                }
                break;
            default:
                break;
        }
    }

    [DllImport("User32.dll", SetLastError = true)]
    public static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTDEVICE
    {
        public ushort UsagePage;
        public ushort Usage;
        public uint Flags;
        public IntPtr Target;
    }

    public const int RIDEV_INPUTSINK = 0x00000100;
    public const int HID_USAGE_PAGE_GENERIC = 0x01;
    public const int HID_USAGE_GENERIC_MOUSE = 0x02;


    public const ushort RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001;
    public const ushort RI_MOUSE_LEFT_BUTTON_UP = 0x0002;
    public const ushort RI_MOUSE_RIGHT_BUTTON_DOWN = 0x0004;
    public const ushort RI_MOUSE_RIGHT_BUTTON_UP = 0x0008;
    public const ushort RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010;
    public const ushort RI_MOUSE_MIDDLE_BUTTON_UP = 0x0020;
    public const ushort RI_MOUSE_BUTTON_4_DOWN = 0x0040;
    public const ushort RI_MOUSE_BUTTON_4_UP = 0x0080;
    public const ushort RI_MOUSE_BUTTON_5_DOWN = 0x0100;
    public const ushort RI_MOUSE_BUTTON_5_UP = 0x0200;

    public static void RegisterForRawInput(IntPtr hwnd)
    {
        RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];

        rid[0].UsagePage = HID_USAGE_PAGE_GENERIC;
        rid[0].Usage = HID_USAGE_GENERIC_MOUSE;
        rid[0].Flags = RIDEV_INPUTSINK;
        rid[0].Target = hwnd;

        if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
        {
            throw new ApplicationException("Failed to register raw input devices.");
        }
    }

    [DllImport("User32.dll")]
    public static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTHEADER
    {
        public uint Type;
        public uint Size;
        public IntPtr Device;
        public IntPtr wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWMOUSE
    {
        public ushort Flags;
        public ushort ButtonFlags;
        public ushort ButtonData;
        public uint RawButtons;
        public int LastX;
        public int LastY;
        public uint ExtraInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUT
    {
        public RAWINPUTHEADER Header;
        public RAWMOUSE Mouse;
    }

    public const uint RID_INPUT = 0x10000003;
    public const uint RIM_TYPEMOUSE = 0x00000000;
    private static nint lastDevice = 0;
    public static void ProcessRawInput(IntPtr hRawInput)
    {
        uint dwSize = 0;
        GetRawInputData(hRawInput, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));
        IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
        try
        {
            if (GetRawInputData(hRawInput, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) != dwSize)
                return;

            RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));

            if (raw.Header.Type == RIM_TYPEMOUSE)
            {
                var device = raw.Header.Device;
                if (lastDevice != device)
                {
                    logger.Log("Switched mouse device to " + device);
                }
                lastDevice = device;
                //if ((raw.Mouse.ButtonFlags & RI_MOUSE_LEFT_BUTTON_DOWN) != 0 ||
                //    (raw.Mouse.ButtonFlags & RI_MOUSE_RIGHT_BUTTON_DOWN) != 0 ||
                //    (raw.Mouse.ButtonFlags & RI_MOUSE_MIDDLE_BUTTON_DOWN) != 0 ||
                //    (raw.Mouse.ButtonFlags & RI_MOUSE_BUTTON_4_DOWN) != 0 ||
                //    (raw.Mouse.ButtonFlags & RI_MOUSE_BUTTON_5_DOWN) != 0)
                //{
                //    logger.Log("raw: " + raw.Header.Device.ToString() + ", " + raw.Mouse.ExtraInformation + ", " + raw.Mouse.ButtonFlags);
                //    // Additional logic for detecting input device type
                //}

            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && observedMessages.TryGetValue(wParam, out _))
        {
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))!;
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
                        string buttonText = TranslateButton(button);
                        logger.Log($"{Resources.IgnoredDoubleClick} ({buttonText}): {timeDifference} ms (#{ignoredClicks})");
                        previousUpTime[button] = 0;
                        return (IntPtr)1;
                    }
                    else
                    {
                        if (timeDifference < settings.WindowsDoubleClickTimeMilliseconds)
                        {
                            string buttonText = TranslateButton(button);

                            logger.Log($"{timeDifference} ms ({buttonText})");
                        }
                    }
                }
                else if (buttonUp)
                {
                    previousUpTime[button] = hookStruct.time;

                }
            }
        }
        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }

    private static string TranslateButton(MouseButtons button)
    {
        return button switch
        {
            MouseButtons.Left => Resources.Left,
            MouseButtons.Right => Resources.Right,
            MouseButtons.Middle => Resources.Middle,
            MouseButtons.XButton1 => Resources.X1,
            MouseButtons.XButton2 => Resources.X2,
            _ => "?",
        };
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