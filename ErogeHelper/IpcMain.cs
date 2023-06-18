using System.IO.Pipes;

namespace ErogeHelper;

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
                    break; // Im not use any other signals
                }
            }
        }, TaskCreationOptions.LongRunning);
    }


    private static readonly Dictionary<string, Action> DictionaryOfEvents = new();

    public static void Once(string channel, Action callback)
    {
        DictionaryOfEvents.Add(channel, callback);
    }
}
