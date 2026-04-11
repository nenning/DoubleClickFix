namespace DoubleClickFix.Tests.Helper;

class ThrowingLogger : ILogger
{
    public bool IsAppVisible => true;

    public void Log(string message, bool foregroundOnly = false)
    {
        throw new ObjectDisposedException("logger");
    }
}
