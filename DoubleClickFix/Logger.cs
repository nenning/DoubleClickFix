using System.Diagnostics;

namespace DoubleClickFix
{
    public class Logger : ILogger
    {
        private Action<string> log;

        public Logger()
        {
            log += text => Debug.WriteLine(text);
        }
        public void AddLogger(Action<string> logger)
        {
            this.log += logger;
        }
        public void Log(string message) { 
            log(message);
        }
    }
}