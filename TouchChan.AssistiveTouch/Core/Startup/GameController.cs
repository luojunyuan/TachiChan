using System.Diagnostics;
using System.Windows;

namespace TouchChan.AssistiveTouch.Core.Startup;

internal class GameController
{
    public static void Start(string pipeHandle)
    {
#if !NET472
        var gamepad = "..\\TouchChan.AssistiveTouch.Gamepad\\TouchChan.AssistiveTouch.Gamepad.exe";
        var pid = Environment.ProcessId;
#else
        var gamepad = "TouchChan.AssistiveTouch.Gamepad.exe";
        var pid = Process.GetCurrentProcess().Id;
#endif
        try
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = gamepad,
                Arguments = pipeHandle + " " + pid + " " + App.GameWindowHandle + " " + App.GameEngine,
#if NET472
                UseShellExecute = false // unless handle would fail
#endif
            });
        }
        catch (SystemException ex)
        {
            MessageBox.Show($"Error while Launching TouchChan.AssistiveTouch.Gamepad.exe{Environment.NewLine}" +
                ex.Message,
                "TachiChan");
            return;
        }
    }
}
