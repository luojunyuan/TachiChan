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
        var w = new MessageWindow();
        w.PointsIntercepted += (_, _) => Debug.WriteLine("sss");
        Application.Run(new Form());
    }
}