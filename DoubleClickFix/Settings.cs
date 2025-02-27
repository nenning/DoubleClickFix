using DoubleClickFix.Properties;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace DoubleClickFix;

/// <summary>
/// Delay in milliseconds per mouse key.Use -1 to disable double click fix for a key or remove the corresponding setting. 
/// IgnoredDevice: Used to ignore touch screen or touch pad double clicks. Usually the id of such a device is 0. But it can be modified here if needed.
/// MinDelay: Can be used to define a minimum delay of 0. Workaround for touch screen or touch pad recognition based on device id not working. To deactivate use -1.
/// </summary>
internal class Settings : ISettings
{
    private const string RegistryKeyPath = @"SOFTWARE\DoubleClickFix\v1";

    private readonly int windowsDoubleClickTimeMilliseconds = GetWindowsMaximumDoubleClickTime();
    private readonly ILogger logger;
    private Action settingsChanged;

    public Settings(string[] args, ILogger logger)
    {
        this.logger = logger;
        UseHook = !Debugger.IsAttached || args.Length == 0 || !args.Contains("-nohook");
        IsFirstAppStart = !SettingsExist();
        IsInteractive = Debugger.IsAttached || IsFirstAppStart || args.Length > 0 && (args.Contains("-interactive") || args.Contains("-i"));
        ApplyLanguageOverride();
        if (IsFirstAppStart)
        {
            Save();
        }
        Load();
        settingsChanged += this.OnSettingsChanged;
    }
    public bool IsFirstAppStart { get; private init; }

    public void RegisterSettingsChangedListener(Action listener)
    {
        this.settingsChanged += listener;
    }
    private void OnSettingsChanged() { }

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
            using var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath, true);
            if (key != null)
            {
                SaveSetting(key, LeftThreshold);
                SaveSetting(key, RightThreshold);
                SaveSetting(key, MiddleThreshold);
                SaveSetting(key, X1Threshold);
                SaveSetting(key, X2Threshold);
                SaveSetting(key, MinDelay);
                SaveSetting(key, IgnoredDevice);
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

    private bool SettingsExist()
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
    private void Load()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
        if (key != null)
        {
            LeftThreshold = LoadSetting(key, LeftThreshold);
            RightThreshold = LoadSetting(key, RightThreshold);
            MiddleThreshold = LoadSetting(key, MiddleThreshold);
            X1Threshold = LoadSetting(key, X1Threshold);
            X2Threshold = LoadSetting(key, X2Threshold);
            MinDelay = LoadSetting(key, MinDelay);
            IgnoredDevice = LoadSetting(key, IgnoredDevice);
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
