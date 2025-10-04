using System.ComponentModel;
using System.Runtime.InteropServices;

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
            new()
            {
                UsagePage = HID_USAGE_PAGE_GENERIC,
                Usage = HID_USAGE_GENERIC_MOUSE,
                Flags = RIDEV_INPUTSINK,
                Target = hwnd
            },
            new()
            {
                UsagePage = HID_USAGE_PAGE_GENERIC,
                Usage = HID_USAGE_GENERIC_POINTER,
                Flags = RIDEV_INPUTSINK,
                Target = hwnd
            },
            new()
            {
                UsagePage = HID_USAGE_PAGE_DIGITIZER,
                Usage = HID_USAGE_DIGITIZER_TOUCHSCREEN,
                Flags = RIDEV_INPUTSINK,
                Target = hwnd
            },
            new()
            {
                UsagePage = HID_USAGE_PAGE_DIGITIZER,
                Usage = HID_USAGE_DIGITIZER_TOUCHPAD,
                Flags = RIDEV_INPUTSINK,
                Target = hwnd
            }
        };

        if (!RegisterRawInputDevices(devices, (uint)devices.Length, (uint)Marshal.SizeOf<RAWINPUTDEVICE>()))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    public bool TryProcessRawInput(nint hRawInput, out nint device)
    {
        uint dwSize = 0;
        _ = GetRawInputData(hRawInput, RID_INPUT, nint.Zero, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>());
        // refuse to allocate absurdly large buffers
        if (dwSize == 0 || dwSize > int.MaxValue)
        {
            device = 0;
            return false;
        }
        nint buffer = Marshal.AllocHGlobal((IntPtr)dwSize);
        try
        {
            if (GetRawInputData(hRawInput, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf<RAWINPUTHEADER>()) != dwSize)
            {
                device = 0;
                return false;
            }

            var raw = Marshal.PtrToStructure<RAWINPUT>(buffer);
            if (raw.Header.Type == RIM_TYPEMOUSE || raw.Header.Type == RIM_TYPEHID)
            {
                device = raw.Header.Device;
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
    private const int HID_USAGE_GENERIC_POINTER = 0x01;
    private const int HID_USAGE_PAGE_DIGITIZER = 0x0D;
    private const int HID_USAGE_DIGITIZER_TOUCHSCREEN = 0x04;
    private const int HID_USAGE_DIGITIZER_TOUCHPAD = 0x05;

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
    private struct RAWHID
    {
        public uint dwSizeHid;
        public uint dwCount;
        public byte bRawData;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RAWINPUT
    {
        public RAWINPUTHEADER Header;
        public Data Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct Data
    {
        [FieldOffset(0)]
        public RAWMOUSE Mouse;
        [FieldOffset(0)]
        public RAWHID Hid;
    }

    private const uint RID_INPUT = 0x10000003;
    internal const uint RIM_TYPEMOUSE = 0x00000000;
    private const uint RIM_TYPEHID = 0x00000002;
    internal const uint RIDI_DEVICEINFO = 0x2000000b;

    [DllImport("User32.dll", SetLastError = true)]
    private static extern uint GetRawInputDeviceInfo(nint hDevice, uint uiCommand, nint pData, ref uint pcbSize);
    
    public bool TryGetRawInputDeviceInfo(nint deviceHandle, uint uiCommand, out RID_DEVICE_INFO pData)
    {
        uint size = (uint)Marshal.SizeOf<RID_DEVICE_INFO>();
        nint buffer = Marshal.AllocHGlobal((int)size);
        try
        {
            var result = GetRawInputDeviceInfo(deviceHandle, uiCommand, buffer, ref size);
            if (result > 0)
            {
                pData = Marshal.PtrToStructure<RID_DEVICE_INFO>(buffer);
                return true;
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

        pData = new RID_DEVICE_INFO();
        return false;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RID_DEVICE_INFO
    {
        public uint cbSize;
        public uint dwType;
        public RID_DEVICE_INFO_MOUSE mouse;
        public RID_DEVICE_INFO_KEYBOARD keyboard;
        public RID_DEVICE_INFO_HID hid;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RID_DEVICE_INFO_MOUSE
    {
        public uint dwId;
        public uint dwNumberOfButtons;
        public uint dwSampleRate;
        [MarshalAs(UnmanagedType.Bool)] public bool fHasHorizontalWheel;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RID_DEVICE_INFO_KEYBOARD
    {
        public uint dwType;
        public uint dwSubType;
        public uint dwKeyboardMode;
        public uint dwNumberOfFunctionKeys;
        public uint dwNumberOfIndicators;
        public uint dwNumberOfKeysTotal;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RID_DEVICE_INFO_HID
    {
        public uint dwVendorId;
        public uint dwProductId;
        public uint dwVersionNumber;
        public ushort usUsagePage;
        public ushort usUsage;
    }

    // For showing the existing window
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
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

    // For hiding the window from alt-tab
    internal const int GWL_EXSTYLE = -20;
    internal const int WS_EX_TOOLWINDOW = 0x00000080;

    [DllImport("user32.dll")]
    internal static extern nint GetWindowLong(nint hWnd, int nIndex);

    nint INativeMethods.GetWindowLong(nint hWnd, int nIndex) => GetWindowLong(hWnd, nIndex);

    [DllImport("user32.dll")]
    internal static extern nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong);

    nint INativeMethods.SetWindowLong(nint hWnd, int nIndex, nint dwNewLong) => SetWindowLong(hWnd, nIndex, dwNewLong);
}