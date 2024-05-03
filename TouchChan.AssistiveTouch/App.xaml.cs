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
        var _pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, e.Args[0]);
        _ = new IpcRenderer(_pipeClient);

        var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        _ = new IpcMain(pipeServer);

        GameWindowHandle = (IntPtr)int.Parse(e.Args[1]);

        // I18N
        Resources.MergedDictionaries.Add(Helper.XamlResource.GetI18nDictionary());

        string dir = GetGameDirByHwnd();
        GameEngine = DetermineEngine(dir);
        switch (GameEngine)
        {
            case Engine.Kirikiri:
                break;
            case Engine.RenPy: // Gesture not for RenPy
                Core.Startup.GameController.Start(pipeServer.GetClientHandleAsString());
                break;
            case Engine.TBD:
                Core.Startup.TouchGestureHooker.Start(pipeServer.GetClientHandleAsString());
                Core.Startup.GameController.Start(pipeServer.GetClientHandleAsString());
                break;
        }

        AdminNotification();

        if (Config.UseEnterKeyMapping)
            KeyMappingHooker.Install(GameWindowHandle);

        if (Config.UseModernSleep)
            ModernSleepTimer.Start();

        DisableWPFTabletSupport();

        if (IsDpiUnware()
            // Can not be normally tapped after menu opened
            || GameEngine == Engine.Shinario
            // The hole window or part content would be blocked
            || GameEngine == Engine.RenPy
            || GameEngine == Engine.Kirikiri
            || Config.EnforceOldTouchStyle)
            TouchStyle = TouchStyle.Old;
    }

    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            MessageBox.Show(args.ExceptionObject.ToString());
        
        Config.Load();

        if (!Config.DisableTouch)
            return; // normally return

        // Do not display AssistiveTouch still enable these functions
        // TouchConversion, Gesture, Gamepad, KeyMapping

        var _pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, Environment.GetCommandLineArgs()[1]);
        _ = new IpcRenderer(_pipeClient);

        var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        _ = new IpcMain(pipeServer);

        GameWindowHandle = (IntPtr)int.Parse(Environment.GetCommandLineArgs()[2]);

        string dir = GetGameDirByHwnd();
        GameEngine = DetermineEngine(dir);
        if (Engine.Kirikiri != GameEngine)
        {
            Core.Startup.TouchGestureHooker.Start(pipeServer.GetClientHandleAsString());
            Core.Startup.GameController.Start(pipeServer.GetClientHandleAsString());
        }

        if (Config.UseEnterKeyMapping)
            KeyMappingHooker.Install(GameWindowHandle);

        if (Config.UseModernSleep)
            ModernSleepTimer.Start();

        Task.Factory.StartNew(() =>
        {
            _ = new Helper.GameWindowHookerOld(() => Environment.Exit(0));

            IpcRenderer.Send("Loaded");

            while (User32.GetMessage(out var msg, IntPtr.Zero, 0, 0) != false)
            {
                User32.TranslateMessage(msg);
                User32.DispatchMessage(msg);
            }
        }, TaskCreationOptions.LongRunning);

        Thread.Sleep(Timeout.Infinite);
        Environment.Exit(0);
    }

    private static Engine DetermineEngine(string dir) => 
        File.Exists(Path.Combine(dir, "RIO.INI")) ? Engine.Shinario :
        File.Exists(Path.Combine(dir, "message.dat")) ? Engine.AtelierKaguya :
        File.Exists(Path.Combine(dir, "data.xp3")) ? Engine.Kirikiri :
        File.Exists(Path.Combine(dir, "SiglusEngine.exe")) ? Engine.SiglusEngine :
        Directory.Exists(Path.Combine(dir, "renpy")) ? Engine.RenPy :
        Engine.TBD;

    private static bool IsDpiUnware()
    {
        User32.GetWindowThreadProcessId(GameWindowHandle, out var pid);
        var handle = Process.GetProcessById(pid).Handle;
        var result = ShCore.GetProcessDpiAwareness(handle, out var v);

        return result == 0 && v == 0;
    }

    private static string GetGameDirByHwnd()
    {
        User32.GetWindowThreadProcessId(GameWindowHandle, out var pid);
        var dir = Path.GetDirectoryName(Process.GetProcessById((int)pid).MainModule!.FileName)!;
        return dir;
    }

    private static void AdminNotification()
    {

#if !NET472
        if (Environment.IsPrivilegedProcess)
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
    RenPy,
}

public enum TouchStyle
{
    New,
    Old
}