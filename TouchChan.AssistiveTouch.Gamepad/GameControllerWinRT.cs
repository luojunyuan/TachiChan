using SharpDX.XInput;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Core.Extend;

public class GameControllerWinRT
{
    private readonly System.Timers.Timer? _timer;

    public GameControllerWinRT()
    {
        //// always null
        //var controller = Gamepad.Gamepads.FirstOrDefault();
        //if (controller == null)
        //    return;
        //previousReading = controller.GetCurrentReading();

        var controller = new Controller(UserIndex.One);
        if (!controller.IsConnected)
            return;

        previousState = controller.GetState().Gamepad;

        _timer = new System.Timers.Timer(33.33);
        _timer.Elapsed += (_, _) => StartGamepadMonitoring();
        _timer.AutoReset = true;
        _timer.Start();
    }


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
    private Gamepad previousState;

    private const int UserTimerMinimum = 0x0000000A;


    private void StartGamepadMonitoring()
    {
        var controller = new Controller(UserIndex.One);

        var state = controller.GetState().Gamepad;

        // Default

        if (state.Buttons.HasFlag(GamepadButtonFlags.A) && !previousState.Buttons.HasFlag(GamepadButtonFlags.A))
            Simulate.Down(Simulate.KeyCode.Enter);
        if (!state.Buttons.HasFlag(GamepadButtonFlags.A) && previousState.Buttons.HasFlag(GamepadButtonFlags.A))
            Simulate.Up(Simulate.KeyCode.Enter);

        if (state.Buttons.HasFlag(GamepadButtonFlags.B) && !previousState.Buttons.HasFlag(GamepadButtonFlags.B))
            Simulate.Down(Simulate.KeyCode.Space);
        if (!state.Buttons.HasFlag(GamepadButtonFlags.B) && previousState.Buttons.HasFlag(GamepadButtonFlags.B))
            Simulate.Up(Simulate.KeyCode.Space);

        // if (state.Buttons.HasFlag(GamepadButtonFlags.X) && !previousState.Buttons.HasFlag(GamepadButtonFlags.X))
        //     Simulate.Down(Simulate.KeyCode.Escape);
        // if (!state.Buttons.HasFlag(GamepadButtonFlags.X) && previousState.Buttons.HasFlag(GamepadButtonFlags.X))
        //     Simulate.Up(Simulate.KeyCode.Escape);

        // if (state.Buttons.HasFlag(GamepadButtonFlags.Y) && !previousState.Buttons.HasFlag(GamepadButtonFlags.Y))
        //     Simulate.Scroll(Simulate.ButtonCode.VScroll, Simulate.ButtonScrollDirection.Up);
        // if (!state.Buttons.HasFlag(GamepadButtonFlags.Y) && previousState.Buttons.HasFlag(GamepadButtonFlags.Y))

        if (state.Buttons.HasFlag(GamepadButtonFlags.RightShoulder) && !previousState.Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
            Simulate.Down(Simulate.KeyCode.Control);
        if (!state.Buttons.HasFlag(GamepadButtonFlags.RightShoulder) && previousState.Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
            Simulate.Up(Simulate.KeyCode.Control);

        // if (state.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) && !previousState.Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
        // Simulate.Pretend(Simulate.KeyCode.Alt, Simulate.KeyCode.Enter);
        // if (!state.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) && previousState.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder))

        // if (state.Buttons.HasFlag(GamepadButtonFlags.Start) && !previousState.Buttons.HasFlag(GamepadButtonFlags.Start))
        // if (!state.Buttons.HasFlag(GamepadButtonFlags.Start) && previousState.Buttons.HasFlag(GamepadButtonFlags.Start))

        // a tip panel
        // if (state.Buttons.HasFlag(GamepadButtonFlags.Back) && !previousState.Buttons.HasFlag(GamepadButtonFlags.Back))
        // if (!state.Buttons.HasFlag(GamepadButtonFlags.Back) && previousState.Buttons.HasFlag(GamepadButtonFlags.Back))

        if (state.Buttons.HasFlag(GamepadButtonFlags.DPadUp) && !previousState.Buttons.HasFlag(GamepadButtonFlags.DPadUp))
            Simulate.Down(Simulate.KeyCode.Up);
        if (!state.Buttons.HasFlag(GamepadButtonFlags.DPadUp) && previousState.Buttons.HasFlag(GamepadButtonFlags.DPadUp))
            Simulate.Up(Simulate.KeyCode.Up);

        if (state.Buttons.HasFlag(GamepadButtonFlags.DPadDown) && !previousState.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
            Simulate.Down(Simulate.KeyCode.Down);
        if (!state.Buttons.HasFlag(GamepadButtonFlags.DPadDown) && previousState.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
            Simulate.Up(Simulate.KeyCode.Down);

        if (state.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) && !previousState.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
            Simulate.Down(Simulate.KeyCode.Left);
        if (!state.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) && previousState.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
            Simulate.Up(Simulate.KeyCode.Left);

        if (state.Buttons.HasFlag(GamepadButtonFlags.DPadRight) && !previousState.Buttons.HasFlag(GamepadButtonFlags.DPadRight))
            Simulate.Down(Simulate.KeyCode.Right);
        if (!state.Buttons.HasFlag(GamepadButtonFlags.DPadRight) && previousState.Buttons.HasFlag(GamepadButtonFlags.DPadRight))
            Simulate.Up(Simulate.KeyCode.Right);

        // if (state.Buttons.HasFlag(GamepadButtonFlags.None) && !previousState.Buttons.HasFlag(GamepadButtonFlags.None))
        //     Console.WriteLine("None 按钮被按下");
        // if (!state.Buttons.HasFlag(GamepadButtonFlags.None) && previousState.Buttons.HasFlag(GamepadButtonFlags.None))
        //     Console.WriteLine("None 按钮被释放");

        // if (state.Buttons.HasFlag(GamepadButtonFlags.LeftThumb) && !previousState.Buttons.HasFlag(GamepadButtonFlags.LeftThumb))
        //     Simulate.Pretend(Simulate.KeyCode.LWin, Simulate.KeyCode.Tab);
        // if (!state.Buttons.HasFlag(GamepadButtonFlags.LeftThumb) && previousState.Buttons.HasFlag(GamepadButtonFlags.LeftThumb))

        // if (state.Buttons.HasFlag(GamepadButtonFlags.RightThumb) && !previousState.Buttons.HasFlag(GamepadButtonFlags.RightThumb))
        //     Simulate.Pretend(Simulate.KeyCode.LWin, Simulate.KeyCode.A);
        // if (!state.Buttons.HasFlag(GamepadButtonFlags.RightThumb) && previousState.Buttons.HasFlag(GamepadButtonFlags.RightThumb))

        previousState = state;
        //https://msdn.microsoft.com/en-us/library/windows/apps/windows.gaming.input.gamepadbuttons.aspx
    }
}
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
