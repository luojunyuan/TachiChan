using TouchChan.AssistiveTouch.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TouchChan.AssistiveTouch;

public partial class TouchButton
{
    public static TimeSpan MenuTransistDuration { get; } = TimeSpan.FromMilliseconds(200);

    public bool IsTouchMenuOpened { get; set; }
    public event EventHandler? TouchMenuClosed;
    public void RaiseMenuClosedEvent(object sender) => TouchMenuClosed?.Invoke(sender, new());
    public event EventHandler? Clicked;

    // iPhone iPad 200ms, but 300ms more suit for eh
    private static readonly TimeSpan TouchReleaseToEdgeDuration = TimeSpan.FromMilliseconds(300);
    private static readonly TimeSpan OpacityFadeInDuration = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan OpacityTransformDuration = TimeSpan.FromMilliseconds(400);
    private const int OpacityChangeDuration = 4000;

    private const double OpacityHalf = 0.4;
    private const double OpacityFull = 1;
    private const double ButtonSpace = 2;

    // Also the diameter of button use for mouse releasing
    public readonly static double TouchSize; // used 60, 100, 75
    private double _distance;
    private double _halfDistance;
    private double _oneThirdDistance;
    private double _twoThirdDistance;

    static TouchButton() { TouchSize = Environment.GetCommandLineArgs().Contains("--small-device") ? 120 : 80; }

    public TouchButton()
    {
        InitializeComponent();

        TouchPosition = string.IsNullOrWhiteSpace(Config.AssistiveTouchPosition) ? AssistiveTouchPosition.Default :
            Config.XmlDeserializer<AssistiveTouchPosition>(Config.AssistiveTouchPosition);
        Application.Current.Exit += (_, _) => { if (TouchPosition != AssistiveTouchPosition.Default) SaveTouchPosition(); };

        SetAssistiveTouchProperties();

        var mainWindow = Application.Current.MainWindow;
        mainWindow.Loaded += (_, _) => SetTouchPosition();
        mainWindow.SizeChanged += (_, _) => SetTouchPosition();


        #region Core Logic of Moving

        var lastPosRealTime = new Point(TouchPosTransform.X, TouchPosTransform.Y);
        bool isDraging = false;
        bool isMoving = false;
        Point relativeMousePos = default;
        Point pointWhenMouseDown = default;
        Point pointWhenMouseUp = default;

        PreviewMouseLeftButtonDown += (_, evt) =>
        {
            if (isMoving)
                return;

            isDraging = true;
            relativeMousePos = evt.GetPosition(this);
            pointWhenMouseDown = evt.GetPosition(mainWindow);
        };

        PreviewMouseMove += (_, evt) =>
        {
            if (!isDraging)
                return;

            isMoving = true;
            // Max mouse event message frequency: 125 fps, dirty react 250 (2*fps)
            var newPos = (Point)(evt.GetPosition(mainWindow) - relativeMousePos);

            TouchPosTransform.SetCurrentValue(TranslateTransform.XProperty, newPos.X);
            TouchPosTransform.SetCurrentValue(TranslateTransform.YProperty, newPos.Y);

            lastPosRealTime = newPos;

            // if mouse go out of the edge
            if (newPos.X < -_oneThirdDistance || newPos.Y < -_oneThirdDistance ||
                newPos.X > mainWindow.ActualWidth - _twoThirdDistance ||
                newPos.Y > mainWindow.ActualHeight - _twoThirdDistance)
            {
                RaiseMouseReleasedEventInCode(this);
            }
        };

        PreviewMouseUp += (_, evt) =>
        {
            isDraging = false;
            pointWhenMouseUp = evt.GetPosition(mainWindow);

            if (isMoving && pointWhenMouseUp != pointWhenMouseDown)
            {
                WhenMouseReleased(mainWindow, TouchPosTransform.X, TouchPosTransform.Y);
            }
        };

        PreviewMouseUp += (_, _) => { if (pointWhenMouseUp == pointWhenMouseDown) isMoving = false; };
        TranslateTouchStoryboard.Completed += (_, _) => isMoving = false;
        TouchMenuClosed += (_, _) => isDraging = isMoving = false;

        #endregion Core Logic of Moving

        #region Opacity Adjust
        // Fade in
        var fadeLock = new AutoResetEvent(true);
        Task ResetAndStartCompleteTask()
        {
            var task = new Task(() => { while (fadeLock.WaitOne()) break; });
            task.Start();
            return task;
        }
        Task fadeAnimationCompleted = ResetAndStartCompleteTask();
        PreviewMouseLeftButtonDown += (_, _) =>
        {
            if (!isMoving && Opacity != OpacityFull)
            {
                fadeAnimationCompleted = ResetAndStartCompleteTask();
                BeginAnimation(OpacityProperty, FadeInOpacityAnimation);
            }
        };
        FadeInOpacityAnimation.Completed += (_, _) => fadeLock.Set();

        // Fade out
        var throttle = new Throttle(OpacityChangeDuration, () =>
        {
            if (isMoving == false && IsTouchMenuOpened == false)
            {
                Dispatcher.Invoke(() => BeginAnimation(OpacityProperty, FadeOpacityAnimation));
            }
        });
        mainWindow.Loaded += (_, _) => throttle.Signal();
        TranslateTouchStoryboard.Completed += (_, _) => throttle.Signal();
        PreviewMouseUp += (_, _) => { if (pointWhenMouseUp == pointWhenMouseDown) throttle.Signal(); };
        TouchMenuClosed += (_, _) => throttle.Signal();
        #endregion

        var singleClickTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        Click += (s, e) => singleClickTimer.Start();

        singleClickTimer.Tick += async (s, e) =>
        {
            singleClickTimer.Stop();
            if (pointWhenMouseUp == pointWhenMouseDown)
            {
                await fadeAnimationCompleted.ConfigureAwait(true);
                Clicked?.Invoke(s, e);
            }
        };

        MouseDoubleClick += (s, e) =>
        {
            singleClickTimer.Stop();
            e.Handled = true;
            System.Diagnostics.Process.Start("launchwinapp", "ms-virtualtouchpad:");
        };


        FadeOpacityAnimation.Freeze();
        FadeInOpacityAnimation.Freeze();
        AnimationTool.BindingAnimation(TranslateTouchStoryboard, TranslateXAnimation, this, AnimationTool.XProperty);
        AnimationTool.BindingAnimation(TranslateTouchStoryboard, TranslateYAnimation, this, AnimationTool.YProperty);
        TranslateTouchStoryboard.Begin();
        TranslateTouchStoryboard.Stop();
    }

    private AssistiveTouchPosition TouchPosition { get; set; }

    private void SaveTouchPosition() => Config.SaveAssistiveTouchPosition(Config.XmlSerializer(TouchPosition));

    private void SetAssistiveTouchProperties()
    {
        XamlResource.AssistiveTouchSize = TouchSize;
        XamlResource.SetAssistiveTouchCornerRadius(new(TouchSize / 4));
        XamlResource.SetAssistiveTouchCircleLinear(new(TouchSize >= 100 ? 2 : 1.5));
        XamlResource.SetAssistiveTouchLayerOneMargin(new(TouchSize / 8));
        XamlResource.SetAssistiveTouchLayerTwoMargin(new(TouchSize * 3 / 16));
        XamlResource.SetAssistiveTouchLayerThreeMargin(new(TouchSize / 4));

        _distance = TouchSize;
        _halfDistance = _distance / 2;
        _oneThirdDistance = _distance / 3;
        _twoThirdDistance = _oneThirdDistance * 2;
    }
    private void SetTouchPosition()
    {
        var pos = CalculateTouchMargin(TouchSize, TouchPosition, Application.Current.MainWindow);
        TouchPosTransform.SetCurrentValue(TranslateTransform.XProperty, pos.Item1);
        TouchPosTransform.SetCurrentValue(TranslateTransform.YProperty, pos.Item2);
    }

    private void WhenMouseReleased(FrameworkElement parent, double left, double top)
    {
        var parentActualHeight = parent.ActualHeight;
        var parentActualWidth = parent.ActualWidth;

        // distance of button and rifht edge
        var right = parentActualWidth - left - TouchSize;
        // distance of button and bottom edge
        var bottom = parentActualHeight - top - TouchSize;
        var verticalMiddleLine = parentActualWidth / 2;

        // determinate the position where button should be with animation
        if (left < _halfDistance && top < _twoThirdDistance) // top-left
        {
            // when the distance of button and top-left less equal than distance
            left = ButtonSpace;
            top = ButtonSpace;
            TouchPosition = AssistiveTouchPosition.UpperLeft;
        }
        else if (left < _halfDistance && bottom < _twoThirdDistance) // bottom-left
        {
            left = ButtonSpace;
            top = parentActualHeight - TouchSize - ButtonSpace;
            TouchPosition = AssistiveTouchPosition.LowerLeft;
        }
        else if (right < _halfDistance && top < _twoThirdDistance) // top-right
        {
            left = parentActualWidth - TouchSize - ButtonSpace;
            top = ButtonSpace;
            TouchPosition = AssistiveTouchPosition.UpperRight;
        }
        else if (right < _halfDistance && bottom < _twoThirdDistance) // bottom-right
        {
            left = parentActualWidth - TouchSize - ButtonSpace;
            top = parentActualHeight - TouchSize - ButtonSpace;
            TouchPosition = AssistiveTouchPosition.LowerRight;
        }
        else if (top < _twoThirdDistance) // top
        {
            top = ButtonSpace;
            TouchPosition = new()
            {
                Corner = TouchButtonCorner.Top,
                Scale = (left + ButtonSpace + (TouchSize / 2)) / parentActualWidth
            };
        }
        else if (bottom < _twoThirdDistance) // bottom
        {
            top = parentActualHeight - TouchSize - ButtonSpace;
            TouchPosition = new()
            {
                Corner = TouchButtonCorner.Bottom,
                Scale = (left + ButtonSpace + (TouchSize / 2)) / parentActualWidth
            };
        }
        else if (left + (TouchSize / 2) < verticalMiddleLine) // left
        {
            left = ButtonSpace;
            TouchPosition = new()
            {
                Corner = TouchButtonCorner.Left,
                Scale = (top + ButtonSpace + (TouchSize / 2)) / parentActualHeight
            };
        }
        else // right
        {
            left = parentActualWidth - TouchSize - ButtonSpace;
            TouchPosition = new()
            {
                Corner = TouchButtonCorner.Right,
                Scale = (top + ButtonSpace + TouchSize / 2) / parentActualHeight
            };
        }

        SmoothMoveAnimation(left, top);
    }

    private static readonly DoubleAnimation FadeOpacityAnimation = new()
    {
        From = OpacityFull,
        To = OpacityHalf,
        Duration = OpacityTransformDuration,
    };
    private static readonly DoubleAnimation FadeInOpacityAnimation = new()
    {
        From = OpacityHalf,
        To = OpacityFull,
        Duration = OpacityFadeInDuration,
    };

    private static readonly Storyboard TranslateTouchStoryboard = new();

    private static readonly DoubleAnimation TranslateXAnimation = new() { Duration = TouchReleaseToEdgeDuration };
    private static readonly DoubleAnimation TranslateYAnimation = new() { Duration = TouchReleaseToEdgeDuration };

    private static void RaiseMouseReleasedEventInCode(Button touch)
    {
        var timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;

        var mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, MouseButton.Left)
        {
            RoutedEvent = PreviewMouseUpEvent,
            Source = touch,
        };

        touch.RaiseEvent(mouseUpEvent);
    }

    private static void SmoothMoveAnimation(double left, double top)
    {
        if (TranslateXAnimation.To == left)
        {
            TranslateYAnimation.To = top;
        }
        else if (TranslateYAnimation.To == top)
        {
            TranslateXAnimation.To = left;
        }
        else
        {
            TranslateXAnimation.To = left;
            TranslateYAnimation.To = top;
        }
        TranslateTouchStoryboard.Begin();
    }

    /// <summary>
    /// Get current AsisitiveTouch margin where it should be.
    /// </summary>
    private static (double, double) CalculateTouchMargin(
        double buttonSize, AssistiveTouchPosition touchPos, FrameworkElement parent)
    {
        var rightLineMargin = parent.ActualWidth - buttonSize - ButtonSpace;
        var bottomLineMargin = parent.ActualHeight - buttonSize - ButtonSpace;
        var verticalScaleMargin = (touchPos.Scale * parent.ActualHeight) - (buttonSize / 2) - ButtonSpace;
        var horizontalScaleMargin = (touchPos.Scale * parent.ActualWidth) - buttonSize / 2 - ButtonSpace;
        return touchPos.Corner switch
        {
            TouchButtonCorner.UpperLeft => (ButtonSpace, ButtonSpace),
            TouchButtonCorner.UpperRight => (rightLineMargin, ButtonSpace),
            TouchButtonCorner.LowerLeft => (ButtonSpace, bottomLineMargin),
            TouchButtonCorner.LowerRight => (rightLineMargin, bottomLineMargin),
            TouchButtonCorner.Left => (ButtonSpace, verticalScaleMargin),
            TouchButtonCorner.Top => (horizontalScaleMargin, ButtonSpace),
            TouchButtonCorner.Right => (rightLineMargin, verticalScaleMargin),
            TouchButtonCorner.Bottom => (horizontalScaleMargin, bottomLineMargin),
            _ => default,
        };
    }
}

public record struct AssistiveTouchPosition(TouchButtonCorner Corner, double Scale = 0)
{
    public static readonly AssistiveTouchPosition Default = new(default, 0.5);
    public static readonly AssistiveTouchPosition UpperLeft = new(TouchButtonCorner.UpperLeft);
    public static readonly AssistiveTouchPosition LowerLeft = new(TouchButtonCorner.LowerLeft);
    public static readonly AssistiveTouchPosition UpperRight = new(TouchButtonCorner.UpperRight);
    public static readonly AssistiveTouchPosition LowerRight = new(TouchButtonCorner.LowerRight);
}

public enum TouchButtonCorner
{
    Left,
    Top,
    Right,
    Bottom,
    UpperLeft,
    UpperRight,
    LowerLeft,
    LowerRight
}