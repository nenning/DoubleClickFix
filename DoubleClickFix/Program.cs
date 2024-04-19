using DoubleClickFix.Properties;
using System.Diagnostics;

namespace DoubleClickFix
{
    class Program
    {
        private static readonly Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
        private static readonly Logger logger = new();

        [STAThread]
        static void Main(string[] args)
        {
            var settings = new Settings(args, logger);
            if (settings.UseHook)
            {
                if (!mutex.WaitOne(TimeSpan.Zero, true))
                {
                    MessageBox.Show(Resources.AppAlreadyRunning, "Double-click fix");
                    return;
                }
            }
            MouseHook mouseHook = new(logger, settings);
            try
            {
                if (settings.UseHook)
                {
                    mouseHook.Install();
                }

                InteractiveForm form = new(new StartupRegistry(), logger, settings);
                if (settings.IsInteractive)
                {
                    form.Visible = true;
                }
                if (!settings.UseHook)
                {
                    form.Text = "No mouse hook installed!";
                }
                Application.Run();
            }
            finally
            {
                if (settings.UseHook)
                {
                    try
                    {
                        mouseHook.Uninstall();
                        mutex.ReleaseMutex();
                    } catch { }
                }
            }
        }
    }
}