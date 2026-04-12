using System.Diagnostics;
using Microsoft.Win32;

namespace DoubleClickFix;

// TODO: could translate.
internal class SystemEventsHandler : IDisposable {
	private readonly MouseHook mouseHook;
	private readonly ILogger logger;

	public SystemEventsHandler(MouseHook mouseHook, ILogger logger, bool isRunningFromStore) {
		this.mouseHook = mouseHook;
		this.logger = logger;

		try {
			SetupExceptionHandlers(mouseHook, logger, isRunningFromStore);
			SystemEvents.SessionSwitch += OnSessionSwitch;
			SystemEvents.SessionEnding += OnSessionEnding;
			SystemEvents.PowerModeChanged += OnPowerModeChanged;
		} catch (Exception ex) {
			logger.Log($"Failed to subscribe to system events: {ex.Message}");
		}
	}

	public void Dispose() {
		try {
			SystemEvents.SessionSwitch -= OnSessionSwitch;
			SystemEvents.SessionEnding -= OnSessionEnding;
			SystemEvents.PowerModeChanged -= OnPowerModeChanged;
		} catch (Exception ex) {
			logger.Log($"Failed to unsubscribe from system events: {ex.Message}");
		}
	}

	private void OnSessionEnding(object sender, SessionEndingEventArgs e) => mouseHook.Uninstall();

	private void OnSessionSwitch(object sender, SessionSwitchEventArgs e) {
		switch (e.Reason) {
			case SessionSwitchReason.SessionLock:
				mouseHook.Uninstall();
				break;
			case SessionSwitchReason.SessionUnlock:
				if (!mouseHook.Install()) {
					logger.Log("Failed to reinstall mouse hook after session unlock.");
				}
				break;
			case SessionSwitchReason.RemoteConnect:
			case SessionSwitchReason.RemoteDisconnect:
			case SessionSwitchReason.ConsoleConnect:
			case SessionSwitchReason.ConsoleDisconnect:
				mouseHook.RefreshRemoteSessionState();
				break;
		}
	}

	private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e) {
		switch (e.Mode) {
			case PowerModes.Suspend:
				mouseHook.Uninstall();
				break;
			case PowerModes.Resume:
				if (!mouseHook.Install()) {
					logger.Log("Failed to reinstall mouse hook after Windows resume.");
				}
				break;
		}
	}
	private static void SetupExceptionHandlers(MouseHook mouseHook, ILogger logger, bool isRunningFromStore) {
		if (!isRunningFromStore) {
			// Standalone: show a dialog so the user can restart, exit, or continue
			Application.ThreadException += (sender, e) => {
				mouseHook.Uninstall();
				logger.Log($"[UI Exception] Please report this as a github issue: {e.Exception}");

				var result = MessageBox.Show(
					"An unexpected error occurred: \n\n" +
					$"{e.Exception.Message}\n\n" +
					"Would you like to restart the application?\n\n" +
					"Yes = Restart   No = Exit   Cancel = Continue",
					"Double-click fix: Application Error",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Error,
					MessageBoxDefaultButton.Button1);

				switch (result) {
					case DialogResult.Yes:
						Application.Restart();
						Environment.Exit(0);
						break;

					case DialogResult.No:
						Application.Exit();
						break;

					case DialogResult.Cancel:
					default:
						mouseHook.Install();
						break;
				}
			};
		}
		// Store builds use SetUnhandledExceptionMode(ThrowException) in Program.cs,
		// so UI thread exceptions route to AppDomain.UnhandledException → WER.

		// Handle exceptions on any non-UI thread (thread‑pool, timers, etc.)
		AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
			mouseHook.Uninstall();
			if (e.ExceptionObject is Exception ex) {
				try {
					Debug.WriteLine($"[Domain Exception] {ex}");
					logger.Log($"[Domain Exception] Please report this as a github issue: {ex}");
					WriteCrashLog(ex);
				} catch { }
			}
			// Let the crash propagate to WER — do not call Environment.Exit() or FailFast()
		};

		// Handle unobserved task exceptions as a last‑resort for async code
		TaskScheduler.UnobservedTaskException += (sender, e) => {
			try {
				logger.Log($"[Task Exception] Please report this as a github issue: {e.Exception}");
				WriteCrashLog(e.Exception);
			} catch { }

			if (!isRunningFromStore) {
				e.SetObserved(); // standalone: prevent crash, keep running
			}
			// Store: don't call SetObserved() — let the exception escalate to WER
		};
	}

	private static void WriteCrashLog(Exception ex) {
		try {
			string folder = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"DoubleClickFix");
			Directory.CreateDirectory(folder);
			string path = Path.Combine(folder, "crashlog.txt");
			string entry = $"[{DateTime.UtcNow:O}] {ex}\n\n";
			File.AppendAllText(path, entry);
		} catch { }
	}
}
