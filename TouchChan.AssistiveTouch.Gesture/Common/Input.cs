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
