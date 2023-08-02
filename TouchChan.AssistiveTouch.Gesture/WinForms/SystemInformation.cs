#if !NET472
using System.Runtime.InteropServices;

namespace TouchChan.AssistiveTouch.Gesture.WinForms
{
    public enum ScreenOrientation
    {
        Angle0 = 0,
        Angle90 = 1,
        Angle180 = 2,
        Angle270 = 3,
    }
    internal class SystemInformation
    {
        public static unsafe ScreenOrientation ScreenOrientation
        {
            // Alternative https://github.com/dotnet/winforms/blob/db612728612916764ad51f0391f57bcdaed61689/src/System.Windows.Forms/src/System/Windows/Forms/SystemInformation.cs#L755

            get
            {
                ScreenOrientation result = ScreenOrientation.Angle0;
                DEVMODE lpDevMode = default;
                lpDevMode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
                lpDevMode.dmDriverExtra = 0;
                try
                {
                    EnumDisplaySettings(null, -1, ref lpDevMode);
                    if ((lpDevMode.dmFields & 0x80) > 0)
                    {
                        result = lpDevMode.dmDisplayOrientation;
                        return result;
                    }

                    return result;
                }
                catch
                {
                    return result;
                }
            }
        }

        private const int DM_DISPLAYORIENTATION = 8388608;

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    }
}
#endif