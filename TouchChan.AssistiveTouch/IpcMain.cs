using System.IO;
using System.IO.Pipes;
using System.Windows;

namespace TouchChan.AssistiveTouch
{
    internal class IpcMain
    {
        private readonly AnonymousPipeServerStream _serverIn;

        public IpcMain(AnonymousPipeServerStream serverIn)
        {
            _serverIn = serverIn;
            // front window check ?
            // get point click at first point
            // note not work for shirario
            // Space not work in some games
            DictionaryOfEvents.Add(ChannelName.TwoFingerTap, () => WindowsInput.Simulate.Events().Click(WindowsInput.Events.ButtonCode.Right).Invoke());
            DictionaryOfEvents.Add(ChannelName.ThreeFingerTap, () => WindowsInput.Simulate.Events().Click(WindowsInput.Events.KeyCode.Space).Invoke());
            DictionaryOfEvents.Add(ChannelName.PointDown, () => MessageBox.Show("Down"));
            DictionaryOfEvents.Add(ChannelName.PointUp, () => MessageBox.Show("Up"));
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
                    var channel = Enum.Parse<ChannelName>(sr.ReadLine() ?? "Undefine");
                    if (channel == ChannelName.Undefine)
                        continue;

                    DictionaryOfEvents[channel].Invoke();
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static readonly Dictionary<ChannelName, Action> DictionaryOfEvents = new();

        public enum ChannelName
        {
            TwoFingerTap,
            ThreeFingerTap,
            PointDown,
            PointUp,
            Undefine
        }
    }
}
