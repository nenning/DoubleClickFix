using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DoubleClickFix;

internal class NativeMethods : INativeMethods
{
    internal const int WH_MOUSE_LL = 14;

    internal const nint WM_MOUSEMOVE = 0x0200;
    internal const nint WM_LBUTTONDOWN = 0x0201;
    internal const nint WM_LBUTTONUP = 0x0202;
    internal const nint WM_RBUTTONDOWN = 0x0204;
    internal const nint WM_RBUTTONUP = 0x0205;
    internal const nint WM_MBUTTONDOWN = 0x0207;
    internal const nint WM_MBUTTONUP = 0x0208;
    internal const nint WM_XBUTTONDOWN = 0x020B;
    internal const nint WM_XBUTTONUP = 0x020C;

    [DllImport("user32.dll")]
    internal static extern int GetDoubleClickTime();

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

    internal delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, nint hMod, uint dwThreadId);

    internal static nint SetWindowsHook(int idHook, LowLevelMouseProc lpfn, nint hMod, uint dwThreadId)
    {
        var hhk = SetWindowsHookEx(idHook, lpfn, hMod, dwThreadId);
        return hhk == nint.Zero ? throw new Win32Exception(Marshal.GetLastWin32Error()) : hhk;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    internal static void UnhookWindowsHook(nint hhk)
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

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint GetModuleHandle(string lpModuleName);

    internal static nint GetModuleHandleOrThrow(string lpModuleName)
    {
        var handle = GetModuleHandle(lpModuleName);
        return handle == nint.Zero ? throw new Win32Exception(Marshal.GetLastWin32Error()) : handle;
    }

    // raw input methods

    [DllImport("User32.dll", SetLastError = true)]
    private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

    internal static void RegisterRawInputDevicesOrThrow(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize)
    {
        if (!RegisterRawInputDevices(pRawInputDevice, uiNumDevices, cbSize))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTDEVICE
    {
        public ushort UsagePage;
        public ushort Usage;
        public uint Flags;
        public nint Target;
    }

    internal const int RIDEV_INPUTSINK = 0x00000100;
    internal const int HID_USAGE_PAGE_GENERIC = 0x01;
    internal const int HID_USAGE_GENERIC_MOUSE = 0x02;

    [DllImport("User32.dll")]
    internal static extern uint GetRawInputData(nint hRawInput, uint uiCommand, nint pData, ref uint pcbSize, uint cbSizeHeader);

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTHEADER
    {
        public uint Type;
        public uint Size;
        public nint Device;
        public nint wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWMOUSE
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
    internal struct RAWINPUT
    {
        public RAWINPUTHEADER Header;
        public RAWMOUSE Mouse;
    }

    internal const uint RID_INPUT = 0x10000003;
    internal const uint RIM_TYPEMOUSE = 0x00000000;

    // For showing the existing window
    [DllImport("user32.dll")]
    internal static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

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
}