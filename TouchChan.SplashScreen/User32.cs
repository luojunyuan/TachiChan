using System.Runtime.InteropServices;

namespace SplashScreenGdip;

internal partial class User32
{
    public const uint WindowMessage_WM_DESTROY = 0x0002;
    public const uint WindowMessage_WM_CLOSE = 0x0010;
    public const uint WindowMessage_WM_DPICHANGED = 0x02E0;
    public const uint WindowMessage_WM_NCHITTEST = 0x0084;
    public const uint WindowStylesEx_WS_EX_LAYERED = 0x00080000;
    public const uint WindowStylesEx_WS_EX_NOACTIVATE = 0x08000000;
    public const uint WindowStyles_WS_OVERLAPPED = 0x0;
    public const int UpdateLayeredWindowFlags_ULW_ALPHA = 0x00000002;
    public const uint WindowClassStyles_CS_DBLCLKS = 0x8u;
    public const uint ShowWindowCommand_SW_SHOWNORMAL = 1;
    public const int SystemMetric_SM_CXSCREEN = 0;
    public const int SystemMetric_SM_CYSCREEN = 1;
    public const int MonitorFlags_MONITOR_DEFAULTTONULL = 0x00000000;
    public const int CW_USEDEFAULT = unchecked((int)0x80000000);
    public const nint HWND_TOPMOST = -1;

    public const nint SystemIcons_IDI_APPLICATION = 32512;
    public const int IDC_ARROW = 32512;
    public const nint SystemColorIndex_COLOR_WINDOW = 5;

    public const short HitTestValues_HTCAPTION = 2;
    public const short HitTestValues_HTNOWHERE = 0;

    private const string User32Dll = "user32.dll";

    // Windows

#if !NET472

    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "LoadIconW")]
    public static partial nint LoadIcon(nint hInstance, nint lpIconName);
    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "LoadCursorW")]
    public static partial nint LoadCursor(nint hInstance, int lpCursorName);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate nint WindowProc(nint hwnd, uint uMsg, nint wParam, nint lParam);

    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "RegisterClassExW")]
    public static partial ushort RegisterClassEx(ref WNDCLASSEX param);

    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "CreateWindowExW")]
    public static partial nint CreateWindowEx(uint dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
        [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, 
        uint dwStyle, int x, int y, int nWidth, int nHeight,
        nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

    [LibraryImport(User32Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [LibraryImport(User32Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ShowWindow(nint hWnd, uint nCmdShow);

    [LibraryImport(User32Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)] // in as ref
    public static partial bool UpdateLayeredWindow(
        nint hWnd, nint hdcDst, in POINT pptDst, in SIZE psize,
        nint hdcSrc, in POINT pptSrc, uint crKey, in Gdi32.BLENDFUNCTION pblend, uint dwFlags);

    // Width Height Top Left

    [LibraryImport(User32Dll, SetLastError = true)]
    public static partial int GetSystemMetrics(int nIndex);

    [LibraryImport(User32Dll, SetLastError = true)]
    public static partial nint MonitorFromWindow(nint hwnd, int dwFlags);

    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "GetMonitorInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMonitorInfo(nint hMonitor, ref MONITORINFOEX lpmi);

    [LibraryImport(User32Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(nint hWnd, out RECT lpRect);

    [LibraryImport(User32Dll, SetLastError = true)]
    public static partial uint GetDpiForWindow(nint hwnd);

    // The three to process window loop

    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "GetMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMessage(out nint lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport(User32Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TranslateMessage(in nint lpMsg);

    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "DispatchMessageW")]
    public static partial nint DispatchMessage(in nint lpMsg);

    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "DefWindowProcW")] // In WndProc the normal process of window
    public static partial nint DefWindowProc(nint hWnd, uint Msg, nint wParam, nint lParam);

    // Get release DC device capture

    [LibraryImport(User32Dll, SetLastError = true)]
    public static partial nint GetDC(nint ptr);

    [LibraryImport(User32Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ReleaseDC(nint hWnd, nint hDC);

    // Quit window

    [LibraryImport(User32Dll, SetLastError = true, EntryPoint = "PostMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PostMessage(nint hWnd, uint Msg, nint wParam, nint lParam);

    [LibraryImport(User32Dll, SetLastError = true)]
    public static partial void PostQuitMessage(int nExitCode);

    [LibraryImport(User32Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DestroyWindow(nint hWnd);

#else
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr WindowProc([In] IntPtr hwnd, [In] uint uMsg, [In] IntPtr wParam, [In] IntPtr lParam);

    [DllImport(User32Dll)]
    public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);
    [DllImport(User32Dll)]
    public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

    [DllImport(User32Dll, SetLastError = true, CharSet = CharSet.Auto)]
    public static extern ushort RegisterClassEx(ref WNDCLASSEX Arg1);

    [DllImport(User32Dll)]
    public static extern IntPtr CreateWindowEx([In] uint dwExStyle, string lpClassName,
        string lpWindowName, [In] uint dwStyle, int x, int y, int nWidth, int nHeight,
        IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern IntPtr GetDC([In, Optional] IntPtr ptr);

    [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

    [DllImport(User32Dll, SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMessage(out nint lpMsg, [Optional] IntPtr hWnd, [Optional] uint wMsgFilterMin, [Optional] uint wMsgFilterMax);

    [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TranslateMessage(in nint lpMsg);

    [DllImport(User32Dll, SetLastError = false, CharSet = CharSet.Auto)]
    public static extern IntPtr DispatchMessage(in nint lpMsg);

    [DllImport(User32Dll, SetLastError = true, ExactSpelling = true)]
    public static extern int GetSystemMetrics(int nIndex);

    [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

    [DllImport(User32Dll, SetLastError = false, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport(User32Dll, SetLastError = true, ExactSpelling = true)]
    [System.Security.SecurityCritical]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport(User32Dll, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [System.Security.SecurityCritical]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport(User32Dll, SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDst, in POINT pptDst, in SIZE psize, IntPtr hdcSrc, in POINT pptSrc, uint crKey, in Gdi32.BLENDFUNCTION pblend, int dwFlags);

    [DllImport(User32Dll, SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage([Optional] IntPtr hWnd, uint Msg, [Optional] IntPtr wParam, [Optional] IntPtr lParam);

    [DllImport(User32Dll, SetLastError = false, ExactSpelling = true)]
    public static extern void PostQuitMessage([Optional] int nExitCode);

    [DllImport(User32Dll, SetLastError = false, CharSet = CharSet.Auto)]
    public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport(User32Dll, SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(IntPtr hWnd);

#endif

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    internal unsafe struct MONITORINFOEX
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public int dwFlags;
        public fixed char szDevice[32];
        public static MONITORINFOEX CreateWritable()
            => new()
            {
#if !NET472
                cbSize = Marshal.SizeOf<MONITORINFOEX>(),
#else
                cbSize = Marshal.SizeOf(typeof(MONITORINFOEX)),
#endif
                rcMonitor = new RECT(),
                rcWork = new RECT(),
                dwFlags = 0
            };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WNDCLASSEX
    {
        public int cbSize;
        public uint style;
        public IntPtr lpfnWndProc; // not WndProc
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public IntPtr lpszMenuName;
        public IntPtr lpszClassName;
        public IntPtr hIconSm;
    }
}
