namespace DoubleClickFix
{
    internal interface IStartupRegistry
    {
        bool IsRegistered();
        bool Register();
        bool Unregister();
    }
}