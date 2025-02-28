using Windows.ApplicationModel;

namespace DoubleClickFix;

internal class StoreStartupRegistry(ILogger Logger) : IStartupRegistry
{
    private bool isRegistered = false;

    public bool IsRegistered()
    {
        try
        {
            var task = GetStartupTask();
            isRegistered = task?.State == StartupTaskState.Enabled;
            return isRegistered;
        }
        catch (Exception ex)
        {
            Logger.Log($"Error: {ex}");
            isRegistered = false;
            return false;
        }
    }

    private static StartupTask? GetStartupTask()
    {
        return StartupTask.GetForCurrentPackageAsync().GetAwaiter().GetResult().FirstOrDefault();
    }

    public bool Register()
    {
        if (isRegistered)
        {
            return true;
        }
        try
        {
            var task = GetStartupTask();
            var enabled = task?.State == StartupTaskState.Enabled;
            if ( task == null) 
            {
                Logger.Log("Startup task not found.");
                return false;
            }
            if (!enabled)
            {
                var result = task.RequestEnableAsync().GetAwaiter().GetResult();
                if (result != StartupTaskState.Enabled)
                {
                    Logger.Log("Startup task not enabled.");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"Error: {ex}");
            return false;
        }
        isRegistered = true;
        return true;
    }
    public bool Unregister()
    {
        if (!isRegistered)
        {
            return true;
        }
        try
        {
            var task = GetStartupTask();
            var enabled = task?.State == StartupTaskState.Enabled;
            if (task == null)
            {
                Logger.Log("Startup task not found.");
                return false;
            }
            if (enabled)
            {
                task.Disable();
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"Error: {ex}");
            return false;
        }
        isRegistered = false;
        return true;
    }
}