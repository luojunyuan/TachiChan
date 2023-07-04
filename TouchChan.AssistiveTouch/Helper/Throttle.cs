namespace TouchChan.AssistiveTouch.Helper;

public sealed class Throttle<T>
{
    private readonly System.Timers.Timer _timer;
    private T? _value;

    public Throttle(int millisec, Action<T?> action)
    {
        _timer = new System.Timers.Timer(millisec);
        _timer.Elapsed += (_, _) => action(_value);
        _timer.AutoReset = false;
    }

    public void Signal(T input)
    {
        _value = input;
        _timer.Stop();
        _timer.Start();
    }
}

public sealed class Throttle
{
    private readonly System.Timers.Timer _timer;

    public Throttle(int millisec, Action action)
    {
        _timer = new System.Timers.Timer(millisec);
        _timer.Elapsed += (_, _) => action();
        _timer.AutoReset = false;
    }

    public void Signal()
    {
        _timer.Stop();
        _timer.Start();
    }
}
