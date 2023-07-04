using TouchChan.AssistiveTouch.NativeMethods;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TouchChan.AssistiveTouch.Helper;

internal class GameWindowHooker : IDisposable
{
    public event EventHandler<Size>? SizeChanged;

    private readonly IntPtr _windowsEventHook;

    private readonly GCHandle _gcSafetyHandle;

    private readonly IntPtr _touchWindow;

    public GameWindowHooker(IntPtr touchWindow)
    {
        _touchWindow = touchWindow;

        var targetThreadId = User32.GetWindowThreadProcessId(App.GameWindowHandle, out var pid);

        User32.WinEventProc winEventDelegate = WinEventCallback;
        _gcSafetyHandle = GCHandle.Alloc(winEventDelegate);

        _windowsEventHook = User32.SetWinEventHook(
             EventObjectLocationChange, EventObjectLocationChange,
             IntPtr.Zero, winEventDelegate, pid, targetThreadId,
             WinEventHookInternalFlags);

        _throttle = new(300, rectClient =>
        {
            _rev = !_rev;
            Win32.SetWindowSize(_touchWindow, rectClient.Width + (_rev ? 1 : -1), rectClient.Height);
        });

        // Lose focus
        const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        var focusStatus = User32.GetForegroundWindow() == App.GameWindowHandle;
        User32.WinEventProc winProc = (_, _, h, _, _, _, _) =>
        {
            if (focusStatus && h != App.GameWindowHandle)
            {
                FocusLost?.Invoke(this, new());
                focusStatus = false;
            }
            else if (h == App.GameWindowHandle)
            {
                focusStatus = true;
            }
        };
        var gcSafetyHandle = GCHandle.Alloc(winProc);
        var focusEventHook = User32.SetWinEventHook(
             EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
             IntPtr.Zero, winProc, 0, 0,
             User32.WINEVENT.WINEVENT_OUTOFCONTEXT);
    }

    public EventHandler? FocusLost { get; set; }

    // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwineventhook
    private const User32.WINEVENT WinEventHookInternalFlags = User32.WINEVENT.WINEVENT_OUTOFCONTEXT |
                                                              User32.WINEVENT.WINEVENT_SKIPOWNPROCESS;
    //private const uint EventObjectFocus = 0x8005;
    private const uint EventObjectLocationChange = 0x800B;
    private const long SWEH_CHILDID_SELF = 0;
    private const int OBJID_WINDOW = 0;

    /// <summary>
    /// Running in UI thread
    /// </summary>
    private void WinEventCallback(
        IntPtr hWinEventHook,
        uint eventType,
        IntPtr hWnd,
        int idObject,
        int idChild,
        uint dwEventThread,
        uint dwmsEventTime)
    {
        if (eventType == EventObjectLocationChange &&
            hWnd == App.GameWindowHandle &&
            idObject == OBJID_WINDOW && idChild == SWEH_CHILDID_SELF)
        {
            User32.GetClientRect(hWnd, out var rectClient);

            if (rectClient.Size != _lastGameWindowSize)
            {
                Win32.SetWindowSize(_touchWindow, rectClient.Width, rectClient.Height);
                _lastGameWindowSize = rectClient.Size;
                SizeChanged?.Invoke(this, rectClient.Size);
            }
            else
            {
                // https://bugreports.qt.io/browse/QTBUG-64116?focusedCommentId=377425&page=com.atlassian.jira.plugin.system.issuetabpanels%3Acomment-tabpanel
                // HACK: Hack way to fix touch position
                _throttle.Signal(rectClient);
            }

            var p = new Point();
            User32.MapWindowPoints(_touchWindow, hWnd, ref p);
            if (p.X != 0 || p.Y != 0)
            {
                Win32.MoveWindowToOrigin(_touchWindow);
            }
        }
    }

    private readonly Throttle<User32.RECT> _throttle;
    private bool _rev;
    private Size _lastGameWindowSize;

    public void Dispose()
    {
        _gcSafetyHandle.Free();
        // May produce EventObjectDestroy
        User32.UnhookWinEvent(_windowsEventHook);
    }
}
