using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TouchChan.AssistiveTouch.Core.Extend;

namespace TouchChan.AssistiveTouch.Core.Startup;

internal class GameController
{
    public static void Start(string pipeHandle)
    {
#if !NET472
        var gamepad = "..\\TouchChan.AssistiveTouch.Gamepad\\TouchChan.AssistiveTouch.Gamepad.exe";
        var pid = Environment.ProcessId;
#else
        var gamepad = "TouchChan.AssistiveTouch.Gamepad.exe";
        var pid = Process.GetCurrentProcess().Id;
#endif
        try
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = gamepad,
                Arguments = pipeHandle + " " + pid + " " + App.GameWindowHandle + " " + App.GameEngine,
#if NET472
                UseShellExecute = false // unless handle would fail
#endif
            });
        }
        catch (SystemException ex)
        {
            MessageBox.Show($"Error while Launching TouchChan.AssistiveTouch.Gamepad.exe{Environment.NewLine}" +
                ex.Message,
                "TachiChan");
            return;
        }
    }

    public static void InteractTip()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (Opened)
            {
                ((Grid)(Application.Current.MainWindow.Content)).Children.Remove(ANoInteractTipBorder);
            }
            else
            {
                ((Grid)(Application.Current.MainWindow.Content)).Children.Add(ANoInteractTipBorder);
            }
        });

        Opened = !Opened;
    }

    private static bool Opened { get; set; }

    private static Border ANoInteractTipBorder { get; set; } = Init();

    private static Border Init()
    {
        Border border = null!;
        Application.Current.Dispatcher.Invoke(() =>
        {
            var text = new TextBlock() { Text = "Default Config" };

            var grid = new Grid();
            grid.Children.Add(text);

            border = new Border()
            {
                Background = new SolidColorBrush() { Color = Colors.Black, Opacity = 0.6 },
                Child = grid,
            };
        });
        return border;
    }
}
