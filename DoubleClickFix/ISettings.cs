
namespace DoubleClickFix;

internal interface ISettings
{
    int IgnoredDevice { get; set; }
    bool IsInteractive { get; }
    int LeftThreshold { get; set; }
    int MiddleThreshold { get; set; }
    int MinDelay { get; set; }
    int RightThreshold { get; set; }
    bool UseHook { get; }
    int WindowsDoubleClickTimeMilliseconds { get; }
    int X1Threshold { get; set; }
    int X2Threshold { get; set; }
    
    bool IsDragCorrectionEnabled { get; }
    int DragStartTimeMilliseconds { get; set; }
    int DragStopTimeMilliseconds { get; set; }

    bool IsFirstAppStart { get; }
    void RegisterSettingsChangedListener(Action listener);
    void Save();
}