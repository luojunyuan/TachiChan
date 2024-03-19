using System.Diagnostics;

namespace TouchChan.AssistiveTouch.Core.Startup;

internal class KeyboardProcess
{
    public static Process? Start()
    {
#if !NET472
        var keyboard = "..\\TouchChan.AssistiveTouch.VirtualKeyboard\\TouchChan.AssistiveTouch.VirtualKeyboard.exe";
        var pid = Environment.ProcessId;
#else
        var keyboard = "TouchChan.AssistiveTouch.VirtualKeyboard.exe";
        var pid = Process.GetCurrentProcess().Id;
#endif
        try
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = keyboard,
                Arguments = App.GameWindowHandle + " " + pid,
#if NET472
                UseShellExecute = false // unless handle would fail
#endif
            });
        }
        catch (SystemException)
        {
            return null;
        }
    }
}
