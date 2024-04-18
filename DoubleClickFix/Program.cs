using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix
{
    class Program
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private static LowLevelMouseProc mouseProc = HookCallback;
        private static IntPtr hookID = IntPtr.Zero;
        private static uint lastClickTime;
        private static int windowsDoubleClickTimeMilliseconds;
        private static int minimumDelayMilliseconds;
        private static Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
        private static Action<string> log = text => Debug.WriteLine(text);

        [STAThread]
        static void Main(string[] args)
        {
            EnsureSingleInstance();
            minimumDelayMilliseconds = GetMinimumDelay();
            windowsDoubleClickTimeMilliseconds = GetMinimumDoubleClickTime();
            try
            {
                hookID = SetHook(mouseProc);

                InteractiveForm form = new(new Startup());
                log = text =>
                {
                    if (form.Visible)
                    {
                        form.Log(text);
                    }
                };
                form.MinDelay = minimumDelayMilliseconds;
                form.OnSave += UpdateAppSettings;
                if (Debugger.IsAttached || args.Length > 0 && (args.Contains("-interactive") || args.Contains("-i")))
                {
                    form.Visible = true;
                }

                Application.Run();
            }
            finally
            {
                UnhookWindowsHookEx(hookID);
                mutex.ReleaseMutex();
            }
        }

        private static void EnsureSingleInstance()
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Debug.WriteLine("Another instance of the application is already running.");
                Application.Exit();
            }
        }

        static int GetMinimumDoubleClickTime()
        {
            int doubleClickTime = GetDoubleClickTime();
            if (doubleClickTime == 0)
            {
                doubleClickTime = 600;
            }
            return doubleClickTime;
        }

        static int GetMinimumDelay()
        {
            string? value = ConfigurationManager.AppSettings["MinimumDelayMilliseconds"];
            int minimumDelay;
            if (value == null || !int.TryParse(value, out minimumDelay))
            {
                minimumDelay = 94;
            }
            return minimumDelay;
        }
        static void UpdateAppSettings(int value)
        {
            if (value <= 0 || value >= windowsDoubleClickTimeMilliseconds)
            {
                log($"Invalid value - not saved. Valid range: [0..{windowsDoubleClickTimeMilliseconds}]");
                return;
            }
            minimumDelayMilliseconds = value;
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["MinimumDelayMilliseconds"].Value = value.ToString();
            configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            log("settings saved: " + value + " ms");
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (ProcessModule curModule = Process.GetCurrentProcess().MainModule!)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                MSLLHOOKSTRUCT? hookStruct = Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)) as MSLLHOOKSTRUCT?;
                if (hookStruct != null && IsDoubleClick(hookStruct.Value))
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

        private static bool IsDoubleClick(MSLLHOOKSTRUCT hookStruct)
        {
            long timeDifference = hookStruct.time - lastClickTime;
            bool ignore = timeDifference < minimumDelayMilliseconds;
            if (timeDifference < windowsDoubleClickTimeMilliseconds)
            {
                log(timeDifference.ToString());
            }
            if (ignore)
            {
                log("ignored double click: " + timeDifference + " ms");
            }
            return ignore;
        }

    }
}