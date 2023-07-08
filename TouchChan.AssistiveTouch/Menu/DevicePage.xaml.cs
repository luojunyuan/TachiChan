using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TouchChan.AssistiveTouch.Helper;
using Windows.Media.Protection.PlayReady;
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

            (_tuneBladeClient, TuneBladePort) = SetupTuneBlade();
        }

        // How about if TuneBlade exit, restart etc. Not consider. 
        private readonly int TuneBladePort;
        private (HttpClient?, int) SetupTuneBlade()
        {
            var pid = Process.GetProcessesByName("TuneBlade").FirstOrDefault()?.Id ?? -1;
            if (pid == -1)
                return (null, 0);
            
            // TuneBlade exist
            var port = RetrieveTuneBladePort(pid);
            var client = new HttpClient();
            var response = client.GetAsync($"http://localhost:{port}/v2/").Result;
            if (!response.IsSuccessStatusCode) // Check port failed
                return (null, 0);

            // Connection
            string responseBody = response.Content.ReadAsStringAsync().Result;
            Debug.WriteLine(responseBody);
            if (responseBody == string.Empty)
                return (null, 0);

            // Only support connect to the first device
            var arr = responseBody.Split(' ');// (id, status, volume, name)
            var id = arr[0];
            var status = int.Parse(arr[1]) != 0;
            if (status == false)
                client.GetAsync($"http://localhost:{port}/v2/{id}/Status/Connect");

            VolumeDown.Symbol = Symbol.CalculatorSubtract;
            VolumeUp.Symbol = Symbol.CalculatorAddition;
            return (client, port);
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

        private readonly HttpClient? _tuneBladeClient;
        private async void VolumeDownOnClickEvent(object sender, EventArgs e)
        {
            if (_tuneBladeClient is null)
            {
                await WindowsInput.Simulate.Events()
                    .Click(KeyCode.VolumeDown)
                    .Invoke();
            }
            else
            {
                var (volume, id) = await GetMasterDeviceVolumeAsync(_tuneBladeClient, TuneBladePort);
                var newVolume = volume < 5 ? 0 : volume - 5;
                await _tuneBladeClient.GetAsync($"http://localhost:{TuneBladePort}/v2/{id}/Volume/{newVolume}");
            }
        }

        private static async Task<(int, string)> GetMasterDeviceVolumeAsync(HttpClient client, int port)
        {
            var response = await client.GetAsync($"http://localhost:{port}/v2/");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var arr = responseBody.Split(' ');// (id, status, volume, name)
            var id = arr[0];
            var status = int.Parse(arr[1]) != 0;
            var volume = int.Parse(arr[2]);
            if (status == false)
                await client.GetAsync($"http://localhost:{port}/v2/{id}/Status/Connect");
            return (volume, id);
        }

        private async void VolumeUpOnClickEvent(object sender, EventArgs e)
        {
            if (_tuneBladeClient is null)
            {
                await WindowsInput.Simulate.Events()
                    .Click(KeyCode.VolumeUp)
                    .Invoke();
            }
            else
            {
                var (volume, id) = await GetMasterDeviceVolumeAsync(_tuneBladeClient, TuneBladePort);
                var newVolume = volume > 95 ? 100 : volume + 5;
                await _tuneBladeClient.GetAsync($"http://localhost:{TuneBladePort}/v2/{id}/Volume/{newVolume}");
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

        private static int RetrieveTuneBladePort(int pid)
        {
            var netstatStartInfo = new ProcessStartInfo
            {
                FileName = "netstat",
                Arguments = "-aon",
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var netstatProcess = new Process();
            netstatProcess.StartInfo = netstatStartInfo;
            netstatProcess.Start();

            var findstrStartInfo = new ProcessStartInfo
            {
                FileName = "findstr",
                Arguments = $"{pid}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var findstrProcess = new Process();
            findstrProcess.StartInfo = findstrStartInfo;
            findstrProcess.Start();

            findstrProcess.StandardInput.Write(netstatProcess.StandardOutput.ReadToEnd());
            findstrProcess.StandardInput.Close();

            var netstatOutput = findstrProcess.StandardOutput.ReadToEnd();

            findstrProcess.WaitForExit();
            netstatProcess.WaitForExit();

            var raw = netstatOutput.Split('\n')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1].Split(':')[1];
            return int.Parse(raw);
        }
    }
}
