namespace DoubleClickFix
{
    internal interface INativeMethods
    {
        internal IntPtr CallNextHook(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }
}