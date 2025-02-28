using DoubleClickFix.Properties;

using System.Runtime.CompilerServices;
using Windows.Storage;

namespace DoubleClickFix;

internal class StoreSettings : SettingsBase, ISettings
{
    private readonly ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

    public StoreSettings(string[] args, ILogger logger): base (args, logger)
    {
    }

    private void SaveSetting(int currentValue, [CallerArgumentExpression(nameof(currentValue))] string name = "")
    {
        settings.Values[name] = currentValue;
    }

    public override void Save()
    {
        SaveSetting(LeftThreshold);
        SaveSetting(RightThreshold);
        SaveSetting(MiddleThreshold);
        SaveSetting(X1Threshold);
        SaveSetting(X2Threshold);
        SaveSetting(MinDelay);
        SaveSetting(IgnoredDevice);
        logger.Log(Resources.SettingsSaved);
    }

    protected override bool SettingsExist()
    {
        return settings.Values.ContainsKey(nameof(LeftThreshold));
    }
    public override void Load()
    {
        leftThreshold = LoadSetting(LeftThreshold);
        rightThreshold = LoadSetting(RightThreshold);
        middleThreshold = LoadSetting(MiddleThreshold);
        x1Threshold = LoadSetting(X1Threshold);
        x2Threshold = LoadSetting(X2Threshold);
        minDelay = LoadSetting(MinDelay);
        ignoredDevice = LoadSetting(IgnoredDevice);
        FireSettingsChanged();
    }
    private int LoadSetting(int defaultValue, [CallerArgumentExpression(nameof(defaultValue))] string name = "")
    {
        object? value = settings.Values[name];
        if (value is int intValue)
        {
            return intValue;
        }
        return defaultValue;
    }
}