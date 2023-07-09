using TouchChan.AssistiveTouch.Gesture.Common;

namespace TouchChan.AssistiveTouch.Gesture.Input;

public interface IPointCapture
{
    event PointsCapturedEventHandler AfterPointsCaptured;
    event PointsCapturedEventHandler BeforePointsCaptured;
    event PointsCapturedEventHandler CaptureStarted;
    event EventHandler CaptureEnded;
    event RecognitionEventHandler GestureRecognized;
    event PointsCapturedEventHandler PointCaptured;
    bool TemporarilyDisableCapture { get; set; }
    CaptureState State { get; set; }
}

public delegate void PointsCapturedEventHandler(object sender, PointsCapturedEventArgs e);
public class PointsCapturedEventArgs : EventArgs
{
    #region Constructors

    public PointsCapturedEventArgs(List<Point> capturePoint)
    {
        this.FirstCapturedPoints = capturePoint;
        this.Points = new List<List<Point>>(capturePoint.Count);
        for (int i = 0; i < capturePoint.Count; i++)
        {
            this.Points.Add(new List<Point>(1));
            this.Points[i].Add(capturePoint[i]);
        }
    }

    public PointsCapturedEventArgs(List<List<Point>> points, List<Point> capturePoint)
    {
        this.Points = points;
        this.FirstCapturedPoints = capturePoint;
    }

    #endregion

    #region Public Properties

    public List<List<Point>> Points { get; set; }
    public List<Point> FirstCapturedPoints { get; set; }
    public bool Cancel { get; set; }
    public int BlockTouchInputThreshold { get; set; }

    #endregion
}

public delegate void RecognitionEventHandler(object sender, RecognitionEventArgs e);
public class RecognitionEventArgs : EventArgs
{
    #region Constructors

    public RecognitionEventArgs(List<List<Point>> points, List<Point> capturePoints, List<int> contactIdentifiers)
    {
        this.Points = points;
        this.FirstCapturedPoints = capturePoints;
        ContactIdentifiers = contactIdentifiers;
    }

    public RecognitionEventArgs(string gestureName, List<List<Point>> points, List<Point> capturePoints, List<int> contactIdentifiers)
        : this(points, capturePoints, contactIdentifiers)
    {
        this.GestureName = gestureName;
    }

    #endregion

    #region Public Instance Properties

    public string GestureName { get; set; }
    public List<List<Point>> Points { get; set; }
    public List<Point> FirstCapturedPoints { get; set; }
    public List<int> ContactIdentifiers { get; set; }

    #endregion
}