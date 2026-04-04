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
        var composite = new Windows.Storage.ApplicationDataCompositeValue();
        int i = 0;
        foreach (var path in ignoredDevicePaths)
            composite[$"p{i++}"] = path;
        settings.Values["IgnoredDevicePaths"] = composite;
        SaveSetting(DragStartTimeMilliseconds);
        SaveSetting(DragStopTimeMilliseconds);
        SaveSetting(remoteDesktopDetection);
        settings.Values["language"] = language;
        settings.Values["colorMode"] = colorMode.ToString();
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
        if (settings.Values["IgnoredDevicePaths"] is Windows.Storage.ApplicationDataCompositeValue c)
            foreach (var kv in c)
                if (kv.Value is string s && !string.IsNullOrEmpty(s))
                    ignoredDevicePaths.Add(s);
        dragStartTimeMilliseconds = LoadSetting(DragStartTimeMilliseconds);
        dragStopTimeMilliseconds = LoadSetting(DragStopTimeMilliseconds);
        remoteDesktopDetection = LoadSetting(remoteDesktopDetection);
        if (settings.Values["language"] is string lang && !string.IsNullOrWhiteSpace(lang))
            language = lang;
        if (settings.Values["colorMode"] is string cm && Enum.TryParse<ColorMode>(cm, true, out var parsedMode))
            colorMode = parsedMode;
        FireSettingsChanged();
    }

    protected override string LoadLanguageSetting()
    {
        try
        {
            return ApplicationData.Current.LocalSettings.Values["language"] as string ?? "";
        }
        catch { return ""; }
    }

    private int LoadSetting(int defaultValue, [CallerArgumentExpression(nameof(defaultValue))] string name = "")
    {
        object? value = settings.Values[name];
        if (value is int intValue) return intValue;
        return defaultValue;
    }
}