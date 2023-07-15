using System.IO;
using System.IO.Pipes;
using System.Windows;
using TouchChan.AssistiveTouch.Core;

namespace TouchChan.AssistiveTouch
{
    internal class IpcMain
    {
        private readonly AnonymousPipeServerStream _serverIn;

        public IpcMain(AnonymousPipeServerStream serverIn)
        {
            _serverIn = serverIn;
            DictionaryOfEvents.Add(ChannelName.TwoFingerTap, TouchGestureHooker.SendRightClick);
            DictionaryOfEvents.Add(ChannelName.ThreeFingerTap, _ => TouchGestureHooker.SendSpaceKey());
            DictionaryOfEvents.Add(ChannelName.PointDown, _ => 
                WindowsInput.Simulate.Events().Scroll(WindowsInput.Events.ButtonCode.VScroll, WindowsInput.Events.ButtonScrollDirection.Up).Invoke());
            DictionaryOfEvents.Add(ChannelName.PointUp, _ => WindowsInput.Simulate.Events().Scroll(WindowsInput.Events.ButtonCode.VScroll, WindowsInput.Events.ButtonScrollDirection.Down).Invoke());
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
