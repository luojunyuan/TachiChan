using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Core
{
    internal static class TouchGestureHooker
    {
        public static void Start(string pipeHandle, int pid)
        {
#if !NET472
            var gestureHooker = "..\\TouchChan.AssistiveTouch.Gesture\\TouchChan.AssistiveTouch.Gesture.exe";
#else
            var gestureHooker = "TouchChan.AssistiveTouch.Gesture.exe";
#endif
            if (!File.Exists(gestureHooker))
            {
                MessageBox.Show("TouchChan.AssistiveTouch.Gesture.exe not exist.", "TachiChan");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = gestureHooker,
                    Arguments = pipeHandle + " " + pid,
                });
            }
            catch (SystemException ex)
            {
                MessageBox.Show($"Error with Launching TouchChan.AssistiveTouch.Gesture.exe{Environment.NewLine}" +
                    ex.Message,
                    "TachiChan");
                return;
            }
        }

        private const int SendKeyBlock = 50;
        public static void SendRightClick(Point p)
        {
            if (User32.GetForegroundWindow() != App.GameWindowHandle)
                return;
            Task.Run(() =>
            {
                User32.SetCursorPos((int)p.X, (int)p.Y);
                User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_RIGHTDOWN, (int)p.X, (int)p.Y, 0, IntPtr.Zero);
                Thread.Sleep(SendKeyBlock);
                User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_RIGHTUP, (int)p.X, (int)p.Y, 0, IntPtr.Zero);
            });
        }

        public static void SendSpaceKey()
        {
            if (User32.GetForegroundWindow() != App.GameWindowHandle)
                return;
            Task.Run(() =>
            {
                const int KEYBOARDMANAGER_SINGLEKEY_FLAG = 0x11;
                var keyEventList = new KeyboardHooker.INPUT[1];
                KeyboardHooker.SetKeyEvent(keyEventList, KeyboardHooker.KeyCode.SPACE, 0, KEYBOARDMANAGER_SINGLEKEY_FLAG);
                KeyboardHooker.SendInput(1, keyEventList, KeyboardHooker.INPUT.Size);
                Thread.Sleep(0xA);
                var keyEventList2 = new KeyboardHooker.INPUT[1];
                KeyboardHooker.SetKeyEvent(keyEventList2, KeyboardHooker.KeyCode.SPACE, KeyboardHooker.KeyboardFlag.KeyUp, KEYBOARDMANAGER_SINGLEKEY_FLAG);
                KeyboardHooker.SendInput(1, keyEventList2, KeyboardHooker.INPUT.Size);
            });
        }
    }
}
