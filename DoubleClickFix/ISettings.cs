
namespace DoubleClickFix;

internal interface ISettings
{
    IReadOnlySet<string> IgnoredDevicePaths { get; }
    void AddIgnoredDevice(string path);
    void RemoveIgnoredDevice(string path);
    bool IsInteractive { get; }
    int LeftThreshold { get; set; }
    int MiddleThreshold { get; set; }
    int MinDelay { get; set; }
    int RightThreshold { get; set; }
    bool UseHook { get; }
    int WindowsDoubleClickTimeMilliseconds { get; }
    int X1Threshold { get; set; }
    int X2Threshold { get; set; }
    int WheelThreshold { get; set; }
    
    bool IsDragCorrectionEnabled { get; }
    int DragStartTimeMilliseconds { get; set; }
    int DragStopTimeMilliseconds { get; set; }

    bool IsRemoteDesktopDetectionEnabled { get; set; }

    bool IsFirstAppStart { get; }
    string Language { get; set; }
    void RegisterSettingsChangedListener(Action listener);
    void Save();
    void Reset();
}