using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using TouchChan.AssistiveTouch.Gesture.Input;

namespace TouchChan.AssistiveTouch.Gesture;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        var pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, args[0]);
        var parent = Process.GetProcessById(int.Parse(args[1]));
        parent.EnableRaisingEvents = true;
        parent.Exited += (s, e) => Environment.Exit(0);

#if !NET472
        ComWrappers.RegisterForMarshalling(WinFormsComInterop.WinFormsComWrappers.Instance);
#endif
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        //ApplicationConfiguration.Initialize();
        SetProcessDPIAware(); // needed to fix mapping
        // or use manifest?

        PointCapture.Instance.Load();
        GestureManager.Instance.Load(PointCapture.Instance);
        using var sw = new StreamWriter(pipeClient);
        sw.AutoFlush = true;
        PointCapture.Instance.GestureRecognized += (_, e) => sw.WriteLine(e.GestureName + " " + e.FirstCapturedPoints.FirstOrDefault());

        Application.Run();
    }

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();
}

#if NET472
internal static partial class ApplicationConfiguration
{
    public static void Initialize()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false); // error net472 release
        // Set dpi aware in manifest instead
        // Application.SetHighDpiMode(HighDpiMode.SystemAware);
    }
}
#endif