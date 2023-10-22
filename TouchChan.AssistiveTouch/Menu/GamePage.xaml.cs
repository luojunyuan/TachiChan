using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TouchChan.AssistiveTouch.Core;
using TouchChan.AssistiveTouch.Core.Extend;
using TouchChan.AssistiveTouch.Helper;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Menu
{
    public partial class GamePage : Page, ITouchMenuPage
    {
        public event EventHandler<PageEventArgs>? PageChanged;

        public GamePage()
        {
            InitializeComponent();
            InitializeAnimation();

            var keyboardPath = Path.Combine(Directory.GetCurrentDirectory(), "TouchChan.VirtualKeyboard.exe");
            if (File.Exists(keyboardPath))
            {
                VirtualKeyboard.Visibility = Visibility.Visible;
                Process? keyboard = null;
                Application.Current.Exit += (_, _) => keyboard?.Kill();
                VirtualKeyboard.Toggled += (_, _) =>
                {
                    if (VirtualKeyboard.IsOn) keyboard = Process.Start(keyboardPath, App.GameWindowHandle.ToString());
                    else keyboard?.Kill();
                };
            }

            void SetFullScreenSwitcher(bool inFullScreen) =>
                (FullScreenSwitcher.Symbol, FullScreenSwitcher.Text) = inFullScreen ?
                    (Symbol.BackToWindow, XamlResource.GetString("AssistiveTouch_Window")) :
                    (Symbol.FullScreen, XamlResource.GetString("AssistiveTouch_FullScreen"));
            SetFullScreenSwitcher(FullScreen.UpdateFullscreenStatus());
            FullScreen.FullscreenChanged += (_, isFullScreen) => SetFullScreenSwitcher(isFullScreen);

            TouchToMouse.Toggled += (_, _) =>
            {
                if (TouchToMouse.IsOn) TouchConversionHooker.Install();
                else TouchConversionHooker.UnInstall();
            };

            BrightnessUp.IsEnabledEx = false;

            // For second inside menu
            _fadeOutAnimation.Completed += (_, _) =>
            {
                Visibility = Visibility.Hidden;
                TouchMenuItem.ClickLocked = false;
            };
            _fadeOutAnimation.Freeze();
        }

        public void Show(double moveDistance)
        {
            SetCurrentValue(VisibilityProperty, Visibility.Visible);

            XamlResource.SetAssistiveTouchItemBackground(Brushes.Transparent);

            var keyboardTransform = AnimationTool.TopOneTransform(moveDistance);
            var fullScreenTransform = AnimationTool.RightOneTopOneTransform(moveDistance);
            var moveGameTransform = AnimationTool.RightTwoTopOneTransform(moveDistance);
            var backTransform = AnimationTool.RightOneTransform(moveDistance);
            var closeGameTransform = AnimationTool.RightTwoTransform(moveDistance);
            var brightnessDownTransform = AnimationTool.BottomOneTransform(moveDistance);
            var brightnessUpTransform = AnimationTool.RightOneBottomOneTransform(moveDistance);

            VirtualKeyboard.SetCurrentValue(RenderTransformProperty, keyboardTransform);
            FullScreenSwitcher.SetCurrentValue(RenderTransformProperty, fullScreenTransform);
            MoveGame.SetCurrentValue(RenderTransformProperty, moveGameTransform);
            Back.SetCurrentValue(RenderTransformProperty, backTransform);
            CloseGame.SetCurrentValue(RenderTransformProperty, closeGameTransform);
            BrightnessDown.SetCurrentValue(RenderTransformProperty, brightnessDownTransform);
            BrightnessUp.SetCurrentValue(RenderTransformProperty, brightnessUpTransform);

            _keyboardAnimation.SetCurrentValue(DoubleAnimation.FromProperty, keyboardTransform.Y);
            _fullScreenMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, fullScreenTransform.X);
            _fullScreenMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, fullScreenTransform.Y);
            _moveGameMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, moveGameTransform.X);
            _moveGameMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, moveGameTransform.Y);
            _backMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, backTransform.X);
            _closeGameMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, closeGameTransform.X);
            _brightnessDownMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, brightnessDownTransform.Y);
            _brightnessUpMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, brightnessUpTransform.X);
            _brightnessUpMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, brightnessUpTransform.Y);

            _transitionInStoryboard.Begin();
        }

        public void Close()
        {
            XamlResource.SetAssistiveTouchItemBackground(Brushes.Transparent);
            _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, true);
            _transitionInStoryboard.Begin();
            _transitionInStoryboard.Seek(TouchButton.MenuTransistDuration);
        }

        private readonly Storyboard _transitionInStoryboard = new();
        private readonly DoubleAnimation _keyboardAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _fullScreenMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _fullScreenMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _moveGameMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _moveGameMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _backMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _closeGameMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _brightnessDownMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _brightnessUpMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _brightnessUpMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;

        private void InitializeAnimation()
        {
            AnimationTool.BindingAnimation(_transitionInStoryboard, AnimationTool.FadeInAnimation, this, new(OpacityProperty), true);

            AnimationTool.BindingAnimation(_transitionInStoryboard, _keyboardAnimation, VirtualKeyboard, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _fullScreenMoveXAnimation, FullScreenSwitcher, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _fullScreenMoveYAnimation, FullScreenSwitcher, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _moveGameMoveXAnimation, MoveGame, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _moveGameMoveYAnimation, MoveGame, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _backMoveAnimation, Back, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _closeGameMoveAnimation, CloseGame, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _brightnessDownMoveAnimation, BrightnessDown, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _brightnessUpMoveXAnimation, BrightnessUp, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _brightnessUpMoveYAnimation, BrightnessUp, AnimationTool.YProperty);

            _transitionInStoryboard.Completed += (_, _) =>
            {
                XamlResource.SetAssistiveTouchItemBackground(XamlResource.AssistiveTouchBackground);

                if (!_transitionInStoryboard.AutoReverse)
                {
                    VirtualKeyboard.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    FullScreenSwitcher.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    MoveGame.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    Back.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    CloseGame.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    BrightnessDown.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    BrightnessUp.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                }
                else
                {
                    _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, false);
                    SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                    TouchMenuItem.ClickLocked = false;
                }
            };
        }

        private const int UIMinimumResponseTime = 50;
        private async void FullScreenSwitcherOnClickEvent(object sender, EventArgs e)
        {
            if (TouchStyle.New == App.TouchStyle)
            {
                switch (App.GameEngine)
                {
                    case Engine.AtelierKaguya:
                        if (FullScreen.GameInFullscreen)
                        {
                            User32.SetCursorPos(User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN) - 5, User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN) - 5);
                            Simulate.Pretend(Simulate.ButtonCode.Right);
                            await Task.Delay(UIMinimumResponseTime);
                            Simulate.Pretend(Simulate.KeyCode.Up);
                            await Task.Delay(UIMinimumResponseTime);
                            Simulate.Pretend(Simulate.KeyCode.E);
                            await Task.Delay(UIMinimumResponseTime);
                            Simulate.Pretend(Simulate.KeyCode.W);
                        }
                        else User32.PostMessage(App.GameWindowHandle, User32.WindowMessage.WM_SYSCOMMAND, (IntPtr)User32.SysCommand.SC_MAXIMIZE);
                        break;
                    default:
                        HwndTools.WindowLostFocus(MainWindow.Handle, true);
                        Simulate.Pretend(Simulate.KeyCode.Alt, Simulate.KeyCode.Enter);
                        HwndTools.WindowLostFocus(MainWindow.Handle, false);
                        break;
                }
            }
            else
            {
                User32.BringWindowToTop(App.GameWindowHandle);
                Simulate.Pretend(Simulate.KeyCode.Alt, Simulate.KeyCode.Enter);
            }

        }

        private readonly DoubleAnimation _fadeOutAnimation = AnimationTool.FadeOutAnimation;
        private void CloseInside() => BeginAnimation(OpacityProperty, _fadeOutAnimation);

        private void MoveGameOnClick(object sender, EventArgs e)
        {
            CloseInside();
            PageChanged?.Invoke(this, new(TouchMenuPageTag.WinMove));
        }

        private void BackOnClick(object sender, EventArgs e) => PageChanged?.Invoke(this, new(TouchMenuPageTag.GameBack));

        private const int MenuTransitsDuration = 200;
        private void CloseGameOnClick(object sender, EventArgs e)
        {
            var CloseGameImplementation = new Dictionary<TouchStyle, Action>
            {
                { TouchStyle.New, async () =>
                {
                    await Task.Delay(MenuTransitsDuration);
                    Simulate.Pretend(Simulate.KeyCode.Alt, Simulate.KeyCode.F4);
                } },
                { TouchStyle.Old, () =>
                {
                    User32.PostMessage(App.GameWindowHandle, User32.WindowMessage.WM_SYSCOMMAND,
                    // ReSharper disable once RedundantArgumentDefaultValue
                    (IntPtr)User32.SysCommand.SC_CLOSE);
                    // User32.BringWindowToTop(App.GameWindowHandle); // Shrink the menu
                } },
            };

            ((MainWindow)Application.Current.MainWindow).Menu.ManualClose();

            CloseGameImplementation[App.TouchStyle]();
        }

        private void BrightnessDownOnClick(object sender, EventArgs e)
        {
            if (BrightnessMaskWindow == null)
            {
                BrightnessMaskWindow = new TransparentChromeWindow();
                BrightnessMaskWindow.Show();
                BrightnessUp.IsEnabledEx = true;
                return;
            }

            BrightnessMaskWindow.Opacity += 0.1;
            if (BrightnessMaskWindow.Opacity > 0.7)
            {
                BrightnessDown.IsEnabledEx = false;
                BrightnessUp.IsEnabledEx = true;
            }
        }

        private void BrightnessUpOnClick(object sender, EventArgs e)
        {
            BrightnessMaskWindow!.Close();
            BrightnessMaskWindow = null;
            BrightnessUp.IsEnabledEx = false;
            BrightnessDown.IsEnabledEx = true;
        }

        private Window? BrightnessMaskWindow { get; set; }
    }
}
