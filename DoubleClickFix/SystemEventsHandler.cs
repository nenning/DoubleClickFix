using Microsoft.Win32;
using System.Diagnostics;

namespace DoubleClickFix;

// TODO: could translate.
internal class SystemEventsHandler : IDisposable
{
    private readonly MouseHook mouseHook;
    private readonly ILogger logger;

    public SystemEventsHandler(MouseHook mouseHook, ILogger logger, bool installExceptionHandlers)
    {
        this.mouseHook = mouseHook;
        this.logger = logger;

        try
        {
            if (installExceptionHandlers)
            {
                SetupExceptionHandlers(mouseHook, logger);
            }
            SystemEvents.SessionSwitch += OnSessionSwitch;
            SystemEvents.SessionEnding += OnSessionEnding;
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
        }
        catch (Exception ex)
        {
            logger.Log($"Failed to subscribe to system events: {ex.Message}");
        }
    }

    public void Dispose()
    {
        try
        {
            SystemEvents.SessionSwitch -= OnSessionSwitch;
            SystemEvents.SessionEnding -= OnSessionEnding;
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        }
        catch (Exception ex)
        {
            logger.Log($"Failed to unsubscribe from system events: {ex.Message}");
        }
    }

    private void OnSessionEnding(object sender, SessionEndingEventArgs e)
    {
        mouseHook.Uninstall();
    }

    private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        switch (e.Reason)
        {
            case SessionSwitchReason.SessionLock:
                mouseHook.Uninstall();
                break;
            case SessionSwitchReason.SessionUnlock:
                if (!mouseHook.Install())
                {
                    logger.Log("Failed to reinstall mouse hook after session unlock.");
                }
                break;
        }
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        switch (e.Mode)
        {
            case PowerModes.Suspend:
                mouseHook.Uninstall();
                break;
            case PowerModes.Resume:
                if (!mouseHook.Install())
                {
                    logger.Log("Failed to reinstall mouse hook after Windows resume."); 
                }
                break;
        }
    }
    private static void SetupExceptionHandlers(MouseHook mouseHook, ILogger logger)
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