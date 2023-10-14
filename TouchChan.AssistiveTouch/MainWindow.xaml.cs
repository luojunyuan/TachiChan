﻿using TouchChan.AssistiveTouch.Core;
using TouchChan.AssistiveTouch.Helper;
using TouchChan.AssistiveTouch.NativeMethods;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using TouchChan.AssistiveTouch.Menu;

namespace TouchChan.AssistiveTouch;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Thread RootHwndWatch, Stylus Input are from Wpf 
    public static IntPtr Handle { get; private set; }

    public double Dpi { get; }

    public MainWindow()
    {
        InitializeComponent();
        Handle = new WindowInteropHelper(this).EnsureHandle();
        Dpi = VisualTreeHelper.GetDpi(this).DpiScaleX;

        Loaded += (_, _) => ForceSetForegroundWindow(App.GameWindowHandle);
        //Loaded += (_, _) => {Console.WriteLine($"Old style: {App.OldStyleTouch}");Console.WriteLine($"Small device: {Environment.GetCommandLineArgs().Contains("--small-device")}");Console.WriteLine($"Dpi: {Dpi}");Console.WriteLine($"Window: ({Width}, {Height})");Console.WriteLine($"Touch: ({Touch.ActualWidth}, {Touch.ActualHeight})");};
        ContentRendered += (_, _) =>
        {
            // Final chance to check window handle, due to some games has a launcher window 
            if (!User32.IsWindow(App.GameWindowHandle))
            {
                Close();
                return;
            }
            IpcRenderer.Send("Loaded");
        };

        if (App.TouchStyle == TouchStyle.New)
        {
            HwndTools.RemovePopupAddChildStyle(Handle);
            User32.SetParent(Handle, App.GameWindowHandle);
            User32.GetClientRect(App.GameWindowHandle, out var rectClient);
            User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, rectClient.Width, rectClient.Height, User32.SetWindowPosFlags.SWP_NOZORDER);

            var hooker = new GameWindowHooker(Handle);
            hooker.SizeChanged += (_, _) => Fullscreen.UpdateFullscreenStatus();
            hooker.GameWindowLostFocus += (_, _) => { if (Menu.IsOpened) Menu.ManualClose(); };
        }
        else
        {
            Topmost = true;
            ShowInTaskbar = false;
            HwndTools.HideWindowInAltTab(Handle);
            Loaded += (_, _) =>
            {
                // ((GamePage)Menu.GameMenu.Content).FullScreenSwitcher.Disable();
                ((GamePage)Menu.GameMenu.Content).MoveGame.Disable();
            };
            var hooker = new GameWindowHookerOld(Close);
            hooker.SizeChanged += (_, _) => Fullscreen.UpdateFullscreenStatus();
            hooker.WindowPositionChanged += (_, pos) =>
            {
                Height = pos.Height / Dpi;
                Width = pos.Width / Dpi;
                Left = pos.Left / Dpi;
                Top = pos.Top / Dpi;
            };
            hooker.UpdatePosition(App.GameWindowHandle);
        }

        if (Config.UseEdgeTouchMask)
        {
            Fullscreen.MaskForScreen(this);
        }
    }

    private static void ForceSetForegroundWindow(IntPtr hWnd)
    {
        // Must use foreground window thread, whatever who is it.(expect task manager, almost explorer predictable:)
        uint foreThread = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), out _);
        uint appThread = Kernel32.GetCurrentThreadId();

        User32.AttachThreadInput(foreThread, appThread, true);
        User32.BringWindowToTop(hWnd);
        User32.AttachThreadInput(foreThread, appThread, false);
    }
}
