using System.IO.Pipes;

namespace TouchChan;

/// <summary>
/// For listen to splash close signal
/// </summary>
internal class IpcMain
{
    private readonly AnonymousPipeServerStream _serverIn;

    public IpcMain(AnonymousPipeServerStream serverIn)
    {
        _serverIn = serverIn;
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
                var channel = sr.ReadLine();
                if (channel == "Loaded" && DictionaryOfEvents.ContainsKey("Loaded"))
                {
                    DictionaryOfEvents["Loaded"].Invoke();
                    DictionaryOfEvents.Remove("Loaded");
                    break;
                }
            }
        }, TaskCreationOptions.LongRunning);
    }


    private static readonly Dictionary<string, Action> DictionaryOfEvents = [];

    public static void Once(string channel, Action callback)
    {
        DictionaryOfEvents.Add(channel, callback);
    }
}
