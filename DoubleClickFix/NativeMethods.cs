using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DoubleClickFix;

internal class NativeMethods: INativeMethods
{
    internal const int WH_MOUSE_LL = 14;

    internal const IntPtr WM_LBUTTONDOWN = 0x0201;
    internal const IntPtr WM_LBUTTONUP = 0x0202;
    internal const IntPtr WM_RBUTTONDOWN = 0x0204;
    internal const IntPtr WM_RBUTTONUP = 0x0205;
    internal const IntPtr WM_MBUTTONDOWN = 0x0207;
    internal const IntPtr WM_MBUTTONUP = 0x0208;
    internal const IntPtr WM_XBUTTONDOWN = 0x020B;
    internal const IntPtr WM_XBUTTONUP = 0x020C;

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
        public IntPtr dwExtraInfo;
    }

    internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    public IntPtr CallNextHook(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam)
    {
        return CallNextHookEx(hhk, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr GetModuleHandle(string lpModuleName);

    // raw input methods

    [DllImport("User32.dll", SetLastError = true)]
    internal static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTDEVICE
    {
        public ushort UsagePage;
        public ushort Usage;
        public uint Flags;
        public IntPtr Target;
    }

    internal const int RIDEV_INPUTSINK = 0x00000100;
    internal const int HID_USAGE_PAGE_GENERIC = 0x01;
    internal const int HID_USAGE_GENERIC_MOUSE = 0x02;

    [DllImport("User32.dll")]
    internal static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTHEADER
    {
        public uint Type;
        public uint Size;
        public IntPtr Device;
        public IntPtr wParam;
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

}
