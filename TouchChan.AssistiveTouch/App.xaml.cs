using TouchChan.AssistiveTouch.NativeMethods;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace TouchChan.AssistiveTouch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IntPtr GameWindowHandle { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var _pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, e.Args[0]);
        _ = new IpcRenderer(_pipeClient);

        GameWindowHandle = int.Parse(e.Args[1]);

        Config.Load();

        User32.GetWindowThreadProcessId(GameWindowHandle, out var pid);
        var dir = Path.GetDirectoryName(Process.GetProcessById((int)pid).MainModule!.FileName);
        if (dir is not null && File.Exists(Path.Combine(dir, "RIO.INI"))) // Shinario
            return;

        DisableWPFTabletSupport();
    }

    private static void DisableWPFTabletSupport()
    {
        // Get a collection of the tablet devices for this window.
        TabletDeviceCollection devices = Tablet.TabletDevices;

        if (devices.Count > 0)
        {
            // Get the Type of InputManager.  
            Type inputManagerType = typeof(InputManager);

            // Call the StylusLogic method on the InputManager.Current instance.  
            object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                        null, InputManager.Current, null)!;

            if (stylusLogic != null)
            {
                //  Get the type of the stylusLogic returned from the call to StylusLogic.  
                Type stylusLogicType = stylusLogic.GetType();

                // Loop until there are no more devices to remove.  
                while (devices.Count > 0)
                {
                    // Remove the first tablet device in the devices collection.  
                    stylusLogicType.InvokeMember("OnTabletRemoved",
                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, stylusLogic, new object[] { (uint)0 });
                }
            }

        }
    }
}
