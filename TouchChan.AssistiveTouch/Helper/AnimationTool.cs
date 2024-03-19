using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TouchChan.AssistiveTouch.Helper;

internal static class AnimationTool
{
    private static TimeSpan TransformDuration => TouchButton.MenuTransistDuration;

    // Use with storyboard and no TransformGroup
    public static readonly PropertyPath XProperty = new("(UIElement.RenderTransform).(TranslateTransform.X)");
    public static readonly PropertyPath YProperty = new("(UIElement.RenderTransform).(TranslateTransform.Y)");

    public static DoubleAnimation FadeOutAnimation => new()
    {
        From = 1.0,
        To = 0.0,
        Duration = TransformDuration,
        FillBehavior = FillBehavior.Stop,
    };
    public static DoubleAnimation FadeInAnimation => new()
    {
        From = 0.0,
        To = 1.0,
        Duration = TransformDuration,
        FillBehavior = FillBehavior.Stop,
    };

    // The status of menu item at animation's beginning
    public static TranslateTransform ZeroTransform => new(0, 0);
    public static TranslateTransform LeftOneTransform(double distance) => new(distance, 0);
    public static TranslateTransform LeftTwoTransform(double distance) => new(distance * 2, 0);
    public static TranslateTransform TopOneTransform(double distance) => new(0, distance);
    public static TranslateTransform RightOneTransform(double distance) => new(-distance, 0);
    public static TranslateTransform RightTwoTransform(double distance) => new(-distance * 2, 0);
    public static TranslateTransform BottomOneTransform(double distance) => new(0, -distance);
    public static TranslateTransform BottomTwoTransform(double distance) => new(0, -distance * 2);

    public static TranslateTransform LeftOneBottomOneTransform(double distance) => new(distance, -distance);
    public static TranslateTransform LeftOneBottomTwoTransform(double distance) => new(distance, -distance * 2);
    public static TranslateTransform LeftOneTopOneTransform(double distance) => new(distance, distance);
    public static TranslateTransform LeftTwoTopOneTransform(double distance) => new(distance * 2, distance);
    public static TranslateTransform LeftTwoBottomOneTransform(double distance) => new(distance * 2, -distance);
    public static TranslateTransform RightOneTopOneTransform(double distance) => new(-distance, distance);
    public static TranslateTransform RightOneBottomOneTransform(double distance) => new(-distance, -distance);
    public static TranslateTransform RightOneBottomTwoTransform(double distance) => new(-distance, -distance * 2);
    public static TranslateTransform RightTwoTopOneTransform(double distance) => new(-distance * 2, distance);


    /// <summary>
    /// Used by menu items
    /// </summary>
    public static DoubleAnimation TransformMoveToZeroAnimation => new()
    {
        To = 0.0,
        Duration = TransformDuration,
        FillBehavior = FillBehavior.Stop,
    };

    /// <summary>
    /// Binding animations in storyboard
    /// </summary>
    public static void BindingAnimation(
        Storyboard storyboard,
        AnimationTimeline animation,
        DependencyObject target,
        PropertyPath path,
        bool freeze = false)
    {
        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, path);
        if (freeze)
            animation.Freeze();
        storyboard.Children.Add(animation);
    }
}
