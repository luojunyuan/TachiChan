using TouchChan.AssistiveTouch.NativeMethods;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;

namespace TouchChan.AssistiveTouch.Helper;

internal class GameWindowHookerOld : IDisposable
{
    public event EventHandler<Size>? SizeChanged;

    public event EventHandler<WindowPosition>? WindowPositionChanged;

    private readonly IntPtr _windowsEventHook;

    private readonly GCHandle _gcSafetyHandle;

    public GameWindowHookerOld()
    {
        var targetThreadId = User32.GetWindowThreadProcessId(App.GameWindowHandle, out var pid);

        User32.WinEventProc winEventDelegate = WinEventCallback;
        _gcSafetyHandle = GCHandle.Alloc(winEventDelegate);

        _windowsEventHook = User32.SetWinEventHook(
             EventObjectDestroy, EventObjectLocationChange,
             IntPtr.Zero, winEventDelegate, pid, targetThreadId,
             WinEventHookInternalFlags);
    }

    // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwineventhook
    private const User32.WINEVENT WinEventHookInternalFlags = User32.WINEVENT.WINEVENT_OUTOFCONTEXT |
                                                              User32.WINEVENT.WINEVENT_SKIPOWNPROCESS;
    private const uint EventObjectDestroy = 0x8001;
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
            UpdatePosition(hWnd);
        }

        if (eventType == EventObjectDestroy &&
            hWnd == App.GameWindowHandle &&
            idObject == OBJID_WINDOW && idChild == SWEH_CHILDID_SELF)
        {
            System.Windows.Application.Current.MainWindow.Close();
        }
    }

    public void UpdatePosition(IntPtr hWnd)
    {
        User32.GetWindowRect(hWnd, out var rect);
        User32.GetClientRect(hWnd, out var rectClient);

        var winShadow = (rect.Width - rectClient.right) / 2;
        var left = rect.left + winShadow;

        var wholeHeight = rect.bottom - rect.top;
        var winTitleHeight = wholeHeight - rectClient.bottom - winShadow;
        var top = rect.top + winTitleHeight;

        var windowPosition = new WindowPosition(rectClient.Height, rectClient.Width, left, top);

        if (rectClient.Size != _lastGameWindowSize)
        {
            _lastGameWindowSize = rectClient.Size;
            SizeChanged?.Invoke(this, rectClient.Size);
        }

        WindowPositionChanged?.Invoke(this, windowPosition);
    }

    private Size _lastGameWindowSize;

    public void Dispose()
    {
        _gcSafetyHandle.Free();
        // May produce EventObjectDestroy
        User32.UnhookWinEvent(_windowsEventHook);
    }

    public readonly record struct WindowPosition
    {
        public double Height { get; }
        public double Width { get; }
        public double Left { get; }
        public double Top { get; }

        public WindowPosition(double height, double width, double left, double top)
        {
            Height = height;
            Width = width;
            Left = left;
            Top = top;
        }

        public override string ToString() => $"({Left}, {Top}), width={Width} height={Height}";
    }
}
