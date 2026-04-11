namespace DoubleClickFix.Tests;

public class LoggerTests
{
    [Fact]
    public void LogAfterDispose_DoesNotThrow()
    {
        Logger logger = new();
        logger.Dispose();

        // Should not throw ObjectDisposedException
        logger.Log("message after dispose");
    }

    [Fact]
    public void LogBeforeDispose_DoesNotThrow()
    {
        using Logger logger = new();
        logger.Log("normal message");
    }
}
