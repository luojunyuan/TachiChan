using TouchChan.AssistiveTouch.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WindowsInput.Events;

namespace TouchChan.AssistiveTouch.Menu
{
    public partial class DevicePage : Page, ITouchMenuPage
    {
        public event EventHandler<PageEventArgs>? PageChanged;

        public DevicePage()
        {
            InitializeComponent();
            InitializeAnimation();
        }

        public void Show(double moveDistance)
        {
            SetCurrentValue(VisibilityProperty, Visibility.Visible);

            XamlResource.SetAssistiveTouchItemBackground(Brushes.Transparent);

            var volumeDownTransform = AnimationTool.LeftOneTransform(moveDistance);
            var screenshotTransform = AnimationTool.RightOneTransform(moveDistance);
            var backTransform = AnimationTool.BottomOneTransform(moveDistance);
            var taskviewTransform = AnimationTool.LeftOneBottomOneTransform(moveDistance);
            var dockrightTransform = AnimationTool.RightOneBottomOneTransform(moveDistance);
            var touchpadTransform = AnimationTool.LeftOneBottomTwoTransform(moveDistance);
            VolumeDown.SetCurrentValue(RenderTransformProperty, volumeDownTransform);
            ScreenShot.SetCurrentValue(RenderTransformProperty, screenshotTransform);
            Back.SetCurrentValue(RenderTransformProperty, backTransform);
            TaskView.SetCurrentValue(RenderTransformProperty, taskviewTransform);
            DockRight.SetCurrentValue(RenderTransformProperty, dockrightTransform);
            VirtualTouchpad.SetCurrentValue(RenderTransformProperty, touchpadTransform);

            _volumeDownMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, volumeDownTransform.X);
            _screenshotMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, screenshotTransform.X);
            _backMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, backTransform.Y);
            _taskviewMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, taskviewTransform.X);
            _taskviewMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, taskviewTransform.Y);
            _dockrightMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, dockrightTransform.X);
            _dockrightMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, dockrightTransform.Y);
            _touchpadMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, touchpadTransform.X);
            _touchpadMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, touchpadTransform.Y);

            _transitionInStoryboard.Begin();
        }

        public void Close()
        {
            XamlResource.SetAssistiveTouchItemBackground(Brushes.Transparent);
            _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, true);
            _transitionInStoryboard.Begin();
            _transitionInStoryboard.Seek(TouchButton.MenuTransistDuration);
        }

        private void BackOnClickEvent(object sender, EventArgs e) => PageChanged?.Invoke(this, new(TouchMenuPageTag.DeviceBack));

        private readonly Storyboard _transitionInStoryboard = new();
        private readonly DoubleAnimation _volumeDownMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _screenshotMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _backMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _taskviewMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _taskviewMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _dockrightMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _dockrightMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _touchpadMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _touchpadMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;

        private void InitializeAnimation()
        {
            AnimationTool.BindingAnimation(_transitionInStoryboard, AnimationTool.FadeInAnimation, this, new(OpacityProperty), true);

            AnimationTool.BindingAnimation(_transitionInStoryboard, _volumeDownMoveAnimation, VolumeDown, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _backMoveAnimation, Back, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _taskviewMoveXAnimation, TaskView, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _taskviewMoveYAnimation, TaskView, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _dockrightMoveXAnimation, DockRight, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _dockrightMoveYAnimation, DockRight, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _screenshotMoveXAnimation, ScreenShot, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _touchpadMoveXAnimation, VirtualTouchpad, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _touchpadMoveYAnimation, VirtualTouchpad, AnimationTool.YProperty);

            _transitionInStoryboard.Completed += (_, _) =>
            {
                XamlResource.SetAssistiveTouchItemBackground(XamlResource.AssistiveTouchBackground);

                if (!_transitionInStoryboard.AutoReverse)
                {
                    VolumeDown.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    ScreenShot.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    TaskView.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    Back.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    DockRight.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    VirtualTouchpad.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                }
                else
                {
                    _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, false);
                    SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                    TouchMenuItem.ClickLocked = false;
                }
            };

            if (Config.ScreenShotTradition)
            {
                _screenMask = new Rectangle() { Fill = Brushes.White };
                _fadeout = AnimationTool.FadeOutAnimation;
                _fadeout.Completed += (_, _) => ((Grid)(Application.Current.MainWindow.Content)).Children.Remove(_screenMask);
                _fadeout.Freeze();
            }
        }
        private async void VolumeDownOnClickEvent(object sender, EventArgs e) =>
             await WindowsInput.Simulate.Events()
                .Click(KeyCode.VolumeDown)
                .Invoke().ConfigureAwait(false);

        private async void VolumeUpOnClickEvent(object sender, EventArgs e) =>
            await WindowsInput.Simulate.Events()
                .Click(KeyCode.VolumeUp)
                .Invoke().ConfigureAwait(false);
        private async void ActionCenterOnClickEvent(object sender, EventArgs e) =>
            await WindowsInput.Simulate.Events()
                .ClickChord(KeyCode.LWin, KeyCode.A)
                .Invoke().ConfigureAwait(false);
        private async void TaskViewOnClickEvent(object sender, EventArgs e) =>
            await WindowsInput.Simulate.Events()
                .ClickChord(KeyCode.LWin, KeyCode.Tab)
                .Invoke().ConfigureAwait(false);

        private Rectangle? _screenMask;
        private DoubleAnimation? _fadeout;
        private const int WaitForScreenShot = 500;
        private async void ScreenShotOnClickEvent(object sender, EventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Menu.ManualClose();
            if (Config.ScreenShotTradition)
            {
                await WindowsInput.Simulate.Events()
                    .Wait(WaitForScreenShot)
                    .ClickChord(KeyCode.Alt, KeyCode.PrintScreen)
                    .Invoke();
                ((Grid)(Application.Current.MainWindow.Content)).Children.Add(_screenMask);
                _screenMask!.BeginAnimation(OpacityProperty, _fadeout);
            }
            else
            {
                await WindowsInput.Simulate.Events()
                    .Wait(WaitForScreenShot)
                    .ClickChord(KeyCode.LWin, KeyCode.Shift, KeyCode.S)
                    .Invoke().ConfigureAwait(false);
            }
        }

        private void VirtualTouchpadOnClickEvent(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("launchwinapp", "ms-virtualtouchpad:");
            ((MainWindow)Application.Current.MainWindow).Menu.ManualClose();
        }
    }
}
