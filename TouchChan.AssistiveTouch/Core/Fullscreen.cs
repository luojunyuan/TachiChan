using TouchChan.AssistiveTouch.NativeMethods;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TouchChan.AssistiveTouch.Core;

internal class Fullscreen
{
    public static bool GameInFullscreen { get; private set; }

    public static event EventHandler<bool>? FullscreenChanged;

    public static bool UpdateFullscreenStatus()
    {
        var isFullscreen = IsWindowFullscreen(App.GameWindowHandle);
        if (GameInFullscreen != isFullscreen)
            FullscreenChanged?.Invoke(null, isFullscreen);
        return GameInFullscreen = isFullscreen;
    }

    // See: http://www.msghelp.net/showthread.php?tid=67047&pid=740345
    private static bool IsWindowFullscreen(IntPtr hwnd)
    {
        User32.GetWindowRect(hwnd, out var rect);
        return rect.left < 50 && rect.top < 50 &&
            rect.Width >= User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN) &&
            rect.Height >= User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN);
    }

    public static void MaskForScreen(Window window)
    {
        DoubleAnimation FadeInAnimation = new()
        {
            To = 1.0,
            Duration = TimeSpan.FromSeconds(1.5),
            FillBehavior = FillBehavior.Stop,
            AutoReverse = true
        };
        Border EdgeMask = new()
        {
            BorderBrush = Brushes.DarkGray,
            BorderThickness = new(5),
            Opacity = 0.002
        };
        ((Grid)window.Content).Children.Add(EdgeMask);
        void AddMask(object? _, bool fullscreen)
        {
            if (fullscreen)
            {
                EdgeMask.Visibility = Visibility.Visible;
                EdgeMask.BeginAnimation(UIElement.OpacityProperty, FadeInAnimation);
            }
            else EdgeMask.Visibility = Visibility.Collapsed;
        };
        AddMask(null, GameInFullscreen);
        FullscreenChanged += AddMask;
    }
}
