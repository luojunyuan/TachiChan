using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TouchChan.AssistiveTouch.Core.Extend;
using TouchChan.AssistiveTouch.Core.Startup;
using TouchChan.AssistiveTouch.Helper;

namespace TouchChan.AssistiveTouch.Menu
{
    public partial class FunctionPage : Page, ITouchMenuPage
    {
        public event EventHandler<PageEventArgs>? PageChanged;

        public FunctionPage()
        {
            InitializeComponent();
            InitializeAnimation();

            Process? keyboard = null;
            VirtualKeyboard.Toggled += async (_, _) =>
            {
                if (VirtualKeyboard.IsOn) keyboard = await KeyboardProcess.StartAsync();
                else keyboard?.Kill();
            };

            Stretch.Toggled += (_, _) =>
            {
                if (Stretch.IsOn) StretchWindow.Stretch(App.GameWindowHandle);
                else StretchWindow.Restore(App.GameWindowHandle);
            };

            TouchToMouse.Toggled += (_, _) =>
            {
                if (TouchToMouse.IsOn) TouchConversionHooker.Install();
                else TouchConversionHooker.UnInstall();
            };

            if (!BatteryInfo.IsBatteryAvaliable())
                Battery.Disable();

            Gesture.Disable();
            if (Process.GetProcessesByName("TouchChan.AssistiveTouch.Gesture").Length == 0)
                Gesture.Text = "Gesture (Disable)";

            GameHandler.Disable();
            if (Process.GetProcessesByName("TouchChan.AssistiveTouch.Gamepad").Length == 0)
                GameHandler.Text = "Handler (Disable)";

            // Open another menu to check status
        }

        public void Show(double moveDistance)
        {
            SetCurrentValue(VisibilityProperty, Visibility.Visible);

            XamlResource.SetAssistiveTouchItemBackground(Brushes.Transparent);

            var keyboardTransform = AnimationTool.LeftTwoTopOneTransform(moveDistance);
            var stretchTransform = AnimationTool.TopOneTransform(moveDistance);
            var touchToMouseTransform = AnimationTool.LeftTwoTransform(moveDistance);
            var backTransform = AnimationTool.LeftOneTransform(moveDistance);
            var gestureTransform = AnimationTool.LeftOneBottomOneTransform(moveDistance);
            var gameHandlerTransform = AnimationTool.BottomOneTransform(moveDistance);

            VirtualKeyboard.SetCurrentValue(RenderTransformProperty, keyboardTransform);
            Stretch.SetCurrentValue(RenderTransformProperty, stretchTransform);
            TouchToMouse.SetCurrentValue(RenderTransformProperty, touchToMouseTransform);
            Back.SetCurrentValue(RenderTransformProperty, backTransform);
            Gesture.SetCurrentValue(RenderTransformProperty, gestureTransform);
            GameHandler.SetCurrentValue(RenderTransformProperty, gameHandlerTransform);

            _keyboardMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, keyboardTransform.X);
            _keyboardMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, keyboardTransform.Y);
            _stretchMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, stretchTransform.Y);
            _touchToMouseMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, touchToMouseTransform.X);
            _backMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, backTransform.X);
            _gestureMoveXAnimation.SetCurrentValue(DoubleAnimation.FromProperty, gestureTransform.X);
            _gestureMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, gestureTransform.Y);
            _gameHandlerMoveYAnimation.SetCurrentValue(DoubleAnimation.FromProperty, gameHandlerTransform.Y);

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
        private readonly DoubleAnimation _keyboardMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _keyboardMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _stretchMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _touchToMouseMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _backMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _gestureMoveXAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _gestureMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;
        private readonly DoubleAnimation _gameHandlerMoveYAnimation = AnimationTool.TransformMoveToZeroAnimation;

        private void BackOnClickEvent(object sender, EventArgs e) => PageChanged?.Invoke(this, new(TouchMenuPageTag.FunctionBack));

        private void InitializeAnimation()
        {
            AnimationTool.BindingAnimation(_transitionInStoryboard, AnimationTool.FadeInAnimation, this, new(OpacityProperty), true);

            AnimationTool.BindingAnimation(_transitionInStoryboard, _keyboardMoveXAnimation, VirtualKeyboard, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _keyboardMoveYAnimation, VirtualKeyboard, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _stretchMoveYAnimation, Stretch, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _touchToMouseMoveXAnimation, TouchToMouse, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _backMoveAnimation, Back, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _gestureMoveXAnimation, Gesture, AnimationTool.XProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _gestureMoveYAnimation, Gesture, AnimationTool.YProperty);
            AnimationTool.BindingAnimation(_transitionInStoryboard, _gameHandlerMoveYAnimation, GameHandler, AnimationTool.YProperty);

            _transitionInStoryboard.Completed += (_, _) =>
            {
                XamlResource.SetAssistiveTouchItemBackground(XamlResource.AssistiveTouchBackground);

                if (!_transitionInStoryboard.AutoReverse)
                {
                    VirtualKeyboard.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    Stretch.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    TouchToMouse.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    Back.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    Gesture.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                    GameHandler.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
                }
                else
                {
                    _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, false);
                    SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                    TouchMenuItem.ClickLocked = false;
                }
            };
        }

        #region Batterty Info

        private Border? _batteryInfo;
        private System.Timers.Timer? _batteryMonitor;
        private void BatteryOnToggledEvent(object sender, EventArgs e)
        {
            if (Battery.IsOn)
            {
                _batteryInfo = CreateBorder(out var dischargeRate, out var percentage, out var remain, out var averageDischargeRate, out var predict);
                _batteryMonitor = CreateTimer(dischargeRate, percentage, remain, averageDischargeRate, predict);
                _batteryMonitor.Enabled = true;
                ((Grid)(Application.Current.MainWindow.Content)).Children.Insert(0, _batteryInfo);
            }
            else
            {
                ((Grid)(Application.Current.MainWindow.Content)).Children.Remove(_batteryInfo);
                _batteryMonitor?.Dispose();
            }
        }

        private static string RecoveryString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "恢复中" : "Recovering";
        private static string ChargingString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "充电中" : "Charging";
        private static string WattString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "瓦" : "W";
        private static string MWhString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "毫瓦时" : "mWh";
        private static string RealTimeString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "实时" : "real-time";
        private static string PredictString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "预计" : "predict";
        private static string AverageString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "平均值" : "average";
        private static string minString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "分" : "m";
        private static string secString = CultureInfo.CurrentCulture.Name == "zh-CN" ? "秒" : "s";

        private static System.Timers.Timer CreateTimer(TextBlock a, TextBlock b, TextBlock c, TextBlock d, TextBlock e)
        {
            var timer = new System.Timers.Timer
            {
                Interval = 1000
            };

            // mWh capacity
            var lastCapacity = 0;
            // negative mW (int) -> / 1000.0 -> W
            // negative mW (int) -> / 3600.0 -> W per-second
            var lastDischargeRate = 0;
            var countRateAlteration = 0;
            var displayCapacity = 0.0;
            var averageRate = 0;
            var totalEnergy = 0;
            var totalSeconds = 0;
            int percent7 = BatteryInfo.GetBatteryInformation().FullChargeCapacity * 6 / 100;
            var fromCharging = false;
            timer.Elapsed += (s, evt) =>
            {
                // Online means no battery or charging
                if (SystemParameters.PowerLineStatus == PowerLineStatus.Online)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        a.Text = ChargingString;
                        b.Text = c.Text = d.Text = e.Text = string.Empty;
                    });
                    countRateAlteration = 0;
                    lastCapacity = 0;
                    displayCapacity = 0;
                    averageRate = 0;
                    totalEnergy = 0;
                    totalSeconds = 0;
                    fromCharging = true;
                    return;
                }

                if (fromCharging)
                {
                    fromCharging = false;
                    timer.Stop();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        a.Text = RecoveryString;
                        b.Text = c.Text = d.Text = e.Text = string.Empty;
                    });
                    Thread.Sleep(10000);
                    timer.Start();
                    return;
                }

                totalSeconds++;
                var info = BatteryInfo.GetBatteryInformation();

                var newRate = info.DischargeRate;

                // countRateAlteration
                countRateAlteration = (info.DischargeRate == lastDischargeRate) switch
                {
                    true => countRateAlteration + 1,
                    false => 0, // reset
                };

                // displayCapacity
                displayCapacity = (info.CurrentCapacity == lastCapacity) switch
                {
                    true => displayCapacity += info.DischargeRate / 3600.0,
                    false => info.CurrentCapacity // reset|calibrate
                };

                // duration
                var duration = (int)(displayCapacity / -info.DischargeRate * 3600); // hours to seconds

                // averageRate 
                (averageRate, totalEnergy) = (totalEnergy == 0) switch
                {
                    true => (info.DischargeRate, -info.DischargeRate), // init
                    false => ((Func<(int, int)>)(() =>
                    {
                        totalEnergy -= info.DischargeRate;
                        return (-totalEnergy / totalSeconds, totalEnergy);
                    }))()
                };

                // duration2
                var durationPredict = (displayCapacity - percent7) / -averageRate * 3600.0;

                var aa = $"{Math.Round(-info.DischargeRate / 1000.0, 2)} {WattString} ({countRateAlteration}{secString})";
                var bb = (info.CurrentCapacity / (double)info.FullChargeCapacity).ToString("P0") + $" ({RealTimeString})";
                var cc = $"{displayCapacity:f1}{MWhString}, {duration / 60}{minString}{duration % 60}{secString}";
                var dd = $"{Math.Round(-averageRate / 1000.0, 2)} {WattString} ({AverageString})";
                var ee = $"{totalSeconds / 60}:{totalSeconds % 60}-{(int)durationPredict / 60}:{(int)durationPredict % 60} ({PredictString})";

                Application.Current.Dispatcher.Invoke(() =>
                {
                    a.Text = aa;
                    b.Text = bb;
                    c.Text = cc;
                    d.Text = dd;
                    e.Text = ee;
                });
                lastDischargeRate = info.DischargeRate;
                lastCapacity = (int)info.CurrentCapacity;
            };

            return timer;
        }


        private static Border CreateBorder(
            out TextBlock dischargeRate,
            out TextBlock percentage,
            out TextBlock remain,
            out TextBlock averageDischargeRate,
            out TextBlock predict)
        {
            dischargeRate = new TextBlock() { FontSize = 24, Foreground = Brushes.White };
            percentage = new TextBlock() { FontSize = 24, Foreground = Brushes.White };
            remain = new TextBlock() { FontSize = 24, Foreground = Brushes.White };
            averageDischargeRate = new TextBlock() { FontSize = 24, Foreground = Brushes.White };
            predict = new TextBlock() { FontSize = 24, Foreground = Brushes.White };
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(dischargeRate);
            stackPanel.Children.Add(percentage);
            stackPanel.Children.Add(remain);
            stackPanel.Children.Add(averageDischargeRate);
            stackPanel.Children.Add(predict);
            var border = new Border()
            {
                Background = new SolidColorBrush() { Color = Colors.Black, Opacity = 0.6 },
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = double.NaN,
                Height = 160,
                Child = stackPanel,
                CornerRadius = new(8),
            };
            //var binding = new Binding("Width")
            //{   // MaxWidth to MainWindow.Width
            //    Source = Application.Current.MainWindow,
            //    Converter = new HalfWidthConverter()
            //};
            //BindingOperations.SetBinding(border, MaxWidthProperty, binding);
            return border;
        }
        //public class HalfWidthConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        if (value is double width)
        //            return width / 2;

        //        return value;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
        //}
        #endregion Battery Info
    }
}
