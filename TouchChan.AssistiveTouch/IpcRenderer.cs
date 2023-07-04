using System.IO;
using System.IO.Pipes;

namespace TouchChan.AssistiveTouch
{
    internal class IpcRenderer
    {
        private static AnonymousPipeClientStream PipeClient = null!;

        public IpcRenderer(AnonymousPipeClientStream pipeClient)
        {
            PipeClient = pipeClient;
        }

        public static void Send(string channel)
        {
            // bug?
            using var sw = new StreamWriter(PipeClient);
            sw.AutoFlush = true;
            sw.WriteLine(channel);
        }
    }
}
