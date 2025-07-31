using System.Diagnostics;
using DoubleClickFix.Properties;

namespace DoubleClickFix;
internal class Program
{
    private static bool IsRunningFromStore()
    {
        try
        {
            return Windows.ApplicationModel.Package.Current != null;
        }
        catch
        {
            return false; // standalone executable
        }
    }

    private static string GetVersion()
    {
        try
        {
            if (IsRunningFromStore())
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
            else
            {
                return typeof(Program).Assembly.GetName().Version?.ToString() ?? "";
            }
        }
        catch
        {
            return "";
        }
    }

    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using Logger logger = new();

        bool isRunningFromStore = IsRunningFromStore();
        if (isRunningFromStore)
        {
            // allow the store to restart the app after an update
            NativeMethods.RegisterApplicationRestart("-restart", 0);
        }

        ISettings settings = isRunningFromStore
            ? new StoreSettings(args, logger)
            : new StandaloneSettings(args, logger);

        // enforce single instance via mutex
        using Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
        if (settings.UseHook && !mutex.WaitOne(TimeSpan.Zero, true))
        {
            // an instance is already running – bring its UI to the front
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(InteractiveForm));
            var windowTitle = resources.GetString("$this.Text") ?? "Double-click Fix";
            IntPtr hWnd = NativeMethods.FindWindow(null, windowTitle);
            if (hWnd != IntPtr.Zero)
            {
                NativeMethods.PostMessage(hWnd, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                // Fallback if the window can't be found for some reason
                MessageBox.Show(Resources.AppAlreadyRunning, windowTitle);
            }
            return;
        }

        using MouseHook mouseHook = new(settings, logger, new NativeMethods());
        SetupExceptionHandlers(logger, mouseHook);

        try
        {
            IStartupRegistry startupRegistry = isRunningFromStore
                ? new StoreStartupRegistry(logger)
                : new StandaloneStartupRegistry(logger);

            if (settings.IsFirstAppStart)
            {
                startupRegistry.Register();
            }

            InteractiveForm form = new(startupRegistry, settings, logger, mouseHook.ProcessRawInput, GetVersion());
            if (!settings.IsInteractive)
            {
                // Minimize and hide the form so it does not flash on start
                form.ShowInTaskbar = false;
                form.WindowState = FormWindowState.Minimized;
                form.Hide();
            }

            // register for raw input before installing the hook
            mouseHook.RegisterForRawInput(form.Handle);
            if (!mouseHook.Install())
            {
                form.Text = "No mouse hook installed!";
                form.BackColor = Color.DarkRed;
                logger.Log($"{Resources.Error}: {Resources.HookNotInstalled}");
            }

            Application.Run(form);
        }
        finally
        {
            // release the mutex if we acquired it
            if (settings.UseHook)
            {
                try { mutex.ReleaseMutex(); } catch { }
            }
        }
    }
    private static void SetupExceptionHandlers(Logger logger, MouseHook mouseHook)
    {
        Application.ThreadException += (sender, e) =>
        {
            // Uninstall hook to clean up
            mouseHook.Uninstall();
            logger.Log($"[UI Exception] Please report this as a github issue: {e.Exception}");

            // Ask the user what to do
            var result = MessageBox.Show(
                "An unexpected error occurred: \n\n" +
                $"{e.Exception.Message}\n\n" +
                "Would you like to restart the application?\n\n" +
                "Yes = Restart   No = Exit   Cancel = Continue",
                "Double-click fix: Application Error",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);

            switch (result)
            {
                case DialogResult.Yes:
                    // Restart: launches a fresh copy, then exits this one
                    Application.Restart();
                    Environment.Exit(0);
                    break;

                case DialogResult.No:
                    // Exit without restarting
                    Application.Exit();
                    break;

                case DialogResult.Cancel:
                default:
                    // Continue running: re-install mouse hook and carry on
                    mouseHook.Install();
                    break;
            }
        };

        // Handle exceptions on any non-UI thread (thread‑pool, timers, etc.)
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            mouseHook.Uninstall();
            if (e.ExceptionObject is Exception ex)
            {
                try
                {
                    Debug.WriteLine($"[Domain Exception] {ex}");
                    logger.Log($"[Domain Exception] Please report this as a github issue: {ex}");
                }
                catch { }
            }
        };

        // Handle unobserved task exceptions as a last‑resort for async code
        TaskScheduler.UnobservedTaskException += (sender, e) => {
            logger.Log($"[Task Exception] Please report this as a github issue: {e.Exception}");
            e.SetObserved();     // prevent the exception escalation policy (which might crash)
        };
    }

}