using System.Runtime.InteropServices;

namespace SplashScreenGdip
{
    internal partial class Gdiplus
    {
        public const int GpStatus_Ok = 0;

        private const string GdiplusDll = "gdiplus.dll";

        // Startup, init 

        [LibraryImport(GdiplusDll, SetLastError = true)]
        public static partial int GdiplusStartup(out nuint token, ref GdiplusStartupInput input, out GdiplusStartupOutput output);

        [StructLayout(LayoutKind.Sequential)]
        internal struct GdiplusStartupOutput
        {
            public nint NotificationHook;
            public nint NotificationUnhook;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct GdiplusStartupInput
        {
            public uint GdiplusVersion;
            public nint DebugEventCallback;
            public int SuppressBackgroundThread; // bool
            public int SuppressExternalCodecs;   // bool
        }

        // Load

        [LibraryImport(GdiplusDll, SetLastError = true)]
        public static partial int GdipLoadImageFromFile([MarshalAs(UnmanagedType.LPWStr)] string filename, out nint image);

        [LibraryImport(GdiplusDll, SetLastError = true)]
        public static partial int GdipLoadImageFromStream(nint stream, out nint image);

        // Draw 

        [LibraryImport(GdiplusDll, SetLastError = true)]
        public static partial int GdipCreateFromHDC(nint hdc, out nint graphics);

        [LibraryImport(GdiplusDll, SetLastError = true)]
        public static partial int GdipDrawImageRectI(nint graphics, nint image, int x, int y, int width, int height);

        // Release

        [LibraryImport(GdiplusDll, SetLastError = true)]
        public static partial void GdiplusShutdown(nuint token);

        // BLENDFUNCTION in gdiplus, anther one in gdi32

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct BLENDFUNCTION
        {
            public static readonly byte AC_SRC_OVER = 0;
            public static readonly byte AC_SRC_ALPHA = 1;
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }
    }
}
