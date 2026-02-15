using DoubleClickFix.Properties;

using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Windows.Storage;

namespace DoubleClickFix;

internal class StoreSettings(string[] args, ILogger logger) : SettingsBase(args, logger), ISettings
{
    private readonly ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

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
        SaveSetting(WheelThreshold);
        SaveSetting(MinDelay);
        SaveSetting(IgnoredDevice);
        SaveSetting(DragStartTimeMilliseconds);
        SaveSetting(DragStopTimeMilliseconds);
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
        wheelThreshold = LoadSetting(WheelThreshold);
        minDelay = LoadSetting(MinDelay);
        ignoredDevice = LoadSetting(IgnoredDevice);
        dragStartTimeMilliseconds = LoadSetting(DragStartTimeMilliseconds);
        dragStopTimeMilliseconds = LoadSetting(DragStopTimeMilliseconds);
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