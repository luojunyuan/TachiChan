global using System.Drawing;
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

        PointCapture.Instance.Load();
        GestureManager.Instance.Load(PointCapture.Instance);
        using var sw = new StreamWriter(pipeClient);
        sw.AutoFlush = true;
        PointCapture.Instance.GestureRecognized += (_, e) => sw.WriteLine(e.GestureName + " " + e.FirstCapturedPoints.FirstOrDefault());

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0) != false)
        {
            TranslateMessage(msg);
            DispatchMessage(msg);
        }
    }

    [DllImport("user32.dll")]
    static extern bool GetMessage(out IntPtr lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll")]
    static extern bool TranslateMessage(in IntPtr lpMsg);
    [DllImport("user32.dll")]
    static extern IntPtr DispatchMessage(in IntPtr lpMsg);
}
