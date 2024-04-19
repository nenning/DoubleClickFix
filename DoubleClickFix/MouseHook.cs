using DoubleClickFix.Properties;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix
{
    internal class MouseHook(ILogger logger, Settings settings)
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private IntPtr hookID = IntPtr.Zero;
        private uint lastClickTime;

        public void Install()
        {
            if (hookID == IntPtr.Zero)
            {
                hookID = SetHook(this.HookCallback);
            }
        }

        public void Uninstall()
        {
            if (hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookID);
                hookID = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                MSLLHOOKSTRUCT? hookStruct = Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)) as MSLLHOOKSTRUCT?;
                if (hookStruct != null && IgnoreDoubleClick(hookStruct.Value))
                {
                    // Ignore the second click
                    return (IntPtr)1;
                }
                if (hookStruct != null)
                {
                    // Store the timestamp of this click for future comparison
                    lastClickTime = hookStruct.Value.time;
                }
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        private bool IgnoreDoubleClick(MSLLHOOKSTRUCT hookStruct)
        {
            long timeDifference = hookStruct.time - lastClickTime;
            bool ignore = timeDifference < settings.MinimumDoubleClickDelayMilliseconds;
            if (ignore)
            {
                logger.Log($"{Resources.IgnoredDoubleClick}: {timeDifference} ms");
            }
            else if (timeDifference < settings.WindowsDoubleClickTimeMilliseconds)
            {
                logger.Log($"{timeDifference} ms");
            }
            return ignore;
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using ProcessModule curModule = Process.GetCurrentProcess().MainModule!;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }
}