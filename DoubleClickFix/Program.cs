using System.Diagnostics;
using DoubleClickFix.Properties;
using Microsoft.Win32;

namespace DoubleClickFix;

internal class Program {
	private static bool IsRunningFromStore() {
		try {
			return Windows.ApplicationModel.Package.Current != null;
		} catch {
			return false; // standalone executable
		}
	}

	private static string GetVersion(bool isRunningFromStore) {
		try {
			if (isRunningFromStore) {
				var version = Windows.ApplicationModel.Package.Current.Id.Version;
				return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
			} else {
				return typeof(Program).Assembly.GetName().Version?.ToString() ?? "";
			}
		} catch {
			return "";
		}
	}

	private static bool NotifyRunningInstance() {
		string processName = Process.GetCurrentProcess().ProcessName;
		HashSet<uint> otherPids = [.. Process.GetProcessesByName(processName)
			.Where(p => p.Id != Environment.ProcessId)
			.Select(p => (uint)p.Id)];

		if (otherPids.Count == 0) {
			return false;
		}

		// Grant each running instance the right to bring itself to the foreground.
		// Without this, Windows just flashes the taskbar button instead.
		foreach (uint pid in otherPids) {
			NativeMethods.AllowSetForegroundWindow((int)pid);
		}

		// Send WM_SHOWME to all top-level windows of the other process.
		// WinForms creates several internal windows alongside the form, so we
		// broadcast to all of them — only InteractiveForm.WndProc handles it.
		bool sent = false;
		NativeMethods.EnumWindows((hWnd, _) => {
			if (NativeMethods.GetWindowThreadProcessId(hWnd, out uint pid) == 0) {
				return true; // skip this window on failure
			}

			if (otherPids.Contains(pid)) {
				NativeMethods.PostMessage(hWnd, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
				sent = true;
			}
			return true; // continue enumeration
		}, IntPtr.Zero);
		return sent;
	}

	[STAThread]
	private static void Main(string[] args) {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		using Logger logger = new();

		bool isRunningFromStore = IsRunningFromStore();
		if (isRunningFromStore) {
			// allow the store to restart the app after an update
			_ = NativeMethods.RegisterApplicationRestart("-restart", 0);
		}

		ISettings settings = isRunningFromStore
			? new StoreSettings(args, logger)
			: new StandaloneSettings(args, logger);

		Application.SetColorMode(settings.ColorMode switch {
			ColorMode.Dark => SystemColorMode.Dark,
			ColorMode.Light => SystemColorMode.Classic,
			_ => SystemColorMode.System
		});

		// enforce single instance via mutex
		using Mutex mutex = new(true, "{F8049D9C-AD6B-4158-92A3-E537355EF536}");
		if (settings.UseHook && !mutex.WaitOne(TimeSpan.Zero, true)) {
			// an instance is already running – bring its UI to the front
			if (!NotifyRunningInstance()) {
				// Fallback if the window can't be found for some reason
				MessageBox.Show(Resources.AppAlreadyRunning, "Double-click Fix");
			}
			return;
		}

		// Elevate process priority so the hook callback gets scheduled promptly
		// during boot when many startup apps compete for CPU.
		try {
			using Process process = Process.GetCurrentProcess();
			process.PriorityClass = ProcessPriorityClass.AboveNormal;
		} catch { }

		using MouseHook mouseHook = new(settings, logger, new NativeMethods());
		using SystemEventsHandler eventsHandler = new(mouseHook, logger, isRunningFromStore);

		if (isRunningFromStore) {
			// Let UI thread exceptions propagate to AppDomain.UnhandledException → WER
			// so Partner Center gets real stack traces instead of "Unknown" failures
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
		}

		// Install the hook as early as possible using a lightweight message-only window
		// for raw input, deferring the heavy InteractiveForm construction until after
		// the message pump starts.
		using RawInputWindow rawInputWindow = new(mouseHook);
		mouseHook.RegisterForRawInput(rawInputWindow.Handle);
		bool hookInstalled = mouseHook.Install();
		if (!hookInstalled) {
			logger.Log($"{Resources.Error}: {Resources.HookNotInstalled}");
		}

		InteractiveForm? form = null;
		ApplicationContext appContext = new();
		try {
			Application.Idle += OnFirstIdle;

			void OnFirstIdle(object? sender, EventArgs e) {
				Application.Idle -= OnFirstIdle;

				IStartupRegistry startupRegistry = isRunningFromStore
					? new StoreStartupRegistry(logger)
					: new StandaloneStartupRegistry(logger);

				if (settings.IsFirstAppStart) {
					startupRegistry.Register();
				}

				form = new(startupRegistry, settings, logger, mouseHook, GetVersion(isRunningFromStore), isRunningFromStore);

				if (!hookInstalled) {
					form.Text = "No mouse hook installed!";
					form.BackColor = NativeMethods.IsDarkMode(settings.ColorMode) ? Color.Crimson : Color.DarkRed;
				}

				// Setting MainForm makes ApplicationContext exit when the form closes,
				// and triggers Show() which creates the handle (SetVisibleCore suppresses
				// the actual show when not interactive).
				appContext.MainForm = form;
				form.Show();

				rawInputWindow.SetShowMeHandler(() => form.BeginInvoke(() => form.ShowFromTray()));
			}

			Application.Run(appContext);
		} finally {
			// release the mutex if we acquired it
			if (settings.UseHook) {
				try { mutex.ReleaseMutex(); } catch { }
			}
		}

		// Restart after mutex is released so the new process can acquire it
		if (form?.RestartArgs != null) {
			string? exePath = Environment.ProcessPath;
			if (exePath != null) {
				Process.Start(new ProcessStartInfo(exePath, form.RestartArgs) { UseShellExecute = false });
			}
		}
	}

}
