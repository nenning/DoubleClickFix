using System.Collections.Concurrent;
using System.Diagnostics;

namespace DoubleClickFix;

public class Logger : ILogger
{
    private readonly ConcurrentQueue<string> logQueue = new();
    private readonly ManualResetEventSlim logSignal = new(false);
    private Action<string>? uiLog;
    private SynchronizationContext? syncContext;
    public Logger()
    {
        Task.Run(ProcessLogEntries);
    }
    public bool IsAppVisible { get; set; } = true;
    public void AddGuiLogger(Action<string> logger)
    {
        syncContext = SynchronizationContext.Current;
        if (syncContext == null)
        {
            Debug.WriteLine($"Logger loaded: syncContext is null!");
        }
        this.uiLog = logger;
    }
    public void Log(string message, bool foregroundOnly = false)
    {
        if (!foregroundOnly || IsAppVisible)
        {
            logQueue.Enqueue(message);
            logSignal.Set();
        }
    }

    private void ProcessLogEntries()
    {
        while (true)
        {
            logSignal.Wait();
            if (logQueue.TryDequeue(out string? message))
            {
                Debug.WriteLine(message);
                syncContext?.Post(status =>
                {
                    uiLog?.Invoke(message);
                }, null);
            }
            else
            {
                logSignal.Reset();
            }
        }
    }

}