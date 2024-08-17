using DoubleClickFix.Properties;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace DoubleClickFix;

public class Settings
{
    private readonly int windowsDoubleClickTimeMilliseconds = GetWindowsMaximumDoubleClickTime();
    private readonly ILogger logger;
    private Action settingsChanged;

    public Settings(string[] args, ILogger logger)
    {
        this.logger = logger;
        UseHook = !Debugger.IsAttached || args.Length == 0 || !args.Contains("-nohook");
        IsInteractive = Debugger.IsAttached || args.Length > 0 && (args.Contains("-interactive") || args.Contains("-i"));
        ApplyLanguageOverride();
        Load();
        settingsChanged += this.OnSettingsChanged;
    }

    public void RegisterSettingsChangedListener(Action listener)
    {
        this.settingsChanged += listener;
    }
    private void OnSettingsChanged() {  }

    private int minDelay = -1;
    public int MinDelay
    {
        get { return minDelay; }
        set
        {
            if (value != minDelay)
            {
                minDelay = value;
                FireSettingsChanged();
            }
        }
    }

    private int ignoredDevice = 0;
    public int IgnoredDevice
    {
        get { return ignoredDevice; }
        set
        {
            if (value != ignoredDevice)
            {
                ignoredDevice = value;
                FireSettingsChanged();
            }
        }
    }

    private int leftThreshold = 50;
    public int LeftThreshold
    {
        get { return leftThreshold; }
        set
        {
            if (value != leftThreshold)
            {
                leftThreshold = value;
                FireSettingsChanged();
            }
        }
    }
    private int rightThreshold = -1;
    public int RightThreshold
    {
        get { return rightThreshold; }
        set
        {
            if (value != rightThreshold)
            {
                rightThreshold = value;
                FireSettingsChanged();
            }
        }
    }
    private int middleThreshold = -1;
    public int MiddleThreshold
    {
        get { return middleThreshold; }
        set
        {
            if (value != middleThreshold)
            {
                middleThreshold = value;
                FireSettingsChanged();
            }
        }
    }

    private int x1Threshold = -1;
    public int X1Threshold
    {
        get { return x1Threshold; }
        set
        {
            if (value != x1Threshold)
            {
                x1Threshold = value;
                FireSettingsChanged();
            }
        }
    }
    private int x2Threshold = -1;
    public int X2Threshold
    {
        get { return x2Threshold; }
        set
        {
            if (value != x2Threshold)
            {
                x2Threshold = value;
                FireSettingsChanged();
            }
        }
    }
    private void FireSettingsChanged()
    {
        settingsChanged?.Invoke();
    }

    /// <summary>
    /// When debugging with the mouse hook and entering a breakpoint, the mouse can lag significantly. Use the -nohook command line argument in this case.
    /// </summary>
    public bool UseHook { get; private set; }
    public bool IsInteractive { get; private set; }

    public void Save()
    {
        try
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configuration.AppSettings.Settings;
            SaveSetting(settings, leftThreshold);
            SaveSetting(settings, rightThreshold);
            SaveSetting(settings, middleThreshold);
            SaveSetting(settings, x1Threshold);
            SaveSetting(settings, x2Threshold);
            SaveSetting(settings, minDelay);
            SaveSetting(settings, ignoredDevice);
            configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            logger.Log(Resources.SettingsSaved);
        }
        catch (Exception ex)
        {
            logger.Log($"{Resources.Error}: " + ex.ToString());
        }
    }

    private static void SaveSetting(KeyValueConfigurationCollection settings, int value, [CallerArgumentExpression(nameof(value))] string key = "")
    {
        if (settings.AllKeys.Contains(key))
        {
            settings[key].Value = value.ToString();
        }
        else
        {
            settings.Add(key, value.ToString());
        }
    }
    public int WindowsDoubleClickTimeMilliseconds
    {
        get => windowsDoubleClickTimeMilliseconds;
    }

    private static int GetWindowsMaximumDoubleClickTime()
    {
        int doubleClickTime = NativeMethods.GetDoubleClickTime();
        if (doubleClickTime == 0)
        {
            doubleClickTime = 600;
        }
        return doubleClickTime;
    }

    private void Load()
    {
        var settings = ConfigurationManager.AppSettings!;
        leftThreshold = LoadSetting(settings, leftThreshold);
        rightThreshold = LoadSetting(settings, rightThreshold);
        middleThreshold = LoadSetting(settings, middleThreshold);
        x1Threshold = LoadSetting(settings, x1Threshold);
        x2Threshold = LoadSetting(settings, x2Threshold);
        minDelay = LoadSetting(settings, minDelay);
        ignoredDevice = LoadSetting(settings, ignoredDevice);
    }
    private static int LoadSetting(NameValueCollection settings, int currentValue, [CallerArgumentExpression(nameof(currentValue))] string key = "")
    {
        string? value = settings[key];
        if (value == null || !int.TryParse(value, out int newValue))
        {
            return currentValue;
        }
        return newValue;
    }

    private static void ApplyLanguageOverride()
    {
        try
        {
            string? value = ConfigurationManager.AppSettings["languageOverride"];
            if (!string.IsNullOrWhiteSpace(value))
            {
                CultureInfo culture = new(value);
                Application.CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture.Name);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture.Name);
            }
        }
        catch
        {
        }
    }
}
