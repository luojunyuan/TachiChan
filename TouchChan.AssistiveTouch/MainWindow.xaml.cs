using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TouchChan.AssistiveTouch.Core;
using TouchChan.AssistiveTouch.Core.Extend;
using TouchChan.AssistiveTouch.Helper;
using TouchChan.AssistiveTouch.NativeMethods;

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
        ContentRendered += (_, _) =>
        {
            // Final chance to check window handle, due to some games has a launcher window 
            if (!User32.IsWindow(App.GameWindowHandle))
                Close();
            IpcRenderer.Send("Loaded");
        };

        if (App.TouchStyle == TouchStyle.Inside)
        {
            HwndTools.RemovePopupAddChildStyle(Handle);
            User32.SetParent(Handle, App.GameWindowHandle);
            User32.GetClientRect(App.GameWindowHandle, out var rectClient);
            User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, rectClient.Width, rectClient.Height, User32.SetWindowPosFlags.SWP_NOZORDER);

            var hooker = new GameWindowHooker(Handle);
            hooker.SizeChanged += async (_, _) =>
            {
                // HACK: rect.left rect.top changes only a few milliseconds slower than size.
                await Task.Delay(10).ConfigureAwait(true);
                FullScreen.UpdateFullscreenStatus();
            };
            hooker.GameWindowLostFocus += (_, _) => { if (Menu.IsOpened) Menu.ManualClose(); };
        }
        else
        {
            Topmost = true;
            ShowInTaskbar = false;
            HwndTools.HideWindowInAltTab(Handle);
            var hooker = new GameWindowHookerOld(Close);
            hooker.SizeChanged += async (_, _) =>
            {
                await Task.Delay(10).ConfigureAwait(true);
                FullScreen.UpdateFullscreenStatus();
            };
            hooker.WindowPositionChanged += (_, pos) =>
            {
                Height = pos.Height / Dpi;
                Width = pos.Width / Dpi;
                Left = pos.Left / Dpi;
                Top = pos.Top / Dpi;
            };
            hooker.UpdatePosition(App.GameWindowHandle);

            // Bring window to top and lost focus when full-screen
            if (Engine.Shinario == App.GameEngine ||
                Engine.RenPy == App.GameEngine)
            {
                const int GameFullScreenStatusRefreshTime = 200;
                System.Timers.Timer stayTopTimer = new(GameFullScreenStatusRefreshTime);
                stayTopTimer.Elapsed += (_, _) => User32.BringWindowToTop(Handle);
                void FullScreenChangedAction(object? _, bool isFullScreen)
                {
                    stayTopTimer.Enabled = isFullScreen;
                    if (isFullScreen) HwndTools.WindowLostFocus(Handle, true);
                    else HwndTools.WindowLostFocus(Handle, false);
                }
                FullScreen.FullscreenChanged += FullScreenChangedAction;
                Loaded += (_, _) => FullScreenChangedAction(this, FullScreen.GameInFullscreen);
            }
        }

        if (Config.UseEdgeTouchMask)
        {
            FullScreen.MaskForScreen(this);
        }

        FadeOutAnimation.Duration = TimeSpan.FromSeconds(2);
        FadeOutAnimation.EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseIn };
        FadeOutAnimation.FillBehavior = FillBehavior.HoldEnd;
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

    private readonly DoubleAnimation FadeOutAnimation = AnimationTool.FadeOutAnimation;
    public void StartBubbleAnimation(string tip) 
    { 
        GestureTip.Text = tip;

        GestureBubble.BeginAnimation(OpacityProperty, null);

        void completedEvent(object sender, EventArgs e) => GestureBubble.Visibility = Visibility.Collapsed;
        FadeOutAnimation.Completed -= completedEvent;

        GestureBubble.Visibility = Visibility.Visible;
        FadeOutAnimation.Completed += completedEvent;
        GestureBubble.BeginAnimation(OpacityProperty, FadeOutAnimation);
    }
}
