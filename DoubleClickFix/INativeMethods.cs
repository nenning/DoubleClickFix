namespace DoubleClickFix
{
    public interface INativeMethods
    {
        internal IntPtr CallNextHook(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }
}