using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TouchChan.AssistiveTouch.Core
{
    internal class KeyboardHooker
    {
        private static nint _hookId;
        private static nint _gameWindowHandle;

        public static void Install(nint gameWindowHandle)
        {
            _gameWindowHandle = gameWindowHandle;
            var moduleHandle = Kernel32.GetModuleHandle(); // get current exe instant handle

            _hookId = User32.SetWindowsHookEx(User32.HookType_WH_KEYBOARD_LL, Hook, moduleHandle, 0); // tid 0 set global hook
            if (_hookId == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static void UnInstall() => User32.UnhookWindowsHookEx(_hookId);

        private static nint Hook(int nCode, nint wParam, nint lParam)
        {
            if (nCode < 0)
                return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);

            var obj = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            if (obj is not KBDLLHOOKSTRUCT info)
                return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);

            const int KEY_Z = 0x5A;
            if (info.vkCode == KEY_Z && User32.GetForegroundWindow() == _gameWindowHandle)
            {
                const int WM_KEYUP = 0x0101;
                const int KEYBOARDMANAGER_SINGLEKEY_FLAG = 0x11;
                var keyEventList = new INPUT[1];
                if ((int)wParam == WM_KEYUP)
                {
                    SetKeyEvent(keyEventList, KeyCode.RETURN, KeyboardFlag.KeyUp, KEYBOARDMANAGER_SINGLEKEY_FLAG);
                }
                else
                {
                    SetKeyEvent(keyEventList, KeyCode.RETURN, 0, KEYBOARDMANAGER_SINGLEKEY_FLAG);
                }

                SendInput(1, keyEventList, INPUT.Size);
                return new IntPtr(1);
            }

            return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public static void SetKeyEvent(INPUT[] keyEventArray, KeyCode keyCode, KeyboardFlag flags, nuint extraInfo)
        {
            var i = keyEventArray
                .Select((input, index) => (input, index))
                .First(x => x.input.Data.Keyboard.KeyCode == default).index;
            keyEventArray[i].Type = InputType.Keyboard;
            keyEventArray[i].Data.Keyboard.KeyCode = keyCode;
            keyEventArray[i].Data.Keyboard.Flags = flags;
            keyEventArray[i].Data.Keyboard.ExtraInfo = extraInfo;

            const int MAPVK_VK_TO_VSC = 0;
            keyEventArray[i].Data.Keyboard.ScanCode = (ushort)MapVirtualKey((int)keyCode, MAPVK_VK_TO_VSC);
        }

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        // 仮想キーコードをスキャンコードに変換
        [DllImport("user32.dll")]
        private extern static int MapVirtualKey(int wCode, int wMapType);

        [Flags]
        private enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80,
        }

        [StructLayout(LayoutKind.Sequential)]
        private class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public nuint dwExtraInfo;
        }

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
            EXTENDEDKEY = 0x0001,
            KeyUp = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        internal enum KeyCode : short
        {
            RETURN = 0x0D,
            SPACE = 0x20,
        }

        private static class Kernel32
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern nint GetModuleHandle([Optional] string lpModuleName);
        }

        private static class User32
        {
            public const int HookType_WH_KEYBOARD_LL = 13;

            public delegate nint HookProc(int nCode, nint wParam, nint lParam);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern nint SetWindowsHookEx(int idHook, HookProc lpfn, nint hmod, int dwThreadId);

            [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern bool UnhookWindowsHookEx(nint hhk);

            [DllImport("user32.dll", SetLastError = false, ExactSpelling = true)]
            public static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

            [DllImport("user32.dll")]
            public static extern nint GetForegroundWindow();
        }
    }
}
