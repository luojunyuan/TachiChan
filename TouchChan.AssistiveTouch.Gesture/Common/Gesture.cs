namespace TouchChan.AssistiveTouch.Gesture.Common
{
    public interface IGesture
    {
        string Name { get; set; }

        PointPattern[] PointPatterns { get; set; }
    }


    [Serializable]
    public class Gesture : IGesture
    {
        #region Constructors
        public Gesture()
        { }
        public Gesture(string name, PointPattern[] pointPatterns)
        {
            this.Name = name;
            this.PointPatterns = pointPatterns;
        }

        #endregion

        #region IPointPattern Instance Properties

        public string Name { get; set; }

        public PointPattern[] PointPatterns { get; set; }

        #endregion
    }

    public class PointPattern : IPointPattern
    {
        public PointPattern(Point[][] points)
        {
            Points = points;
        }

        public PointPattern(IEnumerable<List<Point>> points)
        {
            Points = points.Select(l => l.ToArray()).ToArray();
        }

        public Point[][] Points { get; set; }
    }
}
