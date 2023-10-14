using System.Drawing;
using System.Runtime.InteropServices;

namespace TouchChan.AssistiveTouch.NativeMethods
{
    internal partial class User32
    {
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void WinEventProc(IntPtr hWinEventHook, uint winEvent, IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MSLLHOOKSTRUCT
        {
            public Point pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width
            {
                get { return right - left; }
                set { right = value + left; }
            }

            public int Height
            {
                get { return bottom - top; }
                set { bottom = value + top; }
            }

            public Size Size
            {
                get { return new Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;

            public static readonly LASTINPUTINFO Default = new() { cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO)) };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STYLESTRUCT
        {
            public WindowStylesEx styleOld;
            public WindowStylesEx styleNew;
        }
    }

    internal partial class User32
    {
        public enum SetWindowPosFlags : uint
        {
            SWP_NOACTIVATE = 0x0010,
            SWP_NOMOVE = 0x0002,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040,
        }

        [Flags]
        public enum WINEVENT
        {
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNTHREAD = 1,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_INCONTEXT = 4,
        }

        public enum WindowLongFlags
        {
            GWL_EXSTYLE = -20,
            GWL_HINSTANCE = -6,
            GWL_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }

        [Flags]
        public enum WindowStylesEx : uint
        {
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_NOACTIVATE = 0x08000000,
        }

        [Flags()]
        public enum WindowStyles : uint
        {
            WS_CHILD = 0x40000000,
            WS_POPUP = 0x80000000u,
        }

        public enum HookType
        {
            WH_MOUSE = 7,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14 // Global only
        }

        [Flags]
        public enum MOUSEEVENTF
        {
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
        }

        public enum SystemMetric : int
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }

        public enum WindowMessage
        {
            WM_STYLECHANGING = 0x007C,
            WM_SYSCOMMAND = 0x0112,
        }

        public enum SysCommand
        {
            SC_CLOSE = 0xF060,
            SC_RESTORE = 0xF120,
            SC_MAXIMIZE = 0xF030,
        }
    }
}
