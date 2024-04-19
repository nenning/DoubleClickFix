using System.Configuration;

namespace DoubleClickFix
{
    public class Settings(ILogger logger)
    {
        private const string SettingsKey = "MinimumDelayMilliseconds";
        private readonly int windowsDoubleClickTimeMilliseconds = GetWindowsMaximumDoubleClickTime();
        private int minimumDelayMilliseconds = GetMinimumDelayFromAppSettings();

        public void UpdateAppSettings(int value)
        {
            if (value <= 0 || value >= windowsDoubleClickTimeMilliseconds)
            {
                logger.Log($"Invalid value - not saved. Valid range: [0..{windowsDoubleClickTimeMilliseconds}]");
                return;
            }
            try
            {
                minimumDelayMilliseconds = value;
                var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings[SettingsKey].Value = value.ToString();
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                logger.Log("Settings saved.");
            } catch (Exception ex)
            {
                logger.Log("Error: " + ex.ToString());
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

        private static int GetMinimumDelayFromAppSettings()
        {
            string? value = ConfigurationManager.AppSettings[SettingsKey];
            if (value == null || !int.TryParse(value, out int minimumDelay))
            {
                minimumDelay = 142;
            }
            return minimumDelay;
        }
    }
}
