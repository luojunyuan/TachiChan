using SharpDX.XInput;
using System.Runtime.InteropServices;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Gamepad;

public class GameController
{
    private readonly System.Timers.Timer? _timer;

    public bool IsConnected { get; private set; }

    public void Start() => _timer?.Start();

    public void LoadConfig(string engine)
    {
        if (engine == "default")
            return;

        if (engine == "kirikir")
        {

        }
    }

    // Source Key -> (Any Engine) -> do something, event or simulate different keyboard mapping

    private static IntPtr GameWindowHandle { get; set; }

    public GameController(int handle)
    {
        GameWindowHandle = (IntPtr)handle;
        var controller = new Controller(SharpDX.Win32.UserIndex.One);
        if (!controller.IsConnected)
            return;
        IsConnected = true;

        _controller = controller;
        previousState = controller.GetState().Gamepad;

        _timer = new System.Timers.Timer(33.33);
        _timer.Elapsed += (_, _) => StartGamepadMonitoring();
        _timer.AutoReset = true;
    }

    private SharpDX.XInput.Gamepad previousState;

    private Dictionary<GamepadButtonFlags, Simulate.KeyCode> ActionMapping = new() // default 
        {
            { GamepadButtonFlags.A, Simulate.KeyCode.Enter },
            { GamepadButtonFlags.B, Simulate.KeyCode.Space },
            { GamepadButtonFlags.X, Simulate.KeyCode.None },
            { GamepadButtonFlags.Y, Simulate.KeyCode.None },
            { GamepadButtonFlags.DPadLeft, Simulate.KeyCode.Left },
            { GamepadButtonFlags.DPadUp, Simulate.KeyCode.Up },
            { GamepadButtonFlags.DPadRight, Simulate.KeyCode.Right },
            { GamepadButtonFlags.DPadDown, Simulate.KeyCode.Down },
            { GamepadButtonFlags.LeftShoulder, Simulate.KeyCode.None },
            { GamepadButtonFlags.RightShoulder, Simulate.KeyCode.Control },
            { GamepadButtonFlags.Start, Simulate.KeyCode.None },
            { GamepadButtonFlags.Back, Simulate.KeyCode.None },
            { GamepadButtonFlags.LeftThumb, Simulate.KeyCode.None },
            { GamepadButtonFlags.RightThumb, Simulate.KeyCode.None },
        };

    private readonly Controller? _controller;
    private void StartGamepadMonitoring()
    {
        var state = _controller!.GetState().Gamepad;

        foreach (var btn in ActionMapping.Keys)
        {
            if (state.Buttons.HasFlag(btn) && !previousState.Buttons.HasFlag(btn))
            {
                ActionDownImplement(btn);
            }
            else if (!state.Buttons.HasFlag(btn) && previousState.Buttons.HasFlag(btn))
            {
                ActionUpImplement(btn);
            }
        }

        previousState = state;
    }

    public event EventHandler? OpenMenu;

    private void ActionDownImplement(GamepadButtonFlags btn)
    {
        var key = ActionMapping[btn];
        if (key != Simulate.KeyCode.None && IsGameWindow())
            Simulate.Down(key);
    }
    private void ActionUpImplement(GamepadButtonFlags btn)
    {
        if (btn == GamepadButtonFlags.Back)
        {
            OpenMenu?.Invoke(this, new EventArgs());
            return;
        }

        var key = ActionMapping[btn];
        if (key != Simulate.KeyCode.None && IsGameWindow())
            Simulate.Up(key);
    }


    private static bool IsGameWindow() => GetForegroundWindow() == GameWindowHandle;

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

}


//https://msdn.microsoft.com/en-us/library/windows/apps/windows.gaming.input.gamepadbuttons.aspx

//// always null
//var controller = Gamepad.Gamepads.FirstOrDefault();
//if (controller == null)
//    return;
//previousReading = controller.GetCurrentReading();

//private Border _center { get; set; } = new Border()
//{
//    Background = new SolidColorBrush() { Color = Colors.Red, Opacity = 0.6 },
//    VerticalAlignment = VerticalAlignment.Center,
//    HorizontalAlignment = HorizontalAlignment.Center,
//    Width = 160,
//    Height = 160,
//    CornerRadius = new(8),
//};

//private GamepadReading previousReading;

//var controller = Gamepad.Gamepads[0];
//var reading = controller.GetCurrentReading();
//if (reading.Buttons.HasFlag(GamepadButtons.A) && !previousReading.Buttons.HasFlag(GamepadButtons.A))
//    Simulate.Down(Simulate.KeyCode.Enter);
//if (!reading.Buttons.HasFlag(GamepadButtons.A) && previousReading.Buttons.HasFlag(GamepadButtons.A))
//    Simulate.Up(Simulate.KeyCode.Enter);

//if (reading.Buttons.HasFlag(GamepadButtons.B) && !previousReading.Buttons.HasFlag(GamepadButtons.B))
//    Simulate.Down(Simulate.KeyCode.Space);
//if (!reading.Buttons.HasFlag(GamepadButtons.B) && previousReading.Buttons.HasFlag(GamepadButtons.B))
//    Simulate.Up(Simulate.KeyCode.Space);

//if (reading.Buttons.HasFlag(GamepadButtons.RightShoulder) && !previousReading.Buttons.HasFlag(GamepadButtons.RightShoulder))
//    Simulate.Down(Simulate.KeyCode.Control);
//if (!reading.Buttons.HasFlag(GamepadButtons.RightShoulder) && previousReading.Buttons.HasFlag(GamepadButtons.RightShoulder))
//    Simulate.Up(Simulate.KeyCode.Control);

//if (reading.Buttons.HasFlag(GamepadButtons.DPadUp) && !reading.Buttons.HasFlag(GamepadButtons.DPadUp))
//    Simulate.Down(Simulate.KeyCode.Up);
//if (!reading.Buttons.HasFlag(GamepadButtons.DPadUp) && previousReading.Buttons.HasFlag(GamepadButtons.DPadUp))
//    Simulate.Up(Simulate.KeyCode.Up);

//if (reading.Buttons.HasFlag(GamepadButtons.DPadDown) && !previousReading.Buttons.HasFlag(GamepadButtons.DPadDown))
//    Simulate.Down(Simulate.KeyCode.Down);
//if (!reading.Buttons.HasFlag(GamepadButtons.DPadDown) && previousReading.Buttons.HasFlag(GamepadButtons.DPadDown))
//    Simulate.Up(Simulate.KeyCode.Down);

//if (reading.Buttons.HasFlag(GamepadButtons.DPadLeft) && !previousReading.Buttons.HasFlag(GamepadButtons.DPadLeft))
//    Simulate.Down(Simulate.KeyCode.Left);
//if (!reading.Buttons.HasFlag(GamepadButtons.DPadLeft) && previousReading.Buttons.HasFlag(GamepadButtons.DPadLeft))
//    Simulate.Up(Simulate.KeyCode.Left);

//if (reading.Buttons.HasFlag(GamepadButtons.DPadRight) && !previousReading.Buttons.HasFlag(GamepadButtons.DPadRight))
//    Simulate.Down(Simulate.KeyCode.Right);
//if (!reading.Buttons.HasFlag(GamepadButtons.DPadRight) && previousReading.Buttons.HasFlag(GamepadButtons.DPadRight))
//    Simulate.Up(Simulate.KeyCode.Right);

//previousReading = reading;
