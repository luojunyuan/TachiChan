using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using TouchChan.AssistiveTouch.Gesture.Input;

namespace TouchChan.AssistiveTouch.Gesture;

internal static class Program
{
    public static bool UiAccess { get; private set; }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        var pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, args[0]);
        var parent = Process.GetProcessById(int.Parse(args[1]));
        UIAccess = bool.Parse(args[2]);
        parent.EnableRaisingEvents = true;
        parent.Exited += (s, e) => Environment.Exit(0);

        ComWrappers.RegisterForMarshalling(WinFormsComInterop.WinFormsComWrappers.Instance);
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        PointCapture.Instance.Load();
        GestureManager.Instance.Load(PointCapture.Instance);
        using var sw = new StreamWriter(pipeClient);
        sw.AutoFlush = true;
        PointCapture.Instance.GestureRecognized += (_, e) => sw.WriteLine(e.GestureName + " " + e.FirstCapturedPoints.FirstOrDefault());

        Application.Run();
    }
}