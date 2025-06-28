namespace DoubleClickFix.Tests.Helper;

class TestSettings : ISettings
{
    /// <summary>
    /// mousehook uses -1 as current default device. testsetting uses 0 as default, so it's not ignored.
    /// </summary>
    public int IgnoredDevice { get; set; } = 0;
    public bool IsInteractive { get; set; } = false;
    public bool UseHook { get; set; } = true;
    public int WindowsDoubleClickTimeMilliseconds { get; set; } = 500;
    public int LeftThreshold { get; set; } = 50;
    public int MiddleThreshold { get; set; } = -1;
    public int MinDelay { get; set; } = -1;
    public int RightThreshold { get; set; } = -1;
    public int X1Threshold { get; set; } = -1;
    public int X2Threshold { get; set; } = -1;
    public bool IsDragCorrectionEnabled => DragStartTimeMilliseconds >= 0 && DragStopTimeMilliseconds >= 0;
    public int DragStartTimeMilliseconds { get; set; } = -1;
    public int DragStopTimeMilliseconds { get; set; } = -1;

    public bool IsFirstAppStart => false;

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
}
