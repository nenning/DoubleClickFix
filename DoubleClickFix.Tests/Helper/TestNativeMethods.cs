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
    public int CallNextHookCounter { get; private set; }
    public int SetHookCounter { get; private set; }
    public int UnhookWindowsHookCounter { get; private set; }
    public int RegisterForRawInputCounter { get; private set; }
    public int ProcessRawInputCounter { get; private set; }

    public nint NextHookResult { get; set; } = nint.Zero;
    public nint SetHookResult { get; set; } = new(123);
    public Action? UnhookWindowsHookAction { get; set; }
    public Action? RegisterForRawInputAction { get; set; }
    public Func<nint, (bool, nint)>? ProcessRawInputFunc { get; set; }

    public nint CallNextHook(nint hhk, int nCode, nint wParam, nint lParam)
    {
        CallNextHookCounter++;
        return NextHookResult;
    }

    public nint SetHook(NativeMethods.LowLevelMouseProc proc)
    {
        SetHookCounter++;
        return SetHookResult;
    }

    public void UnhookWindowsHook(nint hhk)
    {
        UnhookWindowsHookCounter++;
        UnhookWindowsHookAction?.Invoke();
    }

    public void RegisterForRawInput(nint hwnd)
    {
        RegisterForRawInputCounter++;
        RegisterForRawInputAction?.Invoke();
    }

    public bool TryProcessRawInput(nint hRawInput, out nint device)
    {
        ProcessRawInputCounter++;
        if (ProcessRawInputFunc != null)
        {
            var (result, dev) = ProcessRawInputFunc(hRawInput);
            device = dev;
            return result;
        }
        device = 0;
        return false;
    }

    public nint GetWindowLong(nint hWnd, int nIndex)
    {
        throw new NotImplementedException();
    }

    public nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong)
    {
        throw new NotImplementedException();
    }
}
