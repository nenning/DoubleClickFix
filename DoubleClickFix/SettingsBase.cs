using System.Configuration;
using System.Diagnostics;
using System.Globalization;

namespace DoubleClickFix
{
    /// <summary>
    /// Delay in milliseconds per mouse key.Use -1 to disable double click fix for a key or remove the corresponding setting. 
    /// IgnoredDevice: Used to ignore touch screen or touch pad double clicks. Usually the id of such a device is 0. But it can be modified here if needed.
    /// MinDelay: Can be used to define a minimum delay of 0. Workaround for touch screen or touch pad recognition based on device id not working. To deactivate use -1.
    /// </summary>
    internal abstract class SettingsBase: ISettings
    {

        public SettingsBase(string[] args, ILogger logger)
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

        protected readonly ILogger logger;
        protected readonly int windowsDoubleClickTimeMilliseconds = GetWindowsMaximumDoubleClickTime();

        protected int ignoredDevice = 0;

        protected int leftThreshold = 50;
        protected int middleThreshold = -1;

        protected int minDelay = -1;
        protected int rightThreshold = -1;
        private Action settingsChanged;

        protected int x1Threshold = -1;
        protected int x2Threshold = -1;
        public int IgnoredDevice
        {
            get => ignoredDevice;
            set
            {
                if (value != ignoredDevice)
                {
                    ignoredDevice = value;
                    FireSettingsChanged();
                }
            }
        }

        public bool IsFirstAppStart { get; private init; }
        public bool IsInteractive { get; private set; }
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
        public int MinDelay
        {
            get => minDelay;
            set
            {
                if (value != minDelay)
                {
                    minDelay = value;
                    FireSettingsChanged();
                }
            }
        }
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

        /// <summary>
        /// When debugging with the mouse hook and entering a breakpoint, the mouse can lag significantly. Use the -nohook command line argument in this case.
        /// </summary>
        public bool UseHook { get; private set; }

        public int WindowsDoubleClickTimeMilliseconds
        {
            get => windowsDoubleClickTimeMilliseconds;
        }
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

        private static int GetWindowsMaximumDoubleClickTime()
        {
            int doubleClickTime = NativeMethods.GetDoubleClickTime();
            if (doubleClickTime == 0)
            {
                doubleClickTime = 600;
            }
            return doubleClickTime;
        }

        public void RegisterSettingsChangedListener(Action listener)
        {
            this.settingsChanged += listener;
        }

        public void Reset()
        {
            MinDelay = -1;
            LeftThreshold = 50;
            MiddleThreshold = -1;
            RightThreshold = -1;
            X1Threshold = -1;
            X2Threshold = -1;
            IgnoredDevice = 0;
        }
        protected void FireSettingsChanged()
        {
            settingsChanged?.Invoke();
        }
        private void OnSettingsChanged() { }

        public abstract void Save();
        protected abstract bool SettingsExist();
        public abstract void Load();

    }
}