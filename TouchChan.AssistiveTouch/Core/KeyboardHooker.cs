using System.ComponentModel;
using System.Runtime.InteropServices;
using TouchChan.AssistiveTouch.NativeMethods;

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

            _hookId = User32.SetWindowsHookEx(User32.HookType.WH_KEYBOARD_LL, Hook, moduleHandle, 0); // tid 0 set global hook
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

            if (info.vkCode == (uint)Config.MappingKey && User32.GetForegroundWindow() == _gameWindowHandle)
            {
                const int WM_KEYUP = 0x0101;
                if ((int)wParam == WM_KEYUP)
                {
                    Simulate.Up(Simulate.KeyCode.Return);
                }
                else
                {
                    Simulate.Down(Simulate.KeyCode.Return);
                }

                return new IntPtr(1);
            }

            return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

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
    }
}
