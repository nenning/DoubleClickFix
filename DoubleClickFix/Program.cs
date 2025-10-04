using System.Diagnostics;
using DoubleClickFix.Properties;
using Microsoft.Win32;

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
            _ = NativeMethods.RegisterApplicationRestart("-restart", 0);
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
        using SystemEventsHandler eventsHandler = new(mouseHook, logger, !isRunningFromStore);

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

            InputCapabilities capabilities = InputCapabilities.Detect();
            logger.Log($"Input capabilities: {capabilities.Description}");
            if (capabilities.HasTouchscreen || capabilities.HasPrecisionTouchpad)
            {
                // register for raw input before installing the hook
                mouseHook.RegisterForRawInput(form.Handle);
            }

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

}