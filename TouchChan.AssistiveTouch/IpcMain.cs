using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
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
            // front window check ?
            const int UserTimerMinimum = 0x0000000A;

            DictionaryOfEvents.Add(ChannelName.TwoFingerTap, p =>
            {
                Task.Run(() =>
                {
                    User32.SetCursorPos((int)p.X, (int)p.Y);
                    User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_RIGHTDOWN, (int)p.X, (int)p.Y, 0, IntPtr.Zero);
                    Thread.Sleep(UserTimerMinimum);
                    User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_RIGHTUP, (int)p.X, (int)p.Y, 0, IntPtr.Zero);
                });
            });
            DictionaryOfEvents.Add(ChannelName.ThreeFingerTap, _ =>
            {
                // not work for くれよんちゅーりっぷ
                Task.Run(() =>
                {
                    User32.keybd_event(0x20, 0, 0, IntPtr.Zero);
                    Thread.Sleep(UserTimerMinimum);
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
