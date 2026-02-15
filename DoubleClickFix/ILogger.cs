namespace DoubleClickFix;

internal interface ILogger
{
    /// <summary>
    /// Log to the UI.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="foregroundOnly">Logs only if the UI is visible, otherwise discards the message.</param>
    void Log(string message, bool foregroundOnly = false);

    public bool IsAppVisible { get; }
}