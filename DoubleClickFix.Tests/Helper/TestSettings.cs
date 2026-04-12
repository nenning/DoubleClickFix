using System.Drawing;
namespace DoubleClickFix.Tests.Helper;

class TestSettings : ISettings
{
    private HashSet<string> ignoredDevicePaths = [];
    public IReadOnlySet<string> IgnoredDevicePaths => ignoredDevicePaths;
    public void AddIgnoredDevice(string path) { ignoredDevicePaths.Add(path); listener?.Invoke(); }
    public void RemoveIgnoredDevice(string path) { ignoredDevicePaths.Remove(path); listener?.Invoke(); }
    public bool IsInteractive { get; set; } = false;
    public bool UseHook { get; set; } = true;
    public int WindowsDoubleClickTimeMilliseconds { get; set; } = 500;
    public int LeftThreshold { get; set; } = 50;
    public int MiddleThreshold { get; set; } = -1;
    public int MinDelay { get; set; } = -1;
    public int RightThreshold { get; set; } = -1;
    public int X1Threshold { get; set; } = -1;
    public int X2Threshold { get; set; } = -1;
    public int WheelThreshold { get; set; } = -1;
    public bool IsDragCorrectionEnabled => DragStartTimeMilliseconds >= 0 && DragStopTimeMilliseconds >= 0;
    public int DragStartTimeMilliseconds { get; set; } = -1;
    public int DragStopTimeMilliseconds { get; set; } = -1;
    public bool IsRemoteDesktopDetectionEnabled { get; set; } = false;

    public Rectangle? RestartBounds { get; set; }
    public bool IsFirstAppStart => false;
    public string Language { get; set; } = "";
    public ColorMode ColorMode { get; set; } = ColorMode.System;

    private Action? listener;
    public void RegisterSettingsChangedListener(Action listener)
    {
        this.listener = listener;
    }
    /// <summary>
    /// has to be called explicitly
    /// </summary>
    public void FireSettingsChanged()
    {
        listener?.Invoke();
    }
    public void Save()
    {
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
}
