﻿namespace DoubleClickFix;

internal interface INativeMethods
{
    nint SetHook(NativeMethods.LowLevelMouseProc proc);
    void UnhookWindowsHook(nint hhk);
    nint CallNextHook(nint hhk, int nCode, nint wParam, nint lParam);
    void RegisterForRawInput(nint hwnd);
    bool TryProcessRawInput(nint hRawInput, out nint device);
}