using DoubleClickFix.Properties;
using System.Reflection;

namespace DoubleClickFix;

class Program
{

    private static bool IsRunningFromStore()
    {
        try
        {
            return Windows.ApplicationModel.Package.Current != null;
        }
        catch
        {
            return false; // Standalone executable
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
                return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
            }
        }
        catch
        {
            return "";
        }
    }

    [STAThread]
    static void Main(string[] args)
    {
        Logger logger = new();
        bool isRunningFromStore = IsRunningFromStore();

        ISettings settings = isRunningFromStore ? new StoreSettings(args, logger) : new StandaloneSettings(args, logger);

        using Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
        if (settings.UseHook && !mutex.WaitOne(TimeSpan.Zero, true))
        {
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
        try
        {
            IStartupRegistry startupRegistry = isRunningFromStore ? new StoreStartupRegistry(logger) : new StandaloneStartupRegistry(logger);
            if (settings.IsFirstAppStart)
            {
                startupRegistry.Register();
            }
            InteractiveForm form = new(startupRegistry, settings, logger, mouseHook.ProcessRawInput, GetVersion());
            if (settings.IsInteractive)
            {
                form.Visible = true;
            }
            mouseHook.RegisterForRawInput(form.Handle);
            if (!(mouseHook.Install()))
            {
                form.Text = "No mouse hook installed!";
                form.BackColor = Color.DarkRed;
                logger.Log($"{Resources.Error}: {Resources.HookNotInstalled}");
            }
            Application.Run();
        }
        finally
        {
            if (settings.UseHook)
            {
                try
                {
                    mutex.ReleaseMutex();
                }
                catch { }
            }
        }
    }
}