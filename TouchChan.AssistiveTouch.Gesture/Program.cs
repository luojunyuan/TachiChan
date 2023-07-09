using System.Diagnostics;
using TouchChan.AssistiveTouch.Gesture.Input;

namespace TouchChan.AssistiveTouch.Gesture;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        PointCapture.Instance.Load();
        GestureManager.Instance.Load(PointCapture.Instance);
        PointCapture.Instance.GestureRecognized += (_, e) => Debug.WriteLine(e.GestureName);

        Application.Run();
    }
}