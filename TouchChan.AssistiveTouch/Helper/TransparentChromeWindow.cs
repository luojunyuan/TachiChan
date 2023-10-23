using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Helper
{
    internal class TransparentChromeWindow : Window
    {
        public TransparentChromeWindow()
        {
            // Chrome window
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            WindowChrome.SetWindowChrome(this, new WindowChrome()
            {
                GlassFrameThickness = new(-1),
                CaptionHeight = 0
            });

            // Click through
            Loaded += TransparentChromeWindow_Loaded;

            Opacity = 0.1;
            Background = Brushes.Black;
            Topmost = true;
            ShowInTaskbar = false;
            ShowActivated = false;
            var handle = new WindowInteropHelper(this).EnsureHandle();
            HwndTools.HideWindowInAltTab(handle);

            var hooker = new GameWindowHookerOld(Close);
            var dpi = ((MainWindow)Application.Current.MainWindow).Dpi;
            void SizeDelegate(object? sender, GameWindowHookerOld.WindowPosition pos)
            {
                Height = pos.Height / dpi;
                Width = pos.Width / dpi;
                Left = pos.Left / dpi;
                Top = pos.Top / dpi;
            }
            hooker.WindowPositionChanged += SizeDelegate;
            hooker.UpdatePosition(App.GameWindowHandle);
            Closed += (_, _) =>
            {
                hooker.WindowPositionChanged -= SizeDelegate;
                hooker = null;
            };

            // Content = new Border() { BorderBrush = Brushes.Red, BorderThickness = new(3) };

            //Application.Current.MainWindow.Closed += (_, _) => Close();
        }

        private void TransparentChromeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Add WS_EX_LAYERED style
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(
                (nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) =>
                {
                    if (msg == (int)User32.WindowMessage.WM_STYLECHANGING &&
                        wParam == (long)User32.WindowLongFlags.GWL_EXSTYLE)
                    {
                        var styleStruct = Marshal.PtrToStructure<User32.STYLESTRUCT>(lParam);
                        styleStruct.styleNew |=
                            User32.WindowStylesEx.WS_EX_LAYERED | User32.WindowStylesEx.WS_EX_TRANSPARENT;
                        Marshal.StructureToPtr(styleStruct, lParam, false);
                        handled = true;
                    }
                    return IntPtr.Zero;
                });
        }
    }
}
