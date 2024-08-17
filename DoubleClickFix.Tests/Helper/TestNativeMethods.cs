namespace DoubleClickFix.Tests.Helper;

/// <summary>
/// HookCallback:
/// in: (int nCode, IntPtr wParam, IntPtr lParam)
///  - nCode: >= 0; otherwise ignored.
///  - wparam: WM_LBUTTONDOWN, etc.
///  - lParam: MSLLHOOKSTRUCT*
/// out: 
///  - 1: click ignored
///  - 0: click processed (and CallNextHook(..) is called, i.e. counter increased)
/// </summary>
class TestNativeMethods : INativeMethods
{
    public int CallCounter { get; private set; }
    public nint CallNextHook(nint hhk, int nCode, nint wParam, nint lParam)
    {
        CallCounter++;
        return nint.Zero;
    }
}
