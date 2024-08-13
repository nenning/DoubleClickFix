using DoubleClickFix.Properties;
using System.Security.Principal;

namespace DoubleClickFix;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Logger logger = new();
        Settings settings = new(args, logger);
        using Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
        if (settings.UseHook && !mutex.WaitOne(TimeSpan.Zero, true))
        {
            MessageBox.Show(Resources.AppAlreadyRunning, "Double-click fix"); // TODO translate
            return;
        }

        using MouseHook mouseHook = new(settings, logger);
        try
        {
            InteractiveForm form = new(new StartupRegistry(), settings, logger, mouseHook.ProcessRawInput);
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