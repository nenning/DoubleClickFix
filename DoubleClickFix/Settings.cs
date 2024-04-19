using System.Configuration;
using System.Diagnostics;

namespace DoubleClickFix
{
    public class Settings
    {
        private const string SettingsKey = "MinimumDelayMilliseconds";
        private readonly int windowsDoubleClickTimeMilliseconds = GetWindowsMaximumDoubleClickTime();
        private readonly ILogger logger;
        private int minimumDelayMilliseconds = GetMinimumDelayFromAppSettings();

        public Settings(string[] args, ILogger logger)
        {
            this.logger = logger;
            UseHook = !Debugger.IsAttached || args.Length == 0 || !args.Contains("-nohook");
            IsInteractive = Debugger.IsAttached || args.Length > 0 && (args.Contains("-interactive") || args.Contains("-i"));
        }

        /// <summary>
        /// When debugging with the mouse hook and entering a breakpoint, the mouse can lag significantly. Use the -nohook command line argument in this case.
        /// </summary>
        public bool UseHook { get; private set; }
        public bool IsInteractive { get; private set; }

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
