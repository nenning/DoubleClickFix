namespace DoubleClickFix;

/// <summary>
/// Lightweight message-only window that receives WM_INPUT messages for raw input
/// device classification. Using this instead of InteractiveForm allows the mouse
/// hook to be installed before the heavy UI is constructed.
/// </summary>
internal class RawInputWindow : NativeWindow, IDisposable
{
    private const int WM_INPUT = 0x00FF;
    private readonly MouseHook mouseHook;
    private Action? showMeHandler;
    private bool pendingShowMe;

    public RawInputWindow(MouseHook mouseHook)
    {
        this.mouseHook = mouseHook;
        var cp = new CreateParams
        {
            // HWND_MESSAGE parent creates a message-only window (no painting, no layout)
            Parent = new IntPtr(-3)
        };
        CreateHandle(cp);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_INPUT)
        {
            mouseHook.ProcessRawInput(m.LParam);
        }
        else if (m.Msg == NativeMethods.WM_SHOWME)
        {
            if (showMeHandler != null)
                showMeHandler.Invoke();
            else
                pendingShowMe = true;
        }
        base.WndProc(ref m);
    }

    public void SetShowMeHandler(Action handler)
    {
        showMeHandler = handler;
        if (pendingShowMe)
        {
            pendingShowMe = false;
            handler.Invoke();
        }
    }

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
            DestroyHandle();
        GC.SuppressFinalize(this);
    }
}
