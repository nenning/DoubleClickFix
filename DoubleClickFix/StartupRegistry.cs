using Microsoft.Win32;

namespace DoubleClickFix;

public class StartupRegistry
{
    private const string registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string registryKey = @"DoubleClickFix";

    private readonly string registryValue = Environment.ProcessPath!;

    private bool isRegistered = false;

    public bool IsRegistered()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(registryPath, false);
            var value = key!.GetValue(registryKey, null);
            isRegistered = value != null && (string)value == registryValue;
            return isRegistered;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
            isRegistered = false;
            return false;
        }
    }

    public bool Register()
    {
        if (isRegistered)
        {
            return true;
        }
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(registryPath, true);
            key!.SetValue(registryKey, registryValue, RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
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
            using var key = Registry.CurrentUser.OpenSubKey(registryPath, true);
            key!.DeleteValue(registryKey, false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
            return false;
        }
        isRegistered = false;
        return true;
    }
}
