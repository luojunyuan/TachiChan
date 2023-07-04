using TouchChan.AssistiveTouch.Helper;
using TouchChan.AssistiveTouch.NativeMethods;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TouchChan.AssistiveTouch.Menu
{
    public partial class WinMovePage : Page, ITouchMenuPage
    {
        public event EventHandler<PageEventArgs>? PageChanged;
        public void Close() => PageChanged?.Invoke(this, new(TouchMenuPageTag.None));

        public WinMovePage()
        {
            InitializeComponent();
            InitializeAnimation();
        }

        public void Show(double moveDistance)
        {
            SetCurrentValue(VisibilityProperty, Visibility.Visible);

            XamlResource.SetAssistiveTouchItemBackground(Brushes.Transparent);

            var aaaTransform = AnimationTool.LeftOneTransform(moveDistance);
            var bbbTransform = AnimationTool.LeftTwoBottomOneTransform(moveDistance);
            var cccTransform = AnimationTool.BottomOneTransform(moveDistance);
            var dddTransform = AnimationTool.LeftOneBottomTwoTransform(moveDistance);

            AAA.SetCurrentValue(RenderTransformProperty, aaaTransform);
            BBB.SetCurrentValue(RenderTransformProperty, bbbTransform);
            CCC.SetCurrentValue(RenderTransformProperty, cccTransform);
            DDD.SetCurrentValue(RenderTransformProperty, dddTransform);

            _aaaMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, aaaTransform.X);
            _bbbMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, bbbTransform.X);
            _bbbMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, bbbTransform.Y);
            _cccMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, cccTransform.Y);
            _dddMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, bbbTransform.X);
            _dddMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, bbbTransform.Y);

            _transitionInStoryboard.Begin();
        }

        private readonly Storyboard _transitionInStoryboard = new();
        private readonly DoubleAnimation _aaaMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _bbbMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _bbbMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _cccMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _dddMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _dddMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;

        private void InitializeAnimation()
        {
            AnimationTool.BindingAnimation(_transitionInStoryboard, AnimationTool.FadeInAnimation, this, new(OpacityProperty), true);

            AnimationTool.BindingAnimation(_transitionInStoryboard, _aaaMoveAnimation, AAA, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _bbbMoveXAnimation, BBB, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _bbbMoveYAnimation, BBB, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _cccMoveAnimation, CCC, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _dddMoveXAnimation, DDD, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _dddMoveYAnimation, DDD, AnimationTool.YProperty);

            _transitionInStoryboard.Completed += (_, _) =>
            {
                XamlResource.SetAssistiveTouchItemBackground(XamlResource.AssistiveTouchBackground);

                if (!_transitionInStoryboard.AutoReverse)
                {
                    AAA.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    BBB.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    CCC.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    DDD.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                }
                else
                {
                    _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, false);
                    SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                    TouchMenuItem.ClickLocked = false;
                }
            };
        }

        private static void Add(int x, int y)
        {
            User32.GetWindowRect(App.GameWindowHandle, out var rect);
            Win32.MoveWindow(App.GameWindowHandle, rect.left += x, rect.top += y);
        }
        private void AAAOnClick(object sender, EventArgs e) => Add(0, -1);
        private void BBBOnClick(object sender, EventArgs e) => Add(-1, 0);
        private void CCCOnClick(object sender, EventArgs e) => Add(1, 0);
        private void DDDOnClick(object sender, EventArgs e) => Add(0, 1);
    }
}
