namespace TouchChan.AssistiveTouch.Gesture.Common;

public delegate void RawPointsDataMessageEventHandler(object sender, RawPointsDataMessageEventArgs e);
public class RawPointsDataMessageEventArgs : EventArgs
{
    #region Constructors

    public RawPointsDataMessageEventArgs(List<RawData> rawData, Devices device)
    {
        this.RawData = rawData;
        SourceDevice = device;
    }


    #endregion

    #region Public Properties

    public List<RawData> RawData { get; set; }
    public Devices SourceDevice { get; set; }

    #endregion
}


public record struct RawData(DeviceStates State, int ContactIdentifier, Point RawPoints);

public record struct InputPoint(int ContactIdentifier, Point Point);
public class InputPointsEventArgs : EventArgs
{
    #region Constructors

    public InputPointsEventArgs(List<InputPoint> inputPointList, Devices pointSource)
    {
        InputPointList = inputPointList;
        PointSource = pointSource;
    }

    public InputPointsEventArgs(List<RawData> rawDataList, Devices pointSource)
    {
        InputPointList = rawDataList?.Select(rd => new InputPoint(rd.ContactIdentifier, rd.RawPoints)).ToList();
        PointSource = pointSource;
    }

    #endregion

    #region Public Properties

    public List<InputPoint> InputPointList { get; set; }

    public bool Handled { get; set; }

    public Devices PointSource { get; set; }

    #endregion
}

[Flags]
public enum DeviceStates
{
    None = 0,
    Tip = 1 << 0,
    InRange = 1 << 1,
    RightClickButton = 1 << 2,
    Invert = 1 << 3,
    Eraser = 1 << 4,
}

[Flags]
public enum Devices
{
    None = 0,
    TouchScreen = 1 << 0,
    TouchPad = 1 << 1,
    Mouse = 1 << 2,
    Pen = 1 << 3,
    TouchDevice = TouchScreen | TouchPad,
}
public enum CaptureState
{
    Ready,
    Disabled,
    Capturing,
    CapturingInvalid,
    TriggerFired
}