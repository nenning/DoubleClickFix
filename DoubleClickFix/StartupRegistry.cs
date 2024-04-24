using Microsoft.Win32;
using System.Diagnostics;

namespace DoubleClickFix
{
    public class StartupRegistry
    {
        private const string registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string registryKey = @"DoubleClickFix";

        private readonly string registryValue = Environment.ProcessPath!;

        public bool IsRegistered()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(registryPath, false);
                var value = key!.GetValue(registryKey, null);
                return value != null && (string)value == registryValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return false;
            }
        }

        public bool Register()
        {
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
            return true;
        }
        public bool Unregister()
        {
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
            return true;
        }
    }
}
