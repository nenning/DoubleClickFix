using System.Collections.Concurrent;
using System.Diagnostics;

namespace DoubleClickFix
{
    public class Logger : ILogger
    {
        private readonly ConcurrentQueue<string> logQueue = new();
        private readonly ManualResetEventSlim logSignal = new(false);
        private Action<string> log;

        public Logger()
        {
            log += text => Debug.WriteLine(text);
            Task.Run(ProcessLogEntries);

        }

        /// <summary>
        /// Careful: the logger is called on a non-UI thread.
        /// </summary>
        public void AddLogger(Action<string> logger)
        {
            this.log += logger;
        }
        public void Log(string message)
        {
            logQueue.Enqueue(message);
            logSignal.Set();
        }

        private void ProcessLogEntries()
        {
            while (true)
            {
                logSignal.Wait();
                if (logQueue.TryDequeue(out string? message))
                {
                    log(message);
                }
                else
                {
                    logSignal.Reset();
                }
            }
        }

    }

}