using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DoubleClickFix;

public sealed class InputCapabilities
{
    /// <summary>True if a touch digitizer is present and ready, or if maximum touches &gt; 0.</summary>
    public bool HasTouchscreen { get; init; }

    /// <summary>True if Windows reports a precision touchpad via the Pointer Device API (Win8.1+).</summary>
    public bool HasPrecisionTouchpad { get; init; }

    /// <summary>True if any mouse-like pointer is present (a touchpad counts as a mouse to Windows).</summary>
    public bool HasMouseLikePointer { get; init; }

    /// <summary>Human-readable description for logging/diagnostics.</summary>
    public string Description
        => HasTouchscreen switch
        {
            true when HasPrecisionTouchpad => "Touchscreen + precision touchpad",
            true when !HasPrecisionTouchpad && HasMouseLikePointer => "Touchscreen + mouse",
            true => "Touchscreen only",
            false when HasPrecisionTouchpad => "Precision touchpad",
            false when HasMouseLikePointer => "Mouse only",
            _ => "No standard pointer device detected"
        };

    /// <summary>
    /// Reads the current capabilities once (no event subscription). Safe to call on any thread.
    /// </summary>
    public static InputCapabilities Detect()
        => new()
        {
            HasTouchscreen = DetectTouchscreen(),
            HasPrecisionTouchpad = DetectPrecisionTouchpad(),
            HasMouseLikePointer = DetectMouseLikePointer()
        };

    // --- Touchscreen (reliable on Win7+) ----------------------------------

    private static bool DetectTouchscreen()
    {
        try
        {
            int caps = GetSystemMetrics(SM_DIGITIZER);
            bool touchPresent = (caps & (NID_INTEGRATED_TOUCH | NID_EXTERNAL_TOUCH)) != 0;
            bool ready = (caps & NID_READY) != 0;
            if (touchPresent && ready) return true;

            // Fallback: some machines report only max touches
            return GetSystemMetrics(SM_MAXIMUMTOUCHES) > 0;
        }
        catch
        {
            return false;
        }
    }

    // --- Touchpad (precision touchpad via Pointer Device API) --------------

    private static bool DetectPrecisionTouchpad()
    {
        // API exists starting in Windows 8; TOUCH_PAD type is reliably reported from 8.1+.
        try
        {
            uint count = 0;
            if (!GetPointerDevices(ref count, IntPtr.Zero) || count == 0)
                return false;

            int size = Marshal.SizeOf<POINTER_DEVICE_INFO>();
            IntPtr buffer = Marshal.AllocHGlobal(size * (int)count);

            try
            {
                if (!GetPointerDevices(ref count, buffer) || count == 0)
                    return false;

                for (int i = 0; i < count; i++)
                {
                    IntPtr itemPtr = buffer + i * size;
                    var info = Marshal.PtrToStructure<POINTER_DEVICE_INFO>(itemPtr);
                    if (info.pointerDeviceType == POINTER_DEVICE_TYPE.POINTER_DEVICE_TYPE_TOUCH_PAD)
                        return true;
                }

                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        catch (EntryPointNotFoundException)
        {
            // Older Windows: API not present
            return false;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (Win32Exception)
        {
            return false;
        }
    }

    // --- Mouse-like pointer ------------------------------------------------

    private static bool DetectMouseLikePointer()
    {
        // WinForms exposes SystemInformation.MousePresent (SM_MOUSEPRESENT).
        try
        {
            return SystemInformation.MousePresent;
        }
        catch
        {
            return false;
        }
    }

    // -------------------- P/Invoke -----------------------------------------

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    private const int SM_DIGITIZER = 94;
    private const int SM_MAXIMUMTOUCHES = 95;

    // winuser.h flags
    private const int NID_INTEGRATED_TOUCH = 0x1;
    private const int NID_EXTERNAL_TOUCH = 0x2;
    private const int NID_READY = 0x80;

    // Pointer Device API
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetPointerDevices(ref uint deviceCount, IntPtr pointerDevices);

    private enum POINTER_DEVICE_TYPE : uint
    {
        POINTER_DEVICE_TYPE_INTEGRATED_PEN = 0x00000001,
        POINTER_DEVICE_TYPE_EXTERNAL_PEN = 0x00000002,
        POINTER_DEVICE_TYPE_TOUCH = 0x00000003,
        POINTER_DEVICE_TYPE_TOUCH_PAD = 0x00000004
    }

    // NOTE: Matches Windows header layout. productString is WCHAR[520].
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct POINTER_DEVICE_INFO
    {
        public uint displayOrientation;
        public IntPtr device; // HANDLE
        public POINTER_DEVICE_TYPE pointerDeviceType;
        public IntPtr monitor; // HMONITOR
        public uint startingCursorId;
        public ushort maxActiveContacts;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 520)]
        public string productString;
    }
}
