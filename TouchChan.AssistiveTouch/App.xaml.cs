﻿using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using TouchChan.AssistiveTouch.Core;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IntPtr GameWindowHandle { get; private set; }

    public static Engine GameEngine { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var _pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, e.Args[0]);
        _ = new IpcRenderer(_pipeClient);

        var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        _ = new IpcMain(pipeServer);

        GameWindowHandle = int.Parse(e.Args[1]);


        TouchGestureHooker.Start(pipeServer.GetClientHandleAsString(), Environment.ProcessId);

        // TODO: net8 Environment.IsPrivilegedProcess
        if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
        {
            new ToastContentBuilder()
                .AddText(Helper.XamlResource.GetString("Notification_Admin"))
                .Show(t =>
                {
                    t.Tag = "eh";
                    t.Dismissed += (_, _) => ToastNotificationManagerCompat.History.Remove("eh");
                    // t.ExpirationTime = DateTime.Now; // ExpirationTime seems not stable
                });
        }

        Config.Load();

        if (Config.UseEnterKeyMapping)
            KeyboardHooker.Install(GameWindowHandle);

        // Config.UseModernSleep
        ModernSleepTimer.Start();

        User32.GetWindowThreadProcessId(GameWindowHandle, out var pid);
        var dir = Path.GetDirectoryName(Process.GetProcessById((int)pid).MainModule!.FileName)!;
        GameEngine = File.Exists(Path.Combine(dir, "RIO.INI")) ? Engine.Shinario :
            File.Exists(Path.Combine(dir, "message.dat")) ? Engine.AtelierKaguya :
            File.Exists(Path.Combine(dir, "data.xp3")) ? Engine.Kirikiri :
            Engine.TBD;
        if (GameEngine == Engine.Shinario)
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

public enum Engine
{
    TBD,
    Shinario,
    Kirikiri, // SoftHouse-Seal extrans.dll krmovie.dll protect.dll.exe.x64.x86 sound.xp3 video.xp3 wuvorbis.dll
    AtelierKaguya,
}