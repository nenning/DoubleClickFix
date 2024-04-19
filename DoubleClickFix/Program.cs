using DoubleClickFix;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static DoubleClickFix.NativeMethods;

namespace DoubleClickFix
{
}
    class Program
    {
        private static readonly Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
        private static readonly Logger logger = new();
        private static readonly Settings settings = new(logger);
        private static readonly MouseHook mouseHook = new(logger, settings);

        [STAThread]
        static void Main(string[] args)
        {
            EnsureSingleInstance();
            try
            {
                mouseHook.Install();

                InteractiveForm form = new(new StartupRegistry(), logger, settings);
                if (Debugger.IsAttached || args.Length > 0 && (args.Contains("-interactive") || args.Contains("-i")))
                {
                    form.Visible = true;
                }

                Application.Run();
            }
            finally
            {
                mouseHook.Uninstall();
                mutex.ReleaseMutex();
            }
        }

        private static void EnsureSingleInstance()
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("Another instance of the application is already running.","DoubleClickFix");
                Application.Exit();
            }
        }

}