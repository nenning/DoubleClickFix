using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace DoubleClickFix;

internal class NativeMethods : INativeMethods
{
    private const int WH_MOUSE_LL = 14;

    internal const nint WM_MOUSEMOVE = 0x0200;
    internal const nint WM_LBUTTONDOWN = 0x0201;
    internal const nint WM_LBUTTONUP = 0x0202;
    internal const nint WM_RBUTTONDOWN = 0x0204;
    internal const nint WM_RBUTTONUP = 0x0205;
    internal const nint WM_MBUTTONDOWN = 0x0207;
    internal const nint WM_MBUTTONUP = 0x0208;
    internal const nint WM_XBUTTONDOWN = 0x020B;
    internal const nint WM_XBUTTONUP = 0x020C;
    internal const nint WM_MOUSEWHEEL = 0x020A;
    internal const nint WM_MOUSEHWHEEL = 0x020E;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    internal static extern uint RegisterApplicationRestart(string? pwzCommandLine, int dwFlags);

    [DllImport("user32.dll")]
    internal static extern int GetDoubleClickTime();

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    private const int SM_REMOTESESSION = 0x1000;

    public bool IsRemoteSession() => GetSystemMetrics(SM_REMOTESESSION) != 0;

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public nint dwExtraInfo;
    }

    internal enum DeviceType { Mouse, TouchScreen, TouchPad }

    internal delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, nint hMod, uint dwThreadId);

    public nint SetHook(LowLevelMouseProc lpfn)
    {
        var hhk = SetWindowsHookEx(WH_MOUSE_LL, lpfn, nint.Zero, 0);
        return hhk == nint.Zero ? throw new Win32Exception(Marshal.GetLastWin32Error()) : hhk;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    public void UnhookWindowsHook(nint hhk)
    {
        if (!UnhookWindowsHookEx(hhk))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    public nint CallNextHook(nint hhk, int nCode, nint wParam, nint lParam)
    {
        return CallNextHookEx(hhk, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    // raw input methods

    [DllImport("User32.dll", SetLastError = true)]
    private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

    public void RegisterForRawInput(nint hwnd)
    {
        var devices = new RAWINPUTDEVICE[]
        {
            new() {
                UsagePage = HID_USAGE_PAGE_GENERIC,
                Usage = HID_USAGE_GENERIC_MOUSE,
                Flags = RIDEV_INPUTSINK,
                Target = hwnd
            },
            new() {
                UsagePage = HID_USAGE_PAGE_DIGITIZER,
                Usage = HID_USAGE_DIGITIZER_TOUCHPAD,
                Flags = RIDEV_INPUTSINK,
                Target = hwnd
            },
            new() {
                UsagePage = HID_USAGE_PAGE_DIGITIZER,
                Usage = HID_USAGE_DIGITIZER_TOUCHSCREEN,
                Flags = RIDEV_INPUTSINK,
                Target = hwnd
            }
        };

        if (!RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf<RAWINPUTDEVICE>()))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    private const uint RIDI_DEVICENAME = 0x20000007;

    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    private static extern uint GetRawInputDeviceInfo(nint hDevice, uint uiCommand, nint pData, ref uint pcbSize);

    public string? TryGetDevicePath(nint hDevice)
    {
        uint size = 0;
        GetRawInputDeviceInfo(hDevice, RIDI_DEVICENAME, nint.Zero, ref size);
        if (size == 0) return null;
        nint buf = Marshal.AllocHGlobal((int)(size * 2));
        try
        {
            return GetRawInputDeviceInfo(hDevice, RIDI_DEVICENAME, buf, ref size) > 0
                ? Marshal.PtrToStringUni(buf)
                : null;
        }
        finally { Marshal.FreeHGlobal(buf); }
    }

    private const uint RID_HEADER = 0x10000005;
    private static readonly nint s_headerBuffer = Marshal.AllocHGlobal(Marshal.SizeOf<RAWINPUTHEADER>());

    public bool TryGetRawInputDeviceHandle(nint hRawInput, out nint device)
    {
        uint size = (uint)Marshal.SizeOf<RAWINPUTHEADER>();
        uint copied = GetRawInputData(hRawInput, RID_HEADER, s_headerBuffer, ref size, (uint)Marshal.SizeOf<RAWINPUTHEADER>());
        if (copied == uint.MaxValue)
        {
            device = 0;
            return false;
        }
        device = Marshal.PtrToStructure<RAWINPUTHEADER>(s_headerBuffer).Device;
        return true;
    }

    public bool TryProcessRawInput(nint hRawInput, out nint device, out DeviceType deviceType)
    {
        uint dwSize = 0;
        _ = GetRawInputData(hRawInput, RID_INPUT, nint.Zero, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>());
        // refuse to allocate absurdly large buffers
        if (dwSize == 0 || dwSize > int.MaxValue)
        {
            device = 0;
            deviceType = DeviceType.Mouse;
            return false;
        }
        nint buffer = Marshal.AllocHGlobal((IntPtr)dwSize);
        try
        {
            if (GetRawInputData(hRawInput, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>()) != dwSize)
            {
                device = 0;
                deviceType = DeviceType.Mouse;
                return false;
            }

            var raw = Marshal.PtrToStructure<RAWINPUT>(buffer);
            if (raw.Header.Type == RIM_TYPEMOUSE)
            {
                device = raw.Header.Device;
                deviceType = (raw.Mouse.Flags & MOUSE_MOVE_ABSOLUTE) != 0 ? DeviceType.TouchScreen : DeviceType.Mouse;
                return true;
            }
            if (raw.Header.Type == RIM_TYPEHID)
            {
                device = raw.Header.Device;
                deviceType = DeviceType.TouchPad;
                return true;
            }
        }
        finally
        {
            if (buffer != nint.Zero)
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        device = 0;
        deviceType = DeviceType.Mouse;
        return false;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RAWINPUTDEVICE
    {
        public ushort UsagePage;
        public ushort Usage;
        public uint Flags;
        public nint Target;
    }

    private const int RIDEV_INPUTSINK = 0x00000100;
    private const int HID_USAGE_PAGE_GENERIC = 0x01;
    private const int HID_USAGE_GENERIC_MOUSE = 0x02;
    private const int HID_USAGE_PAGE_DIGITIZER = 0x0D;
    private const int HID_USAGE_DIGITIZER_TOUCHPAD = 0x05;
    private const int HID_USAGE_DIGITIZER_TOUCHSCREEN = 0x04;

    [DllImport("User32.dll")]
    private static extern uint GetRawInputData(nint hRawInput, uint uiCommand, nint pData, ref uint pcbSize, uint cbSizeHeader);

    [StructLayout(LayoutKind.Sequential)]
    private struct RAWINPUTHEADER
    {
        public uint Type;
        public uint Size;
        public nint Device;
        public nint wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RAWMOUSE
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
    private struct RAWINPUT
    {
        public RAWINPUTHEADER Header;
        public RAWMOUSE Mouse;
    }

    private const uint RID_INPUT = 0x10000003;
    private const uint RIM_TYPEMOUSE = 0x00000000;
    private const uint RIM_TYPEHID = 0x00000002;
    private const uint MOUSE_MOVE_ABSOLUTE = 0x0001;

    // For showing the existing window
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AllowSetForegroundWindow(int dwProcessId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    internal const int SW_RESTORE = 9;
    internal const uint WM_SHOWME = 0x8001; // Custom message

    // For hiding the window from alt-tab
    internal const int GWL_EXSTYLE = -20;
    internal const int WS_EX_TOOLWINDOW = 0x00000080;

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern int GetWindowLong32(nint hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern nint GetWindowLong64(nint hWnd, int nIndex);

    internal static nint GetWindowLong(nint hWnd, int nIndex) =>
        nint.Size == 4 ? GetWindowLong32(hWnd, nIndex) : GetWindowLong64(hWnd, nIndex);

    nint INativeMethods.GetWindowLong(nint hWnd, int nIndex) => GetWindowLong(hWnd, nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(nint hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern nint SetWindowLong64(nint hWnd, int nIndex, nint dwNewLong);

    internal static nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong) =>
        nint.Size == 4 ? SetWindowLong32(hWnd, nIndex, (int)dwNewLong) : SetWindowLong64(hWnd, nIndex, dwNewLong);

    nint INativeMethods.SetWindowLong(nint hWnd, int nIndex, nint dwNewLong) => SetWindowLong(hWnd, nIndex, dwNewLong);

    // Dark mode support

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(nint hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    internal static bool IsDarkMode(ColorMode colorMode = ColorMode.System)
    {
        if (colorMode == ColorMode.Dark) return true;
        if (colorMode == ColorMode.Light) return false;
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
        }
        catch { return false; }
    }

    internal static void ApplyDarkTitleBar(nint hwnd, ColorMode colorMode = ColorMode.System)
    {
        int dark = IsDarkMode(colorMode) ? 1 : 0;
        DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
    }
}