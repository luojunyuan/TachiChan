using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SplashScreenGdip;

public class SplashScreen
{

    private readonly string WindowClass;
    private readonly string WindowName;

    private readonly Stream? SplashResource;
    private readonly string SplashFilePath = string.Empty;

    private readonly int _imageWidth;
    private readonly int _imageHeight;


    public SplashScreen(int size, Stream imageResouce, string windowClassName = "Splash")
    {
        _imageWidth = _imageHeight = size;
        SplashResource = imageResouce;
        WindowClass = WindowName = windowClassName;
    }

    public SplashScreen(int width, int height, string imagePath, string windowClassName = "Splash")
    {
        _imageWidth = width;
        _imageHeight = height;
        SplashFilePath = imagePath;
        WindowClass = WindowName = windowClassName;
    }

    public bool IsClosed { get; private set; }

    public event EventHandler? Closed;

    public void Close()
    {
        if (!IsClosed)
        {
            User32.PostMessage(_window, User32.WindowMessage_WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            Closed?.Invoke(this, new());

            IsClosed = true;
        }
    }

    private nint _window;

    public void Run(int milliseconds = 0)
    {
        try
        {
            var hInstance = Process.GetCurrentProcess().Handle;

            User32.WindowProc _wndProcDelegate = WindowProc;
            User32.WNDCLASSEX wndClassEx = new()
            {
                cbSize = Marshal.SizeOf<User32.WNDCLASSEX>(),
                style = User32.WindowClassStyles_CS_DBLCLKS,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProcDelegate),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = hInstance,
                hIcon = User32.LoadIcon(hInstance, User32.SystemIcons_IDI_APPLICATION),
                hCursor = User32.LoadCursor(IntPtr.Zero, User32.IDC_ARROW),
                hbrBackground = User32.SystemColorIndex_COLOR_WINDOW,
                lpszMenuName = IntPtr.Zero,
                lpszClassName = Marshal.StringToHGlobalUni(WindowClass),
            };

            if (User32.RegisterClassEx(ref wndClassEx) == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            _window = User32.CreateWindowEx(
                User32.WindowStylesEx_WS_EX_LAYERED | User32.WindowStylesEx_WS_EX_NOACTIVATE,
                WindowClass,
                WindowName,
                User32.WindowStyles_WS_OVERLAPPED,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                User32.CW_USEDEFAULT,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);
            
            if (_window == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            _windowDC = User32.GetDC(_window);

            SetPositionAndSize();

            if (IsClosed) // The best time to stop if Close() called before msg loop
                return;

            User32.ShowWindow(_window, User32.ShowWindowCommand_SW_SHOWNORMAL);
            
            // Gdi part

            var startupInput = new Gdiplus.GdiplusStartupInput
            {
                GdiplusVersion = 1,
                DebugEventCallback = IntPtr.Zero,
                SuppressBackgroundThread = 0, //false,
                SuppressExternalCodecs = 0, //false,
            };

            if (Gdiplus.GdiplusStartup(out _gdipToken, ref startupInput, out _) != Gdiplus.GpStatus_Ok)
                throw new Exception("GDI+ Error, please check if image is existed");

            int gpStatus = -1;

            if (SplashResource != null)
            {
#if !NET472
                using System.Drawing.DrawingCom.IStreamWrapper streamWrapper = 
                    System.Drawing.DrawingCom.GetComWrapper(new System.Drawing.Internal.GPStream(SplashResource));
                gpStatus = Gdiplus.GdipLoadImageFromStream(streamWrapper.Ptr, out _splashImage);
#else
                gpStatus = Gdiplus.GdipLoadImageFromStream(new GPStream(SplashResource), out _splashImage);
#endif
            }
            else if (SplashFilePath != string.Empty)
            {
                gpStatus = Gdiplus.GdipLoadImageFromFile(SplashFilePath, out _splashImage);
            }

            if (gpStatus != 0)
            {
                throw new Exception("GDI+ Error, please check if image is existed");
            }

            if (milliseconds > 0)
            {
                var timer = new System.Timers.Timer(milliseconds);
                timer.Elapsed += (s, e) => Close();
                timer.AutoReset = false;
                timer.Start();
            }
            
            DrawImage();
            
            try
            {
                User32.SetWindowPos(_window, User32.HWND_TOPMOST, 0, 0, 0, 0, 0x0001 | 0x0002);
                while (User32.GetMessage(out var msg, IntPtr.Zero, 0, 0) != false)
                {
                    User32.TranslateMessage(msg);
                    User32.DispatchMessage(msg);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        finally
        {
            ReleaseAllResource();
        }
    }

    private RECT _windowRectangle;

    private SIZE _windowSize;

    private nint _windowDC;

    private nint _memoryDC = IntPtr.Zero;

    private nint _splashImage;

    private nint _graphics;

    private nint _memoryBitmap = IntPtr.Zero;

    private nuint _gdipToken = UIntPtr.Zero;

    private nint WindowProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        try
        {
            switch (msg)
            {
                case User32.WindowMessage_WM_DESTROY:
                    User32.PostQuitMessage(0);
                    return IntPtr.Zero;
                case User32.WindowMessage_WM_DPICHANGED:
                    SetPositionAndSize(wParam.ToInt32() >> 16); //ToUInt32
                    DrawImage();
                    return IntPtr.Zero;
                case User32.WindowMessage_WM_NCHITTEST:
                    var result = User32.DefWindowProc(hWnd, msg, wParam, lParam);
                    return result == (IntPtr)User32.HitTestValues_HTCAPTION ? User32.HitTestValues_HTNOWHERE : result;
                default:
                    return User32.DefWindowProc(hWnd, msg, wParam, lParam);
            }
        }
        catch
        {
            //The finally in Main won't run if exception is thrown in this method.
            //This may be because this method was called by system code.
            //So we must handle exception here.
            User32.DestroyWindow(_window);
            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// Set Window Position And Size
    /// </summary>
    /// <param name="dpi"></param>
    private void SetPositionAndSize(int dpi = 0)
    {
        var screenLeft = 0;
        var screenTop = 0;
        var screenWidth = User32.GetSystemMetrics(User32.SystemMetric_SM_CXSCREEN);
        var screenHeight = User32.GetSystemMetrics(User32.SystemMetric_SM_CYSCREEN);

        var monitor = User32.MonitorFromWindow(_window, User32.MonitorFlags_MONITOR_DEFAULTTONULL);
        if (monitor != IntPtr.Zero)
        {
            var info = User32.MONITORINFOEX.CreateWritable();
            if (User32.GetMonitorInfo(monitor, ref info))
            {
                screenLeft = info.rcMonitor.left;
                screenTop = info.rcMonitor.top;
                screenWidth = info.rcMonitor.right - info.rcMonitor.left;
                screenHeight = info.rcMonitor.bottom - info.rcMonitor.top;
            }
        }

        if (dpi == 0)
        {
            var osVersion = Environment.OSVersion.Version;
            if (osVersion > new Version(10, 0, 1607))
                dpi = (int)User32.GetDpiForWindow(_window);
            else
            {
                dpi = Gdi32.GetDeviceCaps(_windowDC, Gdi32.DeviceCap_LOGPIXELSX);
            }
        }

        var windowWidth = _imageWidth * dpi / 96;
        var windowHeight = _imageHeight * dpi / 96;

        User32.SetWindowPos(
            _window,
            User32.HWND_TOPMOST,
            (screenWidth - windowWidth) / 2 + screenLeft,
            (screenHeight - windowHeight) / 2 + screenTop,
            windowWidth, windowHeight,
            0);

        User32.GetWindowRect(_window, out _windowRectangle);

        _windowSize = new SIZE
        {
            cx = windowWidth, //_windowRectangle.right - _windowRectangle.left,
            cy = _windowRectangle.bottom - _windowRectangle.top
        };
    }

    /// <summary>
    /// Screen DC
    /// </summary>
    private readonly nint _screenDC = User32.GetDC(IntPtr.Zero);

    /// <summary>
    /// Draw Image
    /// </summary>
    private void DrawImage()
    {
        if (_memoryDC != IntPtr.Zero)
            Gdi32.DeleteDC(_memoryDC);
        if (_memoryBitmap != IntPtr.Zero)
            Gdi32.DeleteObject(_memoryBitmap);

        _memoryDC = Gdi32.CreateCompatibleDC(_windowDC);
        _memoryBitmap = Gdi32.CreateCompatibleBitmap(_windowDC, _windowSize.cx, _windowSize.cy);
        Gdi32.SelectObject(_memoryDC, _memoryBitmap);

        if (Gdiplus.GdipCreateFromHDC(_memoryDC, out _graphics) == Gdiplus.GpStatus_Ok &&
            Gdiplus.GdipDrawImageRectI(_graphics, _splashImage, 0, 0, _windowSize.cx, _windowSize.cy) == Gdiplus.GpStatus_Ok)
        {
            var ptSrc = new POINT
            {
                X = 0,
                Y = 0,
            };
            var ptDes = new POINT
            {
                X = _windowRectangle.left,
                Y = _windowRectangle.top,
            };
            var blendFunction = new Gdi32.BLENDFUNCTION
            {
                AlphaFormat = Gdiplus.BLENDFUNCTION.AC_SRC_ALPHA,
                BlendFlags = 0,
                BlendOp = Gdiplus.BLENDFUNCTION.AC_SRC_OVER,
                SourceConstantAlpha = 255,
            };

            // Indeed show window
            if (User32.UpdateLayeredWindow(
                _window,
                _screenDC,
                 ptDes,
                 _windowSize,
                 _memoryDC,
                 ptSrc,
                 0,
                 blendFunction,
                 User32.UpdateLayeredWindowFlags_ULW_ALPHA))
            {
                return;
            }
            else
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    /// <summary>
    /// Release All Resource
    /// </summary>
    private void ReleaseAllResource()
    {
        if (_gdipToken != UIntPtr.Zero)
            Gdiplus.GdiplusShutdown(_gdipToken);
        Gdi32.DeleteObject(_memoryBitmap);
        Gdi32.DeleteDC(_memoryDC);
        User32.ReleaseDC(_window, _windowDC);
        User32.ReleaseDC(IntPtr.Zero, _screenDC);
    }
}

public static class Extension
{
    public static int ToInt32(this nint nativeValue) => (int)nativeValue;
}