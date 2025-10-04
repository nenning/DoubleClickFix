using System.Configuration;
using System.Diagnostics;
using System.Globalization;

namespace DoubleClickFix
{
    internal abstract class SettingsBase: ISettings
    {
        private readonly int windowsDoubleClickTimeMilliseconds = GetWindowsMaximumDoubleClickTime();
        private Action settingsChanged;

        protected readonly ILogger logger;
        
        protected int ignoredDevice = 0;
        protected int minDelay = -1;

        protected int leftThreshold = 50;
        protected int middleThreshold = -1;
        protected int rightThreshold = -1;

        protected int x1Threshold = -1;
        protected int x2Threshold = -1;
        protected int wheelThreshold = -1;

        protected int dragStartTimeMilliseconds = -1;
        protected int dragStopTimeMilliseconds = -1;

        public SettingsBase(string[] args, ILogger logger)
        {
            this.logger = logger;
            UseHook = !Debugger.IsAttached && (args.Length == 0 || !args.Contains("-nohook"));
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
            get => leftThreshold;
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
            get => middleThreshold;
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
            get => rightThreshold;
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
            get => x1Threshold;
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
            get => x2Threshold;
            set
            {
                if (value != x2Threshold)
                {
                    x2Threshold = value;
                    FireSettingsChanged();
                }
            }
        }

        public int WheelThreshold
        {
            get => wheelThreshold;
            set
            {
                if (value != wheelThreshold)
                {
                    wheelThreshold = value;
                    FireSettingsChanged();
                }
            }
        }

        public bool IsDragCorrectionEnabled => DragStartTimeMilliseconds >= 0 && DragStopTimeMilliseconds >= 0;

        public int DragStartTimeMilliseconds { 
            get => dragStartTimeMilliseconds;
            set
            {
                if (value != dragStartTimeMilliseconds)
                {
                    value = Math.Max(value, -1);
                    value = Math.Min(value, 2000);
                    dragStartTimeMilliseconds = value;
                    FireSettingsChanged();
                }
            }
        }

        public int DragStopTimeMilliseconds
        {
            get => dragStopTimeMilliseconds;
            set
            {
                if (value != dragStopTimeMilliseconds)
                {
                    value = Math.Max(value, -1);
                    value = Math.Min(value, 500);
                    dragStopTimeMilliseconds = value;
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
            WheelThreshold = -1;
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