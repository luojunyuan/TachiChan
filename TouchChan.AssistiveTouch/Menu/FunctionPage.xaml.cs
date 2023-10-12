using TouchChan.AssistiveTouch.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TouchChan.AssistiveTouch.Core.Extend;

namespace TouchChan.AssistiveTouch.Menu
{
    public partial class FunctionPage : Page, ITouchMenuPage
    {
        public event EventHandler<PageEventArgs>? PageChanged;

        public FunctionPage()
        {
            InitializeComponent();
            InitializeAnimation();

            // TODO: Check the value on AC device, maybe empty?
            // using Windows.Devices.Power;
#if !NET472            
            if (Windows.Devices.Power.Battery.AggregateBattery.DeviceId != "AggregateBattery")
                Battery.Disable();
#endif
        }

        public void Show(double moveDistance)
        {
            SetCurrentValue(VisibilityProperty, Visibility.Visible);

            XamlResource.SetAssistiveTouchItemBackground(Brushes.Transparent);

            var backTransform = AnimationTool.LeftOneTransform(moveDistance);
            Back.SetCurrentValue(RenderTransformProperty, backTransform);
            _backMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, backTransform.X);

            _transitionInStoryboard.Begin();
        }

        public void Close()
        {
            XamlResource.SetAssistiveTouchItemBackground(Brushes.Transparent);
            _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, true);
            _transitionInStoryboard.Begin();
            _transitionInStoryboard.Seek(TouchButton.MenuTransistDuration);
        }

        private void BackOnClickEvent(object sender, EventArgs e) => PageChanged?.Invoke(this, new(TouchMenuPageTag.FunctionBack));

        private readonly Storyboard _transitionInStoryboard = new();
        private readonly DoubleAnimation _backMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;

        private void InitializeAnimation()
        {
            AnimationTool.BindingAnimation(_transitionInStoryboard, AnimationTool.FadeInAnimation, this, new(OpacityProperty), true);

            AnimationTool.BindingAnimation(_transitionInStoryboard, _backMoveAnimation, Back, AnimationTool.XProperty);

            _transitionInStoryboard.Completed += (_, _) =>
            {
                XamlResource.SetAssistiveTouchItemBackground(XamlResource.AssistiveTouchBackground);

                if (!_transitionInStoryboard.AutoReverse)
                {
                    Back.SetCurrentValue(RenderTransformProperty, AnimationTool.ZeroTransform);
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
                        a.Text = "Charging";
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
                        a.Text = "Recovering";
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

                var aa = $"{Math.Round(-info.DischargeRate / 1000.0, 2)} W ({countRateAlteration}s)";
                var bb = (info.CurrentCapacity / (double)info.FullChargeCapacity).ToString("P0") + " (real-time)";
                var cc = $"{displayCapacity:f1}mWh, {duration / 60}m{duration % 60}s";
                var dd = $"{Math.Round(-averageRate / 1000.0, 2)} W (average)";
                var ee = $"{totalSeconds / 60}:{totalSeconds % 60}-{(int)durationPredict / 60}:{(int)durationPredict % 60} (predict)";

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
