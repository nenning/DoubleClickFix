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

    private static string GetVersion(bool isRunningFromStore)
    {
        try
        {
            if (isRunningFromStore)
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

    private static bool NotifyRunningInstance()
    {
        string processName = Process.GetCurrentProcess().ProcessName;
        var otherPids = Process.GetProcessesByName(processName)
            .Where(p => p.Id != Environment.ProcessId)
            .Select(p => (uint)p.Id)
            .ToHashSet();

        if (otherPids.Count == 0)
            return false;

        // Send WM_SHOWME to all top-level windows of the other process.
        // WinForms creates several internal windows alongside the form, so we
        // broadcast to all of them — only InteractiveForm.WndProc handles it.
        bool sent = false;
        NativeMethods.EnumWindows((hWnd, _) =>
        {
            NativeMethods.GetWindowThreadProcessId(hWnd, out uint pid);
            if (otherPids.Contains(pid))
            {
                NativeMethods.PostMessage(hWnd, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                sent = true;
            }
            return true; // continue enumeration
        }, IntPtr.Zero);
        return sent;
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

        Application.SetColorMode(settings.ColorMode switch
        {
            ColorMode.Dark  => SystemColorMode.Dark,
            ColorMode.Light => SystemColorMode.Classic,
            _               => SystemColorMode.System
        });

        // enforce single instance via mutex
        using Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
        if (settings.UseHook && !mutex.WaitOne(TimeSpan.Zero, true))
        {
            // an instance is already running – bring its UI to the front
            if (!NotifyRunningInstance())
            {
                // Fallback if the window can't be found for some reason
                MessageBox.Show(Resources.AppAlreadyRunning, "Double-click Fix");
            }
            return;
        }

        using MouseHook mouseHook = new(settings, logger, new NativeMethods());
        using SystemEventsHandler eventsHandler = new(mouseHook, logger, !isRunningFromStore);

        InteractiveForm? form = null;
        try
        {
            IStartupRegistry startupRegistry = isRunningFromStore
                ? new StoreStartupRegistry(logger)
                : new StandaloneStartupRegistry(logger);

            if (settings.IsFirstAppStart)
            {
                startupRegistry.Register();
            }

            form = new InteractiveForm(startupRegistry, settings, logger, mouseHook, GetVersion(isRunningFromStore));

            // register for raw input before installing the hook
            mouseHook.RegisterForRawInput(form.Handle);
            if (!mouseHook.Install())
            {
                form.Text = "No mouse hook installed!";
                form.BackColor = NativeMethods.IsDarkMode(settings.ColorMode) ? Color.Crimson : Color.DarkRed;
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

        // Restart after mutex is released so the new process can acquire it
        if (form?.RestartArgs != null)
        {
            string? exePath = Environment.ProcessPath;
            if (exePath != null)
                Process.Start(new ProcessStartInfo(exePath, form.RestartArgs) { UseShellExecute = false });
        }
    }

}