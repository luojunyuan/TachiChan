using System.Runtime.InteropServices;
using static TouchChan.AssistiveTouch.NativeMethods.Simulate;

namespace TouchChan.AssistiveTouch.NativeMethods
{
    internal partial class Simulate
    {
        const int KEYBOARDMANAGER_SINGLEKEY_FLAG = 0x11;

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        // 仮想キーコードをスキャンコードに変換
        [DllImport("user32.dll")]
        private extern static int MapVirtualKey(int wCode, int wMapType);

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            internal InputType Type;
            internal InputUnion Data;
            internal static int Size => Marshal.SizeOf<INPUT>();
        }

        public enum InputType : uint
        {
            INPUT_MOUSE,
            Keyboard,
            INPUT_HARDWARE
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT Keyboard;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal nuint dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal KeyCode KeyCode;
            internal ushort ScanCode;
            internal KeyboardFlag Flags;
            internal int time;
            internal nuint ExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }
        [Flags]
        internal enum MOUSEEVENTF : uint
        {
            ABSOLUTE = 0x8000,
            HWHEEL = 0x01000,
            MOVE = 0x0001,
            MOVE_NOCOALESCE = 0x2000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }
        [Flags]
        internal enum KeyboardFlag : uint
        {
            None = 0x00,
            EXTENDEDKEY = 0x01,
            KeyUp = 0x02,
            UNICODE = 0x04,
            SCANCODE = 0x08,
        }
    }

    // https://github1s.com/MediatedCommunications/WindowsInput/blob/master/WindowsInput/Events/Mouse/MouseMove.cs

    internal static class SimulateExtension
    {
        public static MOUSEEVENTF ToMouseButtonDownFlags(this ButtonCode This)
        {
            var ret = default(MOUSEEVENTF);
            switch (This)
            {
                case ButtonCode.Left:
                    ret = MOUSEEVENTF.LEFTDOWN;
                    break;
                case ButtonCode.Right:
                    ret = MOUSEEVENTF.RIGHTDOWN;
                    break;
                case ButtonCode.Middle:
                    ret = MOUSEEVENTF.MIDDLEDOWN;
                    break;
                case ButtonCode.XButton1:
                    ret = MOUSEEVENTF.XDOWN;
                    break;
                case ButtonCode.XButton2:
                    ret = MOUSEEVENTF.XDOWN;
                    break;
                default:
                    throw new InvalidCastException($@"Unable to convert {This} to a valid 'Down' state.  Valid inputs are: {ButtonCode.Left}, {ButtonCode.Right}, {ButtonCode.Middle}, {ButtonCode.XButton1}, and {ButtonCode.XButton2}.");
            }

            return ret;
        }

        public static MOUSEEVENTF ToMouseButtonUpFlags(this ButtonCode This)
        {
            var ret = default(MOUSEEVENTF);
            switch (This)
            {
                case ButtonCode.Left:
                    ret = MOUSEEVENTF.LEFTUP;
                    break;
                case ButtonCode.Right:
                    ret = MOUSEEVENTF.RIGHTUP;
                    break;
                case ButtonCode.Middle:
                    ret = MOUSEEVENTF.MIDDLEUP;
                    break;
                case ButtonCode.XButton1:
                    ret = MOUSEEVENTF.XUP;
                    break;
                case ButtonCode.XButton2:
                    ret = MOUSEEVENTF.XUP;
                    break;
                default:
                    throw new InvalidCastException($@"Unable to convert {This} to a valid 'Up' state.  Valid inputs are: {ButtonCode.Left}, {ButtonCode.Right}, {ButtonCode.Middle}, {ButtonCode.XButton1}, and {ButtonCode.XButton2}.");
            }

            return ret;
        }


        public static uint ToMouseButtonData(this ButtonCode This)
        {
            var ret = default(uint);

            if (This == ButtonCode.XButton1)
            {
                ret = 1;
            }
            else if (This == ButtonCode.XButton2)
            {
                ret = 2;
            }

            return ret;
        }

        public static MOUSEEVENTF ToMouseWheel(this ButtonCode This)
        {
            var ret = default(MOUSEEVENTF);

            switch (This)
            {
                case ButtonCode.HScroll:
                    ret = MOUSEEVENTF.HWHEEL;
                    break;
                case ButtonCode.VScroll:
                    ret = MOUSEEVENTF.WHEEL;
                    break;

                default:
                    throw new InvalidCastException($@"Unable to convert {This} to a Wheel.");
            }
            return ret;
        }
    }
}
