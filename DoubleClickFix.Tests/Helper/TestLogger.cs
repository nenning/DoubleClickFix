namespace DoubleClickFix.Tests.Helper;

class TestLogger : ILogger
{
    public bool IsAppVisible => true;

    public void Log(string message, bool foregroundOnly = false)
    {
    }
}
