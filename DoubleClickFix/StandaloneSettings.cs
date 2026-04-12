using System.Linq;
using System.Runtime.CompilerServices;
using DoubleClickFix.Properties;
using Microsoft.Win32;

namespace DoubleClickFix;

internal class StandaloneSettings(string[] args, ILogger logger) : SettingsBase(args, logger) {
	private const string RegistryKeyPath = @"SOFTWARE\DoubleClickFix\v1";

	public override void Save() {
		try {
			using var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath, true);
			if (key != null) {
				SaveSetting(key, LeftThreshold);
				SaveSetting(key, RightThreshold);
				SaveSetting(key, MiddleThreshold);
				SaveSetting(key, X1Threshold);
				SaveSetting(key, X2Threshold);
				SaveSetting(key, WheelThreshold);
				SaveSetting(key, MinDelay);
				SaveIgnoredDevices(key, ignoredDevicePaths);
				SaveSetting(key, DragStartTimeMilliseconds);
				SaveSetting(key, DragStopTimeMilliseconds);
				SaveSetting(key, remoteDesktopDetection);
				key.SetValue("language", language, RegistryValueKind.String);
				key.SetValue("colorMode", colorMode.ToString(), RegistryValueKind.String);
				logger.Log(Resources.SettingsSaved);
			} else {
				logger.Log("Failed to create or write registry key.");
			}
		} catch (Exception ex) {
			logger.Log($"{Resources.Error}: " + ex.ToString());
		}
	}

	private static void SaveSetting(RegistryKey key, int currentValue, [CallerArgumentExpression(nameof(currentValue))] string name = "") => key.SetValue(name, currentValue, RegistryValueKind.DWord);

	private static void SaveIgnoredDevices(RegistryKey key, HashSet<string> paths) => key.SetValue("IgnoredDevicePaths", paths.ToArray(), RegistryValueKind.MultiString);

	public override bool Load() {
		using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
		if (key == null) {
			return false;
		}

		LeftThreshold = LoadSetting(key, LeftThreshold);
		RightThreshold = LoadSetting(key, RightThreshold);
		MiddleThreshold = LoadSetting(key, MiddleThreshold);
		X1Threshold = LoadSetting(key, X1Threshold);
		X2Threshold = LoadSetting(key, X2Threshold);
		WheelThreshold = LoadSetting(key, WheelThreshold);
		MinDelay = LoadSetting(key, MinDelay);
		if (key.GetValue("IgnoredDevicePaths") is string[] paths) {
			ignoredDevicePaths = [.. paths.Where(p => !string.IsNullOrEmpty(p))];
		}

		DragStartTimeMilliseconds = LoadSetting(key, DragStartTimeMilliseconds);
		DragStopTimeMilliseconds = LoadSetting(key, DragStopTimeMilliseconds);
		remoteDesktopDetection = LoadSetting(key, remoteDesktopDetection);
		if (key.GetValue("language") is string lang && !string.IsNullOrWhiteSpace(lang)) {
			language = lang;
		}

		if (key.GetValue("colorMode") is string cm && Enum.TryParse<ColorMode>(cm, true, out var parsedMode)) {
			colorMode = parsedMode;
		}

		return true;
	}

	private static int LoadSetting(RegistryKey key, int defaultValue, [CallerArgumentExpression(nameof(defaultValue))] string name = "") {
		object? value = key.GetValue(name, defaultValue);
		if (value is int intValue) {
			return intValue;
		}

		return defaultValue;
	}
}
