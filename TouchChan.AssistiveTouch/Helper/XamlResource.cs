using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TouchChan.AssistiveTouch.Helper;

internal class XamlResource
{
    public static ResourceDictionary GetI18nDictionary()
    {
        var xamlName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "zh" => "Lang_cn.xaml",
            "en" => "Lang_en.xaml",
            _ => "Language.xaml", // ja
        };

        return new ResourceDictionary
        {
            Source = new Uri($"/TouchChan.AssistiveTouch;component/Properties/{xamlName}", UriKind.RelativeOrAbsolute)
        };
    }

    public static string GetString(string text) => (string)Application.Current.Resources[text];

    public static Visibility MenuItemTextVisible
    {
        get => (Visibility)Application.Current.Resources["MenuItemTextVisible"];
        set => Application.Current.Resources["MenuItemTextVisible"] = value;
    }

    public static double AssistiveTouchSize
    {
        get => (double)Application.Current.Resources["AssistiveTouchSize"];
        set => Application.Current.Resources["AssistiveTouchSize"] = value;
    }

    public static ControlTemplate AssistiveTouchTemplate =>
        (ControlTemplate)Application.Current.Resources["AssistiveTouchTemplate"];

    public static SolidColorBrush AssistiveTouchBackground =>
        (SolidColorBrush)Application.Current.Resources["AssistiveTouchBackground"];
    public static Thickness AssistiveTouchMenuPadding =>
        (Thickness)Application.Current.Resources["AssistiveTouchMenuPadding"];

    public static void SetAssistiveTouchSize(double value) =>
        Application.Current.Resources["AssistiveTouchSize"] = value;
    public static void SetAssistiveTouchCornerRadius(CornerRadius value) =>
        Application.Current.Resources["AssistiveTouchCornerRadius"] = value;
    public static void SetAssistiveTouchCircleLinear(Thickness value) =>
        Application.Current.Resources["AssistiveTouchCircleLinear"] = value;
    public static void SetAssistiveTouchLayerOneMargin(Thickness value) =>
        Application.Current.Resources["AssistiveTouchLayerOneMargin"] = value;
    public static void SetAssistiveTouchLayerTwoMargin(Thickness value) =>
        Application.Current.Resources["AssistiveTouchLayerTwoMargin"] = value;
    public static void SetAssistiveTouchLayerThreeMargin(Thickness value) =>
        Application.Current.Resources["AssistiveTouchLayerThreeMargin"] = value;

    public static void SetAssistiveTouchItemSize(double value) =>
        Application.Current.Resources["AssistiveTouchItemSize"] = value;
    public static void SetAssistiveTouchItemBackground(SolidColorBrush value) =>
        Application.Current.Resources["AssistiveTouchItemBackground"] = value;
}
