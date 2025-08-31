using DoubleClickFix.Properties;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace DoubleClickFix;

internal class StandaloneSettings(string[] args, ILogger logger) : SettingsBase(args, logger)
{
    private const string RegistryKeyPath = @"SOFTWARE\DoubleClickFix\v1";

    public override void Save()
    {
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath, true);
            if (key != null)
            {
                SaveSetting(key, LeftThreshold);
                SaveSetting(key, RightThreshold);
                SaveSetting(key, MiddleThreshold);
                SaveSetting(key, X1Threshold);
                SaveSetting(key, X2Threshold);
                SaveSetting(key, WheelThreshold);
                SaveSetting(key, MinDelay);
                SaveSetting(key, IgnoredDevice);
                SaveSetting(key, DragStartTimeMilliseconds);
                SaveSetting(key, DragStopTimeMilliseconds);
                logger.Log(Resources.SettingsSaved);
            } else { 
                logger.Log("Failed to create or write registry key."); 
            }
        }
        catch (Exception ex)
        {
            logger.Log($"{Resources.Error}: " + ex.ToString());
        }
    }

    private static void SaveSetting(RegistryKey key, int currentValue, [CallerArgumentExpression(nameof(currentValue))] string name = "")
    {
        key.SetValue(name, currentValue, RegistryValueKind.DWord);
    }

    protected override bool SettingsExist()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
            return key != null;
        }
        catch
        {
            return false;
        }
    }

    public override void Load()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
        if (key != null)
        {
            LeftThreshold = LoadSetting(key, LeftThreshold);
            RightThreshold = LoadSetting(key, RightThreshold);
            MiddleThreshold = LoadSetting(key, MiddleThreshold);
            X1Threshold = LoadSetting(key, X1Threshold);
            X2Threshold = LoadSetting(key, X2Threshold);
            WheelThreshold = LoadSetting(key, WheelThreshold);
            MinDelay = LoadSetting(key, MinDelay);
            IgnoredDevice = LoadSetting(key, IgnoredDevice);
            DragStartTimeMilliseconds = LoadSetting(key, DragStartTimeMilliseconds);
            DragStopTimeMilliseconds = LoadSetting(key, DragStopTimeMilliseconds);
        }
    }

    private static int LoadSetting(RegistryKey key, int defaultValue, [CallerArgumentExpression(nameof(defaultValue))] string name = "")
    {
        object? value = key.GetValue(name, defaultValue);
        if (value is int intValue)
        {
            return intValue;
        }
        return defaultValue;
    }
}
