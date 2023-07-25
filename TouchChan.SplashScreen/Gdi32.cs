using System.Runtime.InteropServices;

namespace SplashScreenGdip
{
    internal partial class Gdi32
    {
        public const int DeviceCap_LOGPIXELSX = 88;

        private const string Gdi32Lib = "gdi32.dll";

#if !NET472
        // Dpi compatible old system before win10 1067
        [LibraryImport(Gdi32Lib, SetLastError = true)]
        public static partial int GetDeviceCaps(nint hdc, int index);

        // The three to draw and create

        [LibraryImport(Gdi32Lib, SetLastError = true)]
        public static partial nint CreateCompatibleDC(nint hdc);

        [LibraryImport(Gdi32Lib, SetLastError = true)]
        public static partial nint CreateCompatibleBitmap(nint hdc, int cx, int cy);

        [LibraryImport(Gdi32Lib, SetLastError = true)]
        public static partial nint SelectObject(nint hDC, nint hObject);

        // Release resources

        [LibraryImport(Gdi32Lib, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DeleteDC(nint hdc);

        [LibraryImport(Gdi32Lib, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DeleteObject(nint hObject);
#else

        [DllImport(Gdi32Lib, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hdc, int index);

        [DllImport(Gdi32Lib, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [System.Security.SecurityCritical]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport(Gdi32Lib, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(Gdi32Lib, CharSet = CharSet.Unicode, EntryPoint = "CreateCompatibleDC", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        [DllImport(Gdi32Lib, SetLastError = false, ExactSpelling = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

        [DllImport(Gdi32Lib, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
#endif
        // BLENDFUNCTION in Gdi32, another one in Gdiplus. This one also used by User32.UpdateLayeredWindow
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
            public bool IsEmpty
            {
                get
                {
                    if (BlendOp == 0 && BlendFlags == 0 && AlphaFormat == 0)
                    {
                        return SourceConstantAlpha == 0;
                    }

                    return false;
                }
            }
            public BLENDFUNCTION(byte alpha)
            {
                BlendOp = 0;
                BlendFlags = 0;
                SourceConstantAlpha = alpha;
                AlphaFormat = 1;
            }
        }
    }
}
