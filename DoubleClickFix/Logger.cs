using System.Collections.Concurrent;
using System.Diagnostics;

namespace DoubleClickFix;

internal class Logger : ILogger, IDisposable
{
    private readonly ConcurrentQueue<string> logQueue = new();
    private readonly ManualResetEventSlim logSignal = new(false);
    private readonly CancellationTokenSource cancelTokenSource = new();
    private Task logTask;
    private Action<string>? uiLog;
    private SynchronizationContext? syncContext;
    private bool disposed;

    public Logger()
    {
        // Start the log processing task with cancellation support
        logTask = Task.Run(() => ProcessLogEntries(cancelTokenSource.Token), cancelTokenSource.Token);
    }

    /// <summary>Indicates whether the UI is currently visible.</summary>
    public bool IsAppVisible { get; set; } = true;

    /// <summary>Register a UI callback for logging. Called from the UI thread.</summary>
    public void AddGuiLogger(Action<string> logger)
    {
        // Capture the current SynchronizationContext so posting occurs on the UI thread
        syncContext = SynchronizationContext.Current;
        if (syncContext == null)
        {
            Debug.WriteLine($"Logger loaded: syncContext is null!");
        }
        uiLog = logger;
    }

    /// <summary>Queue a log entry; optionally restrict to foreground if the UI is visible.</summary>
    public void Log(string message, bool foregroundOnly = false)
    {
        if (disposed) return;
        if (!foregroundOnly || IsAppVisible)
        {
            logQueue.Enqueue(message);
            logSignal.Set();
        }
    }

    /// <summary>Processes queued log entries on a background thread.</summary>
    private void ProcessLogEntries(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                logSignal.Wait(cancellationToken);
                while (logQueue.TryDequeue(out var message))
                {
                    // Write to debug output
                    Debug.WriteLine(message);

                    // Post to the UI thread if available, catching any exceptions
                    try
                    {
                        if (syncContext != null && uiLog != null)
                        {
                            syncContext.Post(_ => uiLog?.Invoke(message), null);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // UI has been disposed – discard any further UI log callbacks
                        uiLog = null;
                        syncContext = null;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Logger error posting to UI: {ex}");
                    }
                }

                // Reset the signal once all messages are processed
                logSignal.Reset();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when disposal cancels the loop
        }
    }

    /// <summary>Dispose releases resources and stops the background thread.</summary>
    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
            cancelTokenSource.Cancel();

            // Wake up the wait if necessary
            logSignal.Set();

            try
            {
                logTask?.Wait();
            }
            catch (AggregateException) { }

            cancelTokenSource.Dispose();
            logSignal.Dispose();

            logTask = null!;
        }
    }
}