using DoubleClickFix.Properties;

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

    [STAThread]
    static void Main(string[] args)
    {
        Logger logger = new();
        bool isRunningFromStore = IsRunningFromStore();

        ISettings settings = isRunningFromStore ? new StoreSettings(args, logger) : new StandaloneSettings(args, logger);

        using Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
        if (settings.UseHook && !mutex.WaitOne(TimeSpan.Zero, true))
        {
            MessageBox.Show(Resources.AppAlreadyRunning, "Double-click Fix");
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
            InteractiveForm form = new(startupRegistry, settings, logger, mouseHook.ProcessRawInput);
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