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


        Application.Run(new Form());
    }
}