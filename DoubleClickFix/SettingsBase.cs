using System.Diagnostics;
using System.Globalization;

namespace DoubleClickFix
{
    internal abstract class SettingsBase: ISettings
    {
        private readonly int windowsDoubleClickTimeMilliseconds = GetWindowsMaximumDoubleClickTime();
        private Action settingsChanged;

        protected readonly ILogger logger;
        
        protected HashSet<string> ignoredDevicePaths = [];
        protected int minDelay = -1;

        protected int leftThreshold = 50;
        protected int middleThreshold = -1;
        protected int rightThreshold = -1;

        protected int x1Threshold = -1;
        protected int x2Threshold = -1;
        protected int wheelThreshold = -1;

        protected int dragStartTimeMilliseconds = -1;
        protected int dragStopTimeMilliseconds = -1;
        protected int remoteDesktopDetection = 0;
        protected string language = "";
        protected ColorMode colorMode = ColorMode.System;

        public SettingsBase(string[] args, ILogger logger)
        {
            this.logger = logger;
            UseHook = !Debugger.IsAttached && (args.Length == 0 || !args.Contains("-nohook"));
            // Load all settings in one pass; returns false on first ever launch (no saved state).
            bool existed = Load();
            IsFirstAppStart = !existed;
            if (IsFirstAppStart)
            {
                Save();
            }
            // language field is now populated by Load() — apply culture before any UI is created.
            ApplyLanguageOverride();
            IsInteractive = Debugger.IsAttached || IsFirstAppStart || args.Length > 0 && (args.Contains("-interactive") || args.Contains("-i"));
            RestartBounds = ParseBounds(args);
            settingsChanged += this.OnSettingsChanged;
        }

        public IReadOnlySet<string> IgnoredDevicePaths => ignoredDevicePaths;

        public void AddIgnoredDevice(string path)
        {
            if (ignoredDevicePaths.Add(path)) FireSettingsChanged();
        }

        public void RemoveIgnoredDevice(string path)
        {
            if (ignoredDevicePaths.Remove(path)) FireSettingsChanged();
        }

        public bool IsFirstAppStart { get; private init; }
        public bool IsInteractive { get; private set; }
        public Rectangle? RestartBounds { get; private set; }
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
                
        public bool IsRemoteDesktopDetectionEnabled
        {
            get => remoteDesktopDetection > 0;
            set
            {
                int newVal = value ? 1 : 0;
                if (newVal != remoteDesktopDetection)
                {
                    remoteDesktopDetection = newVal;
                    FireSettingsChanged();
                }
            }
        }
        public string Language
        {
            get => language;
            set { language = value; }
        }
        public ColorMode ColorMode
        {
            get => colorMode;
            set { colorMode = value; }
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

        private void ApplyLanguageOverride()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(language))
                    ApplyCulture(language);
            }
            catch
            {
            }
        }

        private static void ApplyCulture(string code)
        {
            CultureInfo culture = new(code);
            Application.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture.Name);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture.Name);
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
            DragStartTimeMilliseconds = -1;
            DragStopTimeMilliseconds = -1;
            IsRemoteDesktopDetectionEnabled = false;
            ignoredDevicePaths.Clear();
        }
        protected void FireSettingsChanged()
        {
            settingsChanged?.Invoke();
        }
        private void OnSettingsChanged() { }

        public abstract void Save();
        /// <summary>Loads all settings. Returns true if previously saved settings were found.</summary>
        public abstract bool Load();

        private static Rectangle? ParseBounds(string[] args)
        {
            int i = Array.IndexOf(args, "-bounds");
            if (i < 0 || i + 1 >= args.Length) return null;
            string[] parts = args[i + 1].Split(',');
            if (parts.Length == 4
                && int.TryParse(parts[0], out int x)
                && int.TryParse(parts[1], out int y)
                && int.TryParse(parts[2], out int w)
                && int.TryParse(parts[3], out int h)
                && w > 0 && h > 0)
            {
                return new Rectangle(x, y, w, h);
            }
            return null;
        }
    }
}