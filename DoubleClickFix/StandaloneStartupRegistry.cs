using Microsoft.Win32;

namespace DoubleClickFix;

internal class StandaloneStartupRegistry(ILogger Logger) : IStartupRegistry {
	private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
	private const string RegistryKey = @"DoubleClickFix";

	private readonly string registryValue = Environment.ProcessPath!;

	private bool isRegistered = false;

	public bool IsRegistered() {
		try {
			using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, writable: false);
			var value = key!.GetValue(RegistryKey, null);
			isRegistered = value != null && (string)value == registryValue;
			return isRegistered;
		} catch (Exception ex) {
			Logger.Log($"Error: {ex}");
			isRegistered = false;
			return false;
		}
	}

	public bool Register() {
		if (isRegistered) {
			return true;
		}
		try {
			using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
			key!.SetValue(RegistryKey, registryValue, RegistryValueKind.String);
		} catch (Exception ex) {
			Logger.Log($"Error: {ex}");
			return false;
		}
		isRegistered = true;
		return true;
	}
	public bool Unregister() {
		if (!isRegistered) {
			return true;
		}
		try {
			using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
			key!.DeleteValue(RegistryKey, false);
		} catch (Exception ex) {
			Logger.Log($"Error: {ex}");
			return false;
		}
		isRegistered = false;
		return true;
	}
}
