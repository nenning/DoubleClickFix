namespace DoubleClickFix;

internal interface INativeMethods
{
    nint SetHook(NativeMethods.LowLevelMouseProc proc);
    void UnhookWindowsHook(nint hhk);
    nint CallNextHook(nint hhk, int nCode, nint wParam, nint lParam);
    void RegisterForRawInput(nint hwnd);
    string? TryGetDevicePath(nint hDevice);
    bool TryGetRawInputDeviceHandle(nint hRawInput, out nint device);
    bool TryProcessRawInput(nint hRawInput, out nint device, out NativeMethods.DeviceType deviceType);
    nint GetWindowLong(nint hWnd, int nIndex);
    nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong);
    bool IsRemoteSession();
}