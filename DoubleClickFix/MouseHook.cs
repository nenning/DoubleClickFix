using DoubleClickFix.Properties;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix
{
    //public enum MouseButtons {
    //    Left = 0b1,
    //    Right = 0b01,
    //    Middle = 0b001,
    //}
    /// <summary>
    /// TODO: try using a mouse hook on a high prio background thread to reduce the risk of unhooking by Windows because of a timeout.
    /// See https://stackoverflow.com/a/49965969
    /// </summary>
    internal class MouseHook : IDisposable
    {
        private const int WH_MOUSE_LL = 14;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MBUTTONUP = 0x0208;
        private const int WM_XBUTTONDOWN = 0x020B;
        private const int WM_XBUTTONUP = 0x020C;

        private readonly Settings settings;
        private readonly ILogger logger;

        // make sure we keep a reference so it's not garbage collected
        private LowLevelMouseProc? mouseProc;
        private IntPtr hookHandle = IntPtr.Zero;

        private readonly HashSet<int> observedMessages = new();
        Dictionary<MouseButtons, uint> previousUpTime = new() { {MouseButtons.Left , 0 }, {MouseButtons.Right , 0}, {MouseButtons.Middle , 0}, {MouseButtons.XButton1 , 0}, {MouseButtons.XButton2 , 0} };
        private uint ignoredClicks = 0;

        public MouseHook(Settings settings, ILogger logger)
        {
            this.settings = settings;
            SettingsChanged();
            settings.RegisterSettingsChangedListener(SettingsChanged);
            this.logger = logger;
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
                observedMessages.Add(WM_MBUTTONDOWN);
            }
            if (settings.X1Threshold >= 0 || settings.X2Threshold >= 0)
            {
                observedMessages.Add(WM_XBUTTONDOWN);
                observedMessages.Add(WM_XBUTTONUP);
            }
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
  
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && observedMessages.TryGetValue((int)wParam, out _))
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))!;
                bool buttonUp = false;
                bool buttonDown = false;
                MouseButtons button = MouseButtons.None;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        buttonDown = true;
                        button = MouseButtons.Left;
                        break;
                    case WM_LBUTTONUP:
                        buttonUp = true;
                        button = MouseButtons.Left;
                        break;
                    case WM_RBUTTONDOWN:
                        buttonDown = true;
                        button = MouseButtons.Right;
                        break;
                    case WM_RBUTTONUP:
                        buttonUp = true;
                        button = MouseButtons.Right;
                        break;
                    case WM_MBUTTONDOWN:
                        buttonDown = true;
                        button = MouseButtons.Middle;
                        break;
                    case WM_MBUTTONUP:
                        buttonUp = true;
                        button = MouseButtons.Middle;
                        break;
                    case WM_XBUTTONDOWN:
                        buttonDown = true;
                        button = GetXButton(hookStruct.mouseData);
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
                        bool ignore = timeDifference < settings.MinimumDoubleClickDelayMilliseconds;
                        if (ignore)
                        {
                            ignoredClicks++;
                            logger.Log($"{Resources.IgnoredDoubleClick}: {timeDifference} ms ({button} {ignoredClicks})");
                            previousUpTime[button] = 0;
                            return (IntPtr)1;
                        }
                        else
                        {
                            if (timeDifference < settings.WindowsDoubleClickTimeMilliseconds)
                            {
                                logger.Log($"{timeDifference} ms");
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

        private static MouseButtons GetXButton(uint mouseData)
        {
            const int MK_XBUTTON1_DOWN = 0x0020;
            const int MK_XBUTTON2_DOWN = 0x0040;
            const int XBUTTON1_UP = 0x0001;
            const int XBUTTON2_UP = 0x0002;

            ushort loword = unchecked((ushort)(ulong)mouseData);
            ushort hiword = unchecked((ushort)((ulong)mouseData >> 16));
            if ((loword & MK_XBUTTON1_DOWN) != 0 || (hiword & XBUTTON1_UP) != 0)
            {
                return MouseButtons.XButton1;
            }
            else if ((loword & MK_XBUTTON2_DOWN) != 0 || (hiword & XBUTTON2_UP) != 0)
            {
                return MouseButtons.XButton2;
            }
            return MouseButtons.None;
        }
   

        //private IntPtr HandleHookCallback(MSLLHOOKSTRUCT hookStruct, int nCode, IntPtr wParam, IntPtr lParam, int downParam, int upParam)
        //{
        //    // We take the elapsed time between the last mouse up and the current mouse down event.
        //    // If it's smaller than the minimal delay, we ignore the current mouse down event.
        //    if (wParam == (IntPtr)downParam)
        //    {
        //        long timeDifference = hookStruct.time - previousUpTime;
        //        bool ignore = timeDifference < settings.MinimumDoubleClickDelayMilliseconds;
        //        if (ignore)
        //        {
        //            ignoredClicks++;
        //            logger.Log($"{Resources.IgnoredDoubleClick}: {timeDifference} ms ({ignoredClicks})");
        //            previousUpTime = 0;
        //            return (IntPtr)1;
        //        }
        //        else
        //        {
        //            if (timeDifference < settings.WindowsDoubleClickTimeMilliseconds)
        //            {
        //                logger.Log($"{timeDifference} ms");
        //            }
        //        }
        //    }
        //    else if (wParam == (IntPtr)upParam)
        //    {
        //        previousUpTime = hookStruct.time;

        //    }
        //    return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        //}
        
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
}