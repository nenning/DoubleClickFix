using DoubleClickFix.Properties;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security;
using System.ComponentModel.DataAnnotations;
using System.Collections.Specialized;

namespace DoubleClickFix
{
    public class Settings
    {
        private readonly int windowsDoubleClickTimeMilliseconds = GetWindowsMaximumDoubleClickTime();
        private readonly ILogger logger;
        private int minimumDelayMilliseconds = 50;
        private Action settingsChanged;

        public Settings(string[] args, ILogger logger)
        {
            this.logger = logger;
            UseHook = !Debugger.IsAttached || args.Length == 0 || !args.Contains("-nohook");
            IsInteractive = Debugger.IsAttached || args.Length > 0 && (args.Contains("-interactive") || args.Contains("-i"));
            ApplyLanguageOverride();
            ReadAppSettings();
            settingsChanged += this.OnSettingsChanged;
        }

        public void RegisterSettingsChangedListener(Action listener)
        {
            this.settingsChanged += listener;
        }
        private void OnSettingsChanged() {  }

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
            if (settingsChanged != null)
            {
                settingsChanged();
            }
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
                // TODO check for existence) settings.AllKeys & settings.Add?
                settings[nameof(minimumDelayMilliseconds)].Value = minimumDelayMilliseconds.ToString();
                settings[nameof(leftThreshold)].Value = leftThreshold.ToString();
                settings[nameof(rightThreshold)].Value = rightThreshold.ToString();
                settings[nameof(middleThreshold)].Value = middleThreshold.ToString();
                settings[nameof(x1Threshold)].Value = x1Threshold.ToString();
                settings[nameof(x2Threshold)].Value = x2Threshold.ToString();
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                logger.Log(Resources.SettingsSaved);
            } catch (Exception ex)
            {
                logger.Log($"{Resources.Error}: " + ex.ToString());
            }
        }

        public int MinimumDoubleClickDelayMilliseconds
        {
            get => minimumDelayMilliseconds;
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

        private void ReadAppSettings()
        {
            var settings = ConfigurationManager.AppSettings!;
            minimumDelayMilliseconds = GetAppSetting(settings, nameof(minimumDelayMilliseconds), minimumDelayMilliseconds);
            leftThreshold = GetAppSetting(settings, nameof(leftThreshold), leftThreshold);
            rightThreshold = GetAppSetting(settings, nameof(rightThreshold), rightThreshold);
            middleThreshold = GetAppSetting(settings, nameof(middleThreshold), middleThreshold);
            x1Threshold = GetAppSetting(settings, nameof(x1Threshold), x1Threshold);
            x2Threshold = GetAppSetting(settings, nameof(x2Threshold), x2Threshold);
        }
        private static int GetAppSetting(NameValueCollection settings, string key, int currentValue)
        {
            string? value = settings[nameof(key)];
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
                    CultureInfo culture = new CultureInfo(value);
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
}
