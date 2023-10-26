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
        catch (SystemException)
        {
            //MessageBox.Show($"Error while Launching TouchChan.AssistiveTouch.Gamepad.exe{Environment.NewLine}" + ex.Message, "TachiChan");
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
            var engine = new TextBlock() { Text = $"Engine: {App.GameEngine}" };
            var padLeft = new TextBlock() { Text = "DPad Left: ←" };
            var padUp = new TextBlock() { Text = "DPad Up: ↑" };
            var padRight = new TextBlock() { Text = "DPad Right: →" };
            var padDown = new TextBlock() { Text = "DPad Down: ↓" };
            var r1 = new TextBlock() { Text = "R1: Ctrl" };
            var a = new TextBlock() { Text = "A: Enter" };
            var b = new TextBlock() { Text = "B: Space" };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(engine);
            stackPanel.Children.Add(padLeft);
            stackPanel.Children.Add(padUp);
            stackPanel.Children.Add(padRight);
            stackPanel.Children.Add(padDown);
            stackPanel.Children.Add(r1);
            stackPanel.Children.Add(a);
            stackPanel.Children.Add(b);

            border = new Border()
            {
                Background = new SolidColorBrush() { Color = Colors.White, Opacity = 0.6 },
                Child = stackPanel,
            };
        });
        return border;
    }
}
