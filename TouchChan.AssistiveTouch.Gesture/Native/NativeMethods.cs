﻿using System.Runtime.InteropServices;

namespace TouchChan.AssistiveTouch.Gesture.Native;

internal static class NativeMethods
{
    #region const definitions

    internal const int

        WM_PARENTNOTIFY = 0x0210;

    internal const int

        WM_NCPOINTERUPDATE = 0x0241;

    internal const int

        WM_NCPOINTERDOWN = 0x0242;

    internal const int

        WM_NCPOINTERUP = 0x0243;

    internal const int

        WM_POINTERUPDATE = 0x0245;

    internal const int

        WM_POINTERDOWN = 0x0246;

    internal const int

        WM_POINTERUP = 0x0247;

    internal const int

        WM_POINTERENTER = 0x0249;

    internal const int

        WM_POINTERLEAVE = 0x024A;

    internal const int

        WM_POINTERACTIVATE = 0x024B;

    internal const int

        WM_POINTERCAPTURECHANGED = 0x024C;

    internal const int

        WM_POINTERWHEEL = 0x024E;

    internal const int

        WM_POINTERHWHEEL = 0x024F;

    internal const uint ANRUS_TOUCH_MODIFICATION_ACTIVE = 0x0000002;

    internal const int RIDEV_REMOVE = 0x00000001;
    internal const int RIDEV_INPUTSINK = 0x00000100;
    internal const int RIDEV_DEVNOTIFY = 0x00002000;
    internal const int RID_INPUT = 0x10000003;

    internal const int RIM_TYPEHID = 2;

    internal const uint RIDI_DEVICENAME = 0x20000007;
    internal const uint RIDI_DEVICEINFO = 0x2000000b;
    internal const uint RIDI_PREPARSEDDATA = 0x20000005;

    internal const int WM_KEYDOWN = 0x0100;
    internal const int WM_SYSKEYDOWN = 0x0104;
    internal const int WM_INPUT = 0x00FF;
    internal const int WM_INPUT_DEVICE_CHANGE = 0x00FE;
    internal const int VK_OEM_CLEAR = 0xFE;
    internal const int VK_LAST_KEY = VK_OEM_CLEAR; // this is a made up value used as a sentinal

    internal const int WmClose = 0x0010;

    internal const ushort GenericDesktopPage = 0x01;
    internal const ushort DigitizerUsagePage = 0x0D;
    internal const ushort ContactIdentifierId = 0x51;
    internal const ushort ContactCountId = 0x54;
    internal const ushort ScanTimeId = 0x56;
    internal const ushort TipId = 0x42;
    internal const ushort XCoordinateId = 0x30;
    internal const ushort YCoordinateId = 0x31;
    internal const ushort InRangeId = 0x32;
    internal const ushort BarrelButtonId = 0x44;
    internal const ushort InvertId = 0x3C;
    internal const ushort EraserId = 0x45;

    internal const ushort TouchPadUsage = 0x05;
    internal const ushort TouchScreenUsage = 0x04;
    internal const ushort PenUsage = 0x02;

    #endregion const definitions

    #region DllImports


    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern nint PostMessage(HandleRef hwnd, int msg, int wparam, int lparam);

    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    internal static extern int GetWindowThreadProcessId(HandleRef hWnd, out int lpdwProcessId);

    [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    internal static extern int GetCurrentThreadId();

    [DllImport("user32.dll")]
    internal static extern nint SetParent(nint hWndChild, nint hWndNewParent);

    [DllImport("User32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterPointerInputTarget(nint handle, POINTER_INPUT_TYPE pointerType);

    [DllImport("User32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterPointerInputTarget(nint hwnd, POINTER_INPUT_TYPE pointerType);

    [DllImport("Oleacc.dll")]
    internal static extern int AccSetRunningUtilityState(nint hWnd, uint dwUtilityStateMask, uint dwUtilityState);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool GetPointerFrameInfo(int pointerID, ref int pointerCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_INFO[] pointerInfo);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool InitializeTouchInjection(int maxCount, TOUCH_FEEDBACK feedbackMode);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool InjectTouchInput(int count, [MarshalAs(UnmanagedType.LPArray), In] POINTER_TOUCH_INFO[] contacts);

    [DllImport("User32.dll")]
    internal static extern uint GetRawInputData(nint hRawInput, uint uiCommand, nint pData, ref uint pcbSize, uint cbSizeHeader);

    [DllImport("User32.dll")]
    internal static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

    [DllImport("User32.dll")]
    internal static extern uint GetRawInputDeviceList(nint pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

    [DllImport("User32.dll")]
    internal static extern uint GetRawInputDeviceInfo(nint hDevice, uint uiCommand, nint pData, ref uint pcbSize);

    [DllImport("User32.dll")]
    public static extern nint MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

    [DllImport("Shcore.dll")]
    public static extern nint GetDpiForMonitor([In] nint hmonitor, [In] MonitorDpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);


    #endregion DllImports

    private const string User32Dll = "user32.dll";

    /// <summary>
    /// Represents the different types of scaling.
    /// </summary>
    /// <seealso cref="https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511.aspx"/>
    public enum MonitorDpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }

    [Flags]
    public enum SWP
    {
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_NOACTIVATE = 0x0010,
        SWP_FRAMECHANGED = 0x0020, /* The frame changed: send WM_NCCALCSIZE */
        SWP_SHOWWINDOW = 0x0040,
        SWP_HIDEWINDOW = 0x0080,
        SWP_NOOWNERZORDER = 0x0200, /* Don't do owner Z ordering */
        SWP_DRAWFRAME = SWP_FRAMECHANGED,
        SWP_NOREPOSITION = SWP_NOOWNERZORDER,
        SWP_NOSTARTUP = 0x04000000,
        SWP_STARTUP = 0x08000000
    }

    [DllImport(User32Dll)]
    public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy,
        SWP uFlags);

    [DllImport(User32Dll)]
    public static extern bool UpdateWindow(nint hwnd);

    [DllImport(User32Dll)]
    public static extern nint DispatchMessage([In] ref MSG lpmsg);

    [DllImport(User32Dll)]
    public static extern bool GetMessage(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport(User32Dll)]
    public static extern bool TranslateMessage([In] ref MSG lpMsg);

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public nint hwnd;
        public uint message;
        public nint wParam;
        public nint lParam;
        public uint time;
        public Point pt;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        /// <summary>
        /// A BITMAPINFOHEADER structure that contains information about the dimensions of color format.
        /// </summary>
        public BITMAPINFOHEADER bmiHeader;

        /// <summary>
        /// An array of RGBQUAD. The elements of the array that make up the color table.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
        public
            RGBQUAD[] bmiColors;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    }

    #region GDI

    [DllImport("gdi32.dll")]
    public static extern nint CreateDIBSection(nint hdc, [In] ref BITMAPINFO pbmi,
        uint pila, out nint ppvBits, nint hSection, uint dwOffset);

    [DllImport("gdi32.dll")]
    public static extern int GetObject(nint hgdiobj, int cbBuffer, nint lpvObject);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    public static extern nint CreateCompatibleBitmap([In] nint hdc, int nWidth, int nHeight);

    [DllImport(User32Dll, ExactSpelling = true, SetLastError = true)]
    public static extern nint GetDC(nint hWnd);

    [DllImport(User32Dll, ExactSpelling = true)]
    public static extern int ReleaseDC(nint hWnd, nint hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern nint CreateCompatibleDC(nint hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteDC(nint hdc);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern nint SelectObject(nint hDC, nint hObject);

    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject([In] nint hObject);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport(User32Dll)]
    public static extern bool UpdateLayeredWindowIndirect(nint hwnd, ref UPDATELAYEREDWINDOWINFO pULWInfo);

    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public int cx;
        public int cy;

        public Size(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT // : IEquatable<RECT>
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left_, int top_, int right_, int bottom_)
        {
            Left = left_;
            Top = top_;
            Right = right_;
            Bottom = bottom_;
        }

        public void Shift(int x, int y)
        {
            Left += x;
            Right += x;
            Top += y;
            Bottom += y;
        }

        //public bool Equals(RECT other)
        //{
        //    return Left == other.Left &&
        //           Top == other.Top &&
        //           Right == other.Right &&
        //           Bottom == other.Bottom;
        //}

        //public static bool operator ==(RECT thiz, RECT other)
        //{
        //    return thiz.Equals(other);
        //}

        //public static bool operator !=(RECT thiz, RECT other)
        //{
        //    return !(thiz == other);
        //}

        public override string ToString()
        {
            return $"({Left}, {Top}, {Right}, {Bottom})";
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UPDATELAYEREDWINDOWINFO
    {
        public uint cbSize;
        public nint hdcDst;
        public unsafe Point* pptDst;
        public unsafe Size* psize;
        public nint hdcSrc;
        public unsafe Point* pptSrc;
        public uint crKey;
        public unsafe BLENDFUNCTION* pblend;
        public uint dwFlags;
        public unsafe RECT* prcDirty;
    }

    [DllImport("gdi32.dll")]
    public static extern int GetDeviceCaps(nint hdc, int nIndex);
}
