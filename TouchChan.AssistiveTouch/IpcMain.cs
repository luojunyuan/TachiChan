using System.IO;
using System.IO.Pipes;
using System.Windows;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch
{
    internal class IpcMain
    {
        private readonly AnonymousPipeServerStream _serverIn;

        public IpcMain(AnonymousPipeServerStream serverIn)
        {
            _serverIn = serverIn;
            // front window check ? yes
            const int SendKeyBlock = 50;

            // KeyboardHooker z键在くれよんちゅーりっぷ里不工作，金恋里是空格，其他正常，体验良好
            // 下面的键盘输入在くれよんちゅーりっぷ里不工作
            DictionaryOfEvents.Add(ChannelName.TwoFingerTap, p =>
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
            });
            DictionaryOfEvents.Add(ChannelName.ThreeFingerTap, _ =>
            {
                if (User32.GetForegroundWindow() != App.GameWindowHandle)
                    return;
                Task.Run(() =>
                {
                    User32.keybd_event(0x20, 0, 0, IntPtr.Zero);
                    Thread.Sleep(SendKeyBlock);
                    User32.keybd_event(0x20, 0, 0x0002, IntPtr.Zero);
                });
            });
            DictionaryOfEvents.Add(ChannelName.PointDown, _ => MessageBox.Show("Down"));
            DictionaryOfEvents.Add(ChannelName.PointUp, _ => MessageBox.Show("Up"));
            Start();
        }

        private void Start()
        {
            new TaskFactory().StartNew(() =>
            {
                Thread.CurrentThread.Name = "IpcMain listening loop";
                var sr = new StreamReader(_serverIn);
                while (true)
                {
                    var input = (sr.ReadLine() ?? "Undefine {X=0,Y=0}").Split(' ');
                    var channel = Enum.Parse<ChannelName>(input[0]);
                    var point = ExtractPoint(input[1]);
                    if (channel == ChannelName.Undefine)
                        continue;

                    DictionaryOfEvents[channel].Invoke(point);
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static readonly Dictionary<ChannelName, Action<Point>> DictionaryOfEvents = new();

        public enum ChannelName
        {
            TwoFingerTap,
            ThreeFingerTap,
            PointDown,
            PointUp,
            Undefine
        }

        static Point ExtractPoint(string input)
        {
            string[] parts = input.Replace("{", "").Replace("}", "").Split(',');
            string[] xParts = parts[0].Split('=');
            string[] yParts = parts[1].Split('=');

            int x = int.Parse(xParts[1]);
            int y = int.Parse(yParts[1]);

            return new Point(x, y);
        }
    }
}
