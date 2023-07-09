//using Microsoft.Win32;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//namespace TouchChan.AssistiveTouch.Gesture.Input
//{
//    public class PointCapture : IPointCapture, IDisposable
//    {
//        public static PointCapture Instance { get => _Instance; }

//        // SourceDevice == Devices.TouchScreen

//        #region Private Variables

//        private const uint WINEVENT_OUTOFCONTEXT = 0;
//        private const uint EVENT_SYSTEM_FOREGROUND = 3;
//        private const uint WINEVENT_SKIPOWNPROCESS = 0x0002; // Don't call back for events on installer's process
//        private const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;

//        // Create new Touch hook control to capture global input from Touch, and create an event translator to get formal events
//        private readonly PointEventTranslator _pointEventTranslator;
//        private readonly InputProvider _inputProvider;
//        private readonly List<IPointPattern> _pointPatternCache = new List<IPointPattern>();
//        private readonly System.Threading.Timer _blockTouchDelayTimer;

//        private System.Threading.Timer _initialTimeoutTimer;
//        SynchronizationContext _currentContext;

//        private Dictionary<int, List<Point>> _pointsCaptured;
//        // Create variable to hold the only allowed instance of this class
//        static readonly PointCapture _Instance = new PointCapture();

//        //private CaptureMode _mode = CaptureMode.Normal;
//        private volatile CaptureState _state;

//        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

//        readonly WinEventDelegate _winEventDele;
//        private readonly IntPtr _hWinEventHook;
//        private GCHandle _winEventGch;

//        private bool disposedValue = false; // To detect redundant calls

//        private int? _blockTouchInputThreshold;
//        private Point _touchPadStartPoint;

//        #endregion

//        #region PInvoke 

//        [DllImport("user32.dll")]
//        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

//        [DllImport("user32.dll")]
//        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

//        #endregion

//        #region Public Instance Properties

//        public Devices SourceDevice { get { return _pointEventTranslator.SourceDevice; } }

//        public bool TemporarilyDisableCapture { get; set; }

//        public List<Point>[] InputPoints
//        {
//            get
//            {
//                if (_pointsCaptured == null)
//                    return new List<Point>[0];
//                return _pointsCaptured.Values.ToArray();
//            }
//        }

//        public CaptureState State
//        {
//            get { return _state; }
//            set { _state = value; }
//        }

//        #endregion

//        #region Custom Events

//        public event PointsCapturedEventHandler? CaptureStarted;

//        public event PointsCapturedEventHandler? AfterPointsCaptured;
//        public event PointsCapturedEventHandler? BeforePointsCaptured;
//        public event RecognitionEventHandler? GestureRecognized;


//        public event PointsCapturedEventHandler? PointCaptured;

//        public event EventHandler? CaptureEnded;


//        public event PointsCapturedEventHandler CaptureCanceled;

//        #endregion

//        protected PointCapture()
//        {
//            _inputProvider = new InputProvider();
//            _pointEventTranslator = new PointEventTranslator(_inputProvider);
//            _pointEventTranslator.PointDown += (PointEventTranslator_PointDown);
//            _pointEventTranslator.PointUp += (PointEventTranslator_PointUp);
//            _pointEventTranslator.PointMove += (PointEventTranslator_PointMove);

//            _currentContext = SynchronizationContext.Current;

//            _winEventDele = WinEventProc;
//            _winEventGch = GCHandle.Alloc(_winEventDele);
//            _hWinEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, _winEventDele, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);

//            if (AppConfig.UiAccess)
//            {
//                _blockTouchDelayTimer = new System.Threading.Timer(UpdateBlockTouchInputThresholdCallback, null, Timeout.Infinite, Timeout.Infinite);
//                ForegroundApplicationsChanged += PointCapture_ForegroundApplicationsChanged;
//            }

//            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
//        }

//        #region IDisposable Support

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposedValue)
//            {
//                if (disposing)
//                {
//                    _initialTimeoutTimer?.Dispose();
//                    _blockTouchDelayTimer?.Dispose();
//                    _inputProvider?.Dispose();
//                }

//                SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
//                if (_hWinEventHook != IntPtr.Zero)
//                    UnhookWinEvent(_hWinEventHook);
//                if (_winEventGch.IsAllocated)
//                {
//                    _winEventGch.Free();
//                }

//                disposedValue = true;
//            }
//        }

//        ~PointCapture()
//        {
//            Dispose(false);
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        #endregion

//        #region System Events

//        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
//        {
//            if (eventType == EVENT_SYSTEM_FOREGROUND || eventType == EVENT_SYSTEM_MINIMIZEEND)
//            {
//                if (State != CaptureState.Ready || Mode != CaptureMode.Normal || hwnd.Equals(IntPtr.Zero))
//                    return;
//                var systemWindow = new SystemWindow(hwnd);
//                if (!systemWindow.Visible)
//                    return;
//                var apps = ApplicationManager.Instance.GetApplicationFromWindow(systemWindow);
//                ForegroundApplicationsChanged?.Invoke(this, new ApplicationChangedEventArgs(apps));
//            }
//        }

//        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
//        {
//            switch (e.Reason)
//            {
//                case SessionSwitchReason.RemoteConnect:
//                case SessionSwitchReason.SessionLogon:
//                case SessionSwitchReason.SessionUnlock:
//                    if (State == CaptureState.Disabled)
//                        State = CaptureState.Ready;
//                    break;
//                case SessionSwitchReason.SessionLock:
//                    State = CaptureState.Disabled;
//                    break;
//                default:
//                    break;
//            }
//        }

//        #endregion

//        #region Events

//        private void PointCapture_ForegroundApplicationsChanged(object sender, ApplicationChangedEventArgs appsChanged)
//        {
//            if (appsChanged.Applications != null)
//            {
//                var userAppList = appsChanged.Applications.Where(application => application is UserApp).ToList();
//                if (userAppList.Count == 0) return;
//                UpdateBlockTouchInputThreshold(userAppList.Cast<UserApp>().Max(app => app.BlockTouchInputThreshold));
//            }
//        }

//        protected void PointEventTranslator_PointDown(object sender, InputPointsEventArgs e)
//        {
//            if (State == CaptureState.Ready || State == CaptureState.Capturing || State == CaptureState.CapturingInvalid)
//            {
//                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

//                // Try to begin capture process, if capture started then don't notify other applications of a Point event, otherwise do
//                if (!TryBeginCapture(e.InputPointList))
//                {
//                    Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
//                }
//                else e.Handled = true;
//            }
//        }

//        protected void PointEventTranslator_PointMove(object sender, InputPointsEventArgs e)
//        {
//            // Only add point if we're capturing
//            if (State == CaptureState.Capturing || State == CaptureState.CapturingInvalid)
//            {
//                AddPoint(e.InputPointList);
//            }
//            UpdateBlockTouchInputThreshold();
//        }

//        protected void PointEventTranslator_PointUp(object sender, InputPointsEventArgs e)
//        {
//            if (State == CaptureState.Capturing || State == CaptureState.CapturingInvalid && (SourceDevice & Devices.TouchDevice) != 0)
//            {
//                e.Handled = true;

//                EndCapture();

//                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
//            }
//            else if (State == CaptureState.TriggerFired)
//            {
//                State = CaptureState.Ready;
//                e.Handled = true;
//                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
//            }

//            UpdateBlockTouchInputThreshold();
//            if (_initialTimeoutTimer != null)
//                _initialTimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
//        }

//        #endregion

//        #region Private Methods

//        private void UpdateBlockTouchInputThreshold(int? threshold = null)
//        {
//            if (!AppConfig.UiAccess) return;

//            if (threshold != null)
//                _blockTouchInputThreshold = threshold;
//            if (_blockTouchInputThreshold != null)
//                _blockTouchDelayTimer.Change(100, Timeout.Infinite);
//        }

//        private void UpdateBlockTouchInputThresholdCallback(object o)
//        {
//            if (!_blockTouchInputThreshold.HasValue) return;

//            _currentContext.Post((state) =>
//            {
//                _blockTouchInputThreshold = null;
//            }, null);
//        }

//        private bool TryBeginCapture(List<InputPoint> firstPoint)
//        {
//            // Create capture args so we can notify subscribers that capture has started and allow them to cancel if they want.
//            PointsCapturedEventArgs captureStartedArgs;
//            captureStartedArgs = new PointsCapturedEventArgs(firstPoint.Select(p => p.Point).ToList());
//            OnCaptureStarted(captureStartedArgs);

//            UpdateBlockTouchInputThreshold(captureStartedArgs.BlockTouchInputThreshold);

//            if (captureStartedArgs.Cancel)
//                return false;

//            State = CaptureState.CapturingInvalid;

//            // Clear old gesture from point list so we can start adding the new captures points to the list 
//            _pointsCaptured = new Dictionary<int, List<Point>>(firstPoint.Count);
//            if (true)// AppConfig.IsOrderByLocation)
//            {
//                foreach (var rawData in firstPoint.OrderBy(p => p.Point.X))
//                {
//                    if (!_pointsCaptured.ContainsKey(rawData.ContactIdentifier))
//                        _pointsCaptured.Add(rawData.ContactIdentifier, new List<Point>(30));
//                }
//            }
//            else
//            {
//                foreach (var rawData in firstPoint.OrderBy(p => p.ContactIdentifier))
//                {
//                    if (!_pointsCaptured.ContainsKey(rawData.ContactIdentifier))
//                        _pointsCaptured.Add(rawData.ContactIdentifier, new List<Point>(30));
//                }
//            }
//            AddPoint(firstPoint);
//            return true;
//        }

//        private void EndCapture()
//        {
//            // Create points capture event args, to be used to send off to event subscribers or to simulate original Point event
//            var pointsInformation = new PointsCapturedEventArgs(new List<List<Point>>(_pointsCaptured.Values), _pointsCaptured.Values.Select(p => p.FirstOrDefault()).ToList());

//            // Notify subscribers that capture has ended （draw end）
//            OnCaptureEnded();
//            State = CaptureState.Ready;

//            // Notify PointsCaptured event subscribers that points have been captured.
//            //CaptureWindow GetGestureName
//            OnBeforePointsCaptured(pointsInformation);

//            if (pointsInformation.Cancel) return;

//            if (GestureManager.Instance.GestureName != null)
//            {
//                List<Point> capturedPoints = pointsInformation.FirstCapturedPoints;
//                OnGestureRecognized(new RecognitionEventArgs(GestureManager.Instance.GestureName, pointsInformation.Points, capturedPoints, _pointsCaptured.Keys.ToList()));
//            }

//            OnAfterPointsCaptured(pointsInformation);

//            _pointsCaptured.Clear();
//        }

//        private const int MinimumPointDistance = 20;

//        private void AddPoint(List<InputPoint> point)
//        {
//            bool getNewPoint = false;
//            int threshold = MinimumPointDistance;
//            foreach (var p in point)
//            {
//                // Don't accept point if it's within specified distance of last point unless it's the first point
//                if (_pointsCaptured.TryGetValue(p.ContactIdentifier, out List<Point> stroke))
//                {
//                    if (stroke.Count != 0)
//                    {
//                        if (PointPatternMath.GetDistance(stroke.Last(), p.Point) < threshold)
//                            continue;

//                        if (State == CaptureState.CapturingInvalid)
//                            State = CaptureState.Capturing;
//                    }

//                    getNewPoint = true;
//                    // Add point to captured points list
//                    stroke.Add(p.Point);
//                }
//            }
//            if (getNewPoint)
//            {
//                // Notify subscribers that point has been captured
//                OnPointCaptured(new PointsCapturedEventArgs(new List<List<Point>>(_pointsCaptured.Values), point.Select(p => p.Point).ToList()));
//            }
//        }

//        #endregion
//    }
//}
