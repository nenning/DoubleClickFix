namespace DoubleClickFix;

internal interface INativeMethods
{
    internal nint CallNextHook(nint hhk, int nCode, nint wParam, nint lParam);
}