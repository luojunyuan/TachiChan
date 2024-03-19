using System.IO;
using System.IO.Pipes;
using System.Windows;
using System.Windows.Controls;
using TouchChan.AssistiveTouch.Core.Extend;
using TouchChan.AssistiveTouch.Core.Startup;
using TouchChan.AssistiveTouch.NativeMethods;

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
            DictionaryOfEvents.Add(ChannelName.PointDown, 
                _ => Simulate.Scroll(Simulate.ButtonCode.VScroll, Simulate.ButtonScrollDirection.Up));
            DictionaryOfEvents.Add(ChannelName.PointUp, 
                _ => Simulate.Scroll(Simulate.ButtonCode.VScroll, Simulate.ButtonScrollDirection.Down));
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
                    var rawInput = sr.ReadLine();
                    
                    switch(rawInput)
                    {
                        // GamePad
                        case "OpenMenu":
                            GameController.InteractTip();
                            break;

                        // Gesture
                        default:
                            var input = (rawInput ?? "Undefine {X=0,Y=0}").Split(' ');
#if !NET472
                            var channel = Enum.Parse<ChannelName>(input[0]);
#else
                            var channel = (ChannelName)Enum.Parse(typeof(ChannelName), input[0]);
#endif
                            var point = ExtractPoint(input[1]);
                            if (channel == ChannelName.Undefine)
                                continue;

                            TouchGestureHooker.ShowTipAnimation(channel);
                            DictionaryOfEvents[channel].Invoke(point);
                            break;
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static readonly Dictionary<ChannelName, Action<Point>> DictionaryOfEvents = new();

        public enum ChannelName
        {
            Undefine,
            TwoFingerTap,
            ThreeFingerTap,
            PointDown,
            PointUp,
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
