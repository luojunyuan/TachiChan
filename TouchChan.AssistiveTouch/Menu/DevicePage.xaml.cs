using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TouchChan.AssistiveTouch.Helper;
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

            if (ExistProcess("TuneBlade")) // && Config.TuneBladePort not 0
            {
                _tuneBlade = TuneBladeApi.Setup(port);
                if (_tuneBlade is not null)
                {
                    VolumeDown.Symbol = Symbol.CalculatorSubtract;
                    VolumeUp.Symbol = Symbol.CalculatorAddition;
                }
            }
        }

        private const int port = 60142;
        private static bool ExistProcess(string processName) => Process.GetProcessesByName(processName).Length != 0;

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
            var backToDesktopTransform = AnimationTool.BottomTwoTransform(moveDistance);
            VolumeDown.SetCurrentValue(RenderTransformProperty, volumeDownTransform);
            ScreenShot.SetCurrentValue(RenderTransformProperty, screenshotTransform);
            Back.SetCurrentValue(RenderTransformProperty, backTransform);
            TaskView.SetCurrentValue(RenderTransformProperty, taskviewTransform);
            DockRight.SetCurrentValue(RenderTransformProperty, dockrightTransform);
            VirtualTouchpad.SetCurrentValue(RenderTransformProperty, touchpadTransform);
            BackToDesktop.SetCurrentValue(RenderTransformProperty, backToDesktopTransform);

            _volumeDownMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, volumeDownTransform.X);
            _screenshotMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, screenshotTransform.X);
            _backMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, backTransform.Y);
            _taskviewMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, taskviewTransform.X);
            _taskviewMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, taskviewTransform.Y);
            _dockrightMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, dockrightTransform.X);
            _dockrightMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, dockrightTransform.Y);
            _touchpadMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, touchpadTransform.X);
            _touchpadMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, touchpadTransform.Y);
            _backToDesktopMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, backToDesktopTransform.Y);

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
        private readonly DoubleAnimation _backToDesktopMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;

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
            AnimationTool.BindingAnimation(_transitionInStoryboard, _backToDesktopMoveYAnimation, BackToDesktop, AnimationTool.YProperty);

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
                    BackToDesktop.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
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

        private readonly TuneBladeApi? _tuneBlade;
        private async void VolumeDownOnClickEvent(object sender, EventArgs e)
        {
            if (_tuneBlade is null)
            {
                await WindowsInput.Simulate.Events()
                    .Click(KeyCode.VolumeDown)
                    .Invoke();
            }
            else
            {
                var (volume, id) = await _tuneBlade.GetFirstDeviceVolumeAsync();
                var newVolume = volume < 5 ? 0 : volume - 5;
                await _tuneBlade.ChangeVolumeAsync(id, newVolume);
            }
        }

        private async void VolumeUpOnClickEvent(object sender, EventArgs e)
        {
            if (_tuneBlade is null)
            {
                await WindowsInput.Simulate.Events()
                    .Click(KeyCode.VolumeUp)
                    .Invoke();
            }
            else
            {
                var (volume, id) = await _tuneBlade.GetFirstDeviceVolumeAsync();
                var newVolume = volume > 95 ? 100 : volume + 5;
                await _tuneBlade.ChangeVolumeAsync(id, newVolume);
            }
        }

        private async void ActionCenterOnClickEvent(object sender, EventArgs e) =>
            await WindowsInput.Simulate.Events()
                .ClickChord(KeyCode.LWin, KeyCode.A)
                .Invoke();
        private async void TaskViewOnClickEvent(object sender, EventArgs e) =>
            await WindowsInput.Simulate.Events()
                .ClickChord(KeyCode.LWin, KeyCode.Tab)
                .Invoke();

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
                    .Invoke();
            }
        }

        private void VirtualTouchpadOnClickEvent(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("launchwinapp", "ms-virtualtouchpad:");
            ((MainWindow)Application.Current.MainWindow).Menu.ManualClose();
        }

        private async void BackToDesktopOnClickEvent(object sender, EventArgs e) =>
            await WindowsInput.Simulate.Events()
                .ClickChord(KeyCode.LWin, KeyCode.D)
                .Invoke();

        private class TuneBladeApi
        {
            public static TuneBladeApi? Setup(int port)
            {
                var tb = new TuneBladeApi(port);
                HttpResponseMessage response;
                try
                {
                    response = tb.GetAllDevicesAsync().Result;
                }
                catch (Exception ex)
                {
                    // error port number
                    Debug.WriteLine(ex.Message);
                    return null;
                }

                if (!response.IsSuccessStatusCode) // Check port failed
                    return null;

                // Connection
                string responseBody = response.Content.ReadAsStringAsync().Result;
                Debug.WriteLine(responseBody); // TODO: if there has many (check the option config "friendly name")
                if (responseBody == string.Empty)
                    return null;

                // Only support connect to the first device
                var arr = responseBody.Split(' ');// (id, status, volume, name)
                var id = arr[0];
                var status = Enum.Parse<ConnectionStatus>(arr[1]);
                if (status == ConnectionStatus.Disconnected)
                    _ = tb.ConnectDeviceAsync(id).Result;

                return tb;
            }

            public enum ConnectionStatus
            {
                Disconnected = 0,
                Connected = 100,
                Standby = 200
            }

            private readonly HttpClient Client = new HttpClient();
            private readonly int Port;

            public TuneBladeApi(int port)
            {
                Port = port;
            }

            /// <summary>
            /// Get list of all AirPlay receivers including connection and volume status.
            /// </summary>
            public async Task<HttpResponseMessage> GetAllDevicesAsync() =>
                await Client.GetAsync($"http://localhost:{Port}/v2/");

            /// <summary>
            /// Get connection and volume status of a particular AirPlay receiver.
            /// </summary>
            public async Task<HttpResponseMessage> GetValueByDeviceIdAsync(int id) =>
                await Client.GetAsync($"http://localhost:{Port}/v2/{id}");


            /// <summary>
            /// Connect/Disconnect to a particular AirPlay receiver.
            /// </summary>
            /// <returns>no response body</returns>
            public async Task<HttpResponseMessage> ConnectDeviceAsync(string id, bool connect = true) =>
                await Client.GetAsync($"http://localhost:{Port}/v2/{id}/Status/{(connect ? "Connect" : "Disconnect")}");

            /// <summary>
            /// Change volume of a particular AirPlay receiver
            /// </summary>
            /// <returns>no response body</returns>
            public async Task<HttpResponseMessage> ChangeVolumeAsync(string id, int volumeLevel) =>
                await Client.GetAsync($"http://localhost:{Port}/v2/{id}/Volume/{volumeLevel}");

            public async Task<(int, string)> GetFirstDeviceVolumeAsync()
            {
                var response = await Client.GetAsync($"http://localhost:{Port}/v2/");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var arr = responseBody.Split(' ');// (id, status, volume, name)
                var id = arr[0];
                var status = int.Parse(arr[1]) != 0;
                var volume = int.Parse(arr[2]);
                if (status == false)
                    await Client.GetAsync($"http://localhost:{Port}/v2/{id}/Status/Connect");
                return (volume, id);
            }
        }
    }
}
