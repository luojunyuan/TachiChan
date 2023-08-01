using System.Diagnostics;
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

            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = gestureHooker,
                    Arguments = pipeHandle + " " + pid,
#if NET472
                    UseShellExecute = false // unless handle would fail
#endif
                });
            }
            catch (SystemException ex)
            {
                MessageBox.Show($"Error while Launching TouchChan.AssistiveTouch.Gesture.exe{Environment.NewLine}" +
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
                Simulate.Click(Simulate.ButtonCode.Right);
            });
        }

        public static void SendSpaceKey()
        {
            if (User32.GetForegroundWindow() != App.GameWindowHandle)
                return;
            Task.Run(() =>
            {
                const int KEYBOARDMANAGER_SINGLEKEY_FLAG = 0x11;
                var keyEventList = new Simulate.INPUT[1];
                Simulate.SetKeyEvent(0, keyEventList, Simulate.KeyCode.Space, 0, KEYBOARDMANAGER_SINGLEKEY_FLAG);
                Simulate.SendInput(1, keyEventList, Simulate.INPUT.Size);
                Thread.Sleep(0xA);
                var keyEventList2 = new Simulate.INPUT[1];
                Simulate.SetKeyEvent(0, keyEventList2, Simulate.KeyCode.Space, Simulate.KeyboardFlag.KeyUp, KEYBOARDMANAGER_SINGLEKEY_FLAG);
                Simulate.SendInput(1, keyEventList2, Simulate.INPUT.Size);
            });
        }
    }
}
