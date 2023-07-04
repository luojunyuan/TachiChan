using TouchChan.AssistiveTouch.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace TouchChan.AssistiveTouch.Menu
{
    public partial class MainPage : Page, ITouchMenuPage
    {
        private readonly DoubleAnimation _fadeOutAnimation = AnimationTool.FadeOutAnimation;
        private readonly DoubleAnimation _fadeInAnimation = AnimationTool.FadeInAnimation;

        public event EventHandler<PageEventArgs>? PageChanged;

        public MainPage()
        {
            InitializeComponent();
            _fadeOutAnimation.Completed += (_, _) =>
            {
                Visibility = Visibility.Hidden;
                TouchMenuItem.ClickLocked = false;
            };
            _fadeOutAnimation.Freeze();
            _fadeInAnimation.Freeze();
        }

        public void Close() => BeginAnimation(OpacityProperty, _fadeOutAnimation);

        public void Show(double _)
        {
            Visibility = Visibility.Visible;
            BeginAnimation(OpacityProperty, _fadeInAnimation);
        }

        private void GameOnClick(object sender, EventArgs e) => PageChanged?.Invoke(this, new(TouchMenuPageTag.Game));

        private void DeviceOnClick(object sender, EventArgs e) => PageChanged?.Invoke(this, new(TouchMenuPageTag.Device));

        private void FunctionOnClick(object sender, EventArgs e) => PageChanged?.Invoke(this, new(TouchMenuPageTag.Function));

    }
}
