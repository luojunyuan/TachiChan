using Microsoft.Win32;
using SplashScreenGdip;
using System.Diagnostics;
using System.IO.Pipes;

namespace TouchChan;

static class TouchLauncher
{
    public static void Run(Process game, SplashScreen? splash = null)
    {
        var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        _ = new IpcMain(pipeServer);

        if (splash != null) IpcMain.Once("Loaded", () =>
        {
            splash!.Close();
            splash = null;
        });

        // The small device means dpi settings did not make the item large enough for the device
        var smallDevice = Environment.GetCommandLineArgs().Contains("--small-device");// || RegistryModifier.IsSmallDevice();

        Environment.CurrentDirectory = AppContext.BaseDirectory;
        Process? touch = null;
        // SystemEvents.DisplaySettingsChanged += (_, _) => touch?.Kill();

        while (!game.HasExited)
        {
            var gameWindowHandle = AppLauncher.FindMainWindowHandle(game);
            if (gameWindowHandle == IntPtr.Zero) // process exit
            {
                if (splash != null)
                {
                    splash.Close();
                    MessageBox.Show(Strings.App_Unexpected);
                }
                break;
            }
            else if (gameWindowHandle.ToInt32() == -1) // FindHandleFailed
            {
                splash?.Close();
                MessageBox.Show(Strings.App_Timeout);
                break;
            }

            // Tip: Make sure AssistiveTouch exist when you debuging
            touch = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
#if !NET472
                    FileName = "..\\TouchChan.AssistiveTouch\\TouchChan.AssistiveTouch.exe",
#else
                    FileName = "TouchChan.AssistiveTouch.exe",
#endif
                    Arguments = pipeServer.GetClientHandleAsString() + ' ' + gameWindowHandle,
                    UseShellExecute = false,
                }
            };

            touch.Start();

            touch.WaitForExit();
            touch = null;
        }

        // prevent exception when startup
        splash?.Close();
    }
}
