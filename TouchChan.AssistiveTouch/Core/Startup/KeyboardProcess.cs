using System.Diagnostics;

namespace TouchChan.AssistiveTouch.Core.Startup;

internal class KeyboardProcess
{
    public static async Task<Process?> StartAsync()
    {
        Process? process = null;
        await Task.Run(() => 
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
                process = Process.Start(new ProcessStartInfo()
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
                process = null;
            }
        });

        return process;
    }
}
