using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using TouchChan.AssistiveTouch.Core.Extend;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IntPtr GameWindowHandle { get; private set; }

    public static Engine GameEngine { get; private set; }

    public static TouchStyle TouchStyle { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        // TouchConversion, Gesture, Gamepad, KeyMapping (non-interact functions)

        var _pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, e.Args[0]);
        _ = new IpcRenderer(_pipeClient);

        var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        _ = new IpcMain(pipeServer);

        GameWindowHandle = (IntPtr)int.Parse(e.Args[1]);

        if (e.Args.Contains("--small-device"))
            TouchButton.TouchSize = 120;

        var noDpiCompatibleSet = e.Args.Contains("--no-dpi-compatible");

        // I18N
        Resources.MergedDictionaries.Add(Helper.XamlResource.GetI18nDictionary());

        // Engine.Kirikiri did not work
        Core.Startup.TouchGestureHooker.Start(pipeServer.GetClientHandleAsString());

        AdminNotification();

        //Config.Load();

        //if (Config.UseEnterKeyMapping)
        //    KeyboardHooker.Install(GameWindowHandle);

        //if (Config.UseModernSleep)
        //    ModernSleepTimer.Start();

        //DisableWPFTabletSupport();

        //string dir = GetGameDirByHwnd();
        //GameEngine = DetermineEngine(dir);
        //if (noDpiCompatibleSet
        //    // Can not be tapped after menu opened
        //    || GameEngine == Engine.Shinario
        //    // The hole window is blocked (game さめ)
        //    || GameEngine == Engine.Kirikiri)
        //    //|| File.Exists(Path.Combine(dir, "pixel.windows.exe")))
        //    TouchStyle = TouchStyle.Old;
    }

    private static Engine DetermineEngine(string dir) => 
        File.Exists(Path.Combine(dir, "RIO.INI")) ? Engine.Shinario :
        File.Exists(Path.Combine(dir, "message.dat")) ? Engine.AtelierKaguya :
        File.Exists(Path.Combine(dir, "char.xp3")) ? Engine.Kirikiri : // data.xp3 ?
        File.Exists(Path.Combine(dir, "SiglusEngine.exe")) ? Engine.SiglusEngine :
        Engine.TBD;

    private static string GetGameDirByHwnd()
    {
        User32.GetWindowThreadProcessId(GameWindowHandle, out var pid);
        var dir = Path.GetDirectoryName(Process.GetProcessById((int)pid).MainModule!.FileName)!;
        return dir;
    }

    private static void AdminNotification()
    {

#if !NET472
        // TODO: net8 Environment.IsPrivilegedProcess
        if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
        {
            new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                .AddText(Helper.XamlResource.GetString("Notification_Admin"))
                .Show(t =>
                {
                    t.Tag = "eh";
                    t.Dismissed += (_, _) => Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.History.Remove("eh");
                    // t.ExpirationTime = DateTime.Now; // ExpirationTime seems not stable
                });
        }
#else
        if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
        {
            Task.Run(() => MessageBox.Show(Helper.XamlResource.GetString("Notification_Admin")));
        }
#endif
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
                // Get the type of the stylusLogic returned from the call to StylusLogic.  
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

public enum Engine
{
    TBD,
    Shinario,
    Kirikiri, // SoftHouse-Seal extrans.dll krmovie.dll protect.dll.exe.x64.x86 sound.xp3 video.xp3 wuvorbis.dll
    AtelierKaguya,
    SiglusEngine,
}

public enum TouchStyle
{
    New,
    Old
}