using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace TouchChan.AssistiveTouch.Gesture.WinForms
{
    /// <summary>
    /// Represents a display device or multiple display devices on a single system.
    /// </summary>
    public class Screen
    {
        // References:
        // http://referencesource.microsoft.com/#System.Windows.Forms/ndp/fx/src/winforms/Managed/System/WinForms/Screen.cs
        // http://msdn.microsoft.com/en-us/library/windows/desktop/dd145072.aspx
        // http://msdn.microsoft.com/en-us/library/windows/desktop/dd183314.aspx

        /// <summary>
        /// Indicates if we have more than one monitor.
        /// </summary>
        private static readonly bool MultiMonitorSupport;

        // This identifier is just for us, so that we don't try to call the multimon
        // functions if we just need the primary monitor... this is safer for
        // non-multimon OSes.
        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);

        private const int MONITORINFOF_PRIMARY = 0x00000001;

        /// <summary>
        /// The monitor handle.
        /// </summary>
        private readonly nint monitorHandle;

        /// <summary>
        /// Initializes static members of the <see cref="Screen"/> class.
        /// </summary>
        static Screen()
        {
            MultiMonitorSupport = InteropMethods.GetSystemMetrics(InteropMethods.SystemMetric.SM_CMONITORS) != 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Screen"/> class.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        private Screen(nint monitor)
            : this(monitor, nint.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Screen"/> class.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <param name="hdc">The hdc.</param>
        private Screen(nint monitor, nint hdc)
        {
            if (InteropMethods.IsProcessDPIAware())
            {
                uint dpiX;

                try
                {
                    if (monitor == PRIMARY_MONITOR)
                    {
                        var ptr = InteropMethods.MonitorFromPoint(new InteropMethods.POINTSTRUCT(0, 0), InteropMethods.MonitorDefault.MONITOR_DEFAULTTOPRIMARY);
                        InteropMethods.GetDpiForMonitor(ptr, InteropMethods.DpiType.EFFECTIVE, out dpiX, out _);
                    }
                    else
                    {
                        InteropMethods.GetDpiForMonitor(monitor, InteropMethods.DpiType.EFFECTIVE, out dpiX, out _);
                    }
                }
                catch
                {
                    // Windows 7 fallback
                    var hr = InteropMethods.D2D1CreateFactory(InteropMethods.D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof(InteropMethods.ID2D1Factory).GUID, nint.Zero, out var factory);
                    if (hr < 0)
                    {
                        dpiX = 96;
                    }
                    else
                    {
                        factory.GetDesktopDpi(out var x, out _);
                        Marshal.ReleaseComObject(factory);
                        dpiX = (uint)x;
                    }
                }

                ScaleFactor = dpiX / 96.0;
            }

            if (!MultiMonitorSupport || monitor == PRIMARY_MONITOR)
            {
                var size = new Size(
                    InteropMethods.GetSystemMetrics(InteropMethods.SystemMetric.SM_CXSCREEN),
                    InteropMethods.GetSystemMetrics(InteropMethods.SystemMetric.SM_CYSCREEN));

                Bounds = new Rectangle(0, 0, size.Width, size.Height);
                Primary = true;
                DeviceName = "DISPLAY";
            }
            else
            {
                var info = new InteropMethods.MONITORINFOEX();

                InteropMethods.GetMonitorInfo(new HandleRef(null, monitor), info);

                Bounds = new Rectangle(
                    info.rcMonitor.left,
                    info.rcMonitor.top,
                    info.rcMonitor.right - info.rcMonitor.left,
                    info.rcMonitor.bottom - info.rcMonitor.top);
                Primary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;
                DeviceName = new string(info.szDevice).TrimEnd((char)0);
            }

            monitorHandle = monitor;
        }

        /// <summary>
        /// Gets an array of all displays on the system.
        /// </summary>
        /// <returns>An enumerable of type Screen, containing all displays on the system.</returns>
        public static IEnumerable<Screen> AllScreens
        {
            get
            {
                if (MultiMonitorSupport)
                {
                    var closure = new MonitorEnumCallback();
                    var proc = new InteropMethods.MonitorEnumProc(closure.Callback);
                    InteropMethods.EnumDisplayMonitors(InteropMethods.NullHandleRef, null, proc, nint.Zero);
                    if (closure.Screens.Count > 0)
                    {
                        return closure.Screens.Cast<Screen>();
                    }
                }

                return new[] { new Screen(PRIMARY_MONITOR) };
            }
        }

        /// <summary>
        /// Gets the primary display.
        /// </summary>
        /// <returns>The primary display.</returns>
        public static Screen PrimaryScreen
        {
            get
            {
                return MultiMonitorSupport ? AllScreens.FirstOrDefault(t => t.Primary) : new Screen(PRIMARY_MONITOR);
            }
        }

        /// <summary>
        /// Gets the bounds of the display in units.
        /// </summary>
        /// <returns>A <see cref="T:System.Windows.Rect" />, representing the bounds of the display in units.</returns>
        public Rectangle WpfBounds =>
            ScaleFactor.Equals(1.0)
                ? Bounds
                : new Rectangle(
                    (int)(Bounds.X / ScaleFactor),
                    (int)(Bounds.Y / ScaleFactor),
                    (int)(Bounds.Width / ScaleFactor),
                    (int)(Bounds.Height / ScaleFactor));

        /// <summary>
        /// Gets the device name associated with a display.
        /// </summary>
        /// <returns>The device name associated with a display.</returns>
        public string DeviceName { get; }

        /// <summary>
        /// Gets the bounds of the display in pixels.
        /// </summary>
        /// <returns>A <see cref="T:System.Windows.Rect" />, representing the bounds of the display in pixels.</returns>
        public Rectangle Bounds { get; }

        /// <summary>
        /// Gets a value indicating whether a particular display is the primary device.
        /// </summary>
        /// <returns>true if this display is primary; otherwise, false.</returns>
        public bool Primary { get; }

        /// <summary>
        /// Gets the scale factor of the display.
        /// </summary>
        /// <returns>The scale factor of the display.</returns>
        public double ScaleFactor { get; } = 1.0;

        /// <summary>
        /// Gets the working area of the display. The working area is the desktop area of the display, excluding task bars,
        /// docked windows, and docked tool bars in pixels.
        /// </summary>
        /// <returns>A <see cref="T:System.Windows.Rect" />, representing the working area of the display in pixels.</returns>
        public Rectangle WorkingArea
        {
            get
            {
                Rectangle workingArea;

                if (!MultiMonitorSupport || monitorHandle == PRIMARY_MONITOR)
                {
                    var rc = new InteropMethods.RECT();

                    InteropMethods.SystemParametersInfo(InteropMethods.SPI.SPI_GETWORKAREA, 0, ref rc, InteropMethods.SPIF.SPIF_SENDCHANGE);

                    workingArea = new Rectangle(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
                }
                else
                {
                    var info = new InteropMethods.MONITORINFOEX();
                    InteropMethods.GetMonitorInfo(new HandleRef(null, monitorHandle), info);

                    workingArea = new Rectangle(info.rcWork.left, info.rcWork.top, info.rcWork.right - info.rcWork.left, info.rcWork.bottom - info.rcWork.top);
                }

                return workingArea;
            }
        }

        /// <summary>
        /// Gets the working area of the display. The working area is the desktop area of the display, excluding task bars,
        /// docked windows, and docked tool bars in units.
        /// </summary>
        /// <returns>A <see cref="T:System.Windows.Rect" />, representing the working area of the display in units.</returns>
        public Rectangle WpfWorkingArea =>
            ScaleFactor.Equals(1.0)
                ? WorkingArea
                : new Rectangle(
                    (int)(WorkingArea.X / ScaleFactor),
                    (int)(WorkingArea.Y / ScaleFactor),
                    (int)(WorkingArea.Width / ScaleFactor),
                    (int)(WorkingArea.Height / ScaleFactor));

        /// <summary>
        /// Retrieves a Screen for the display that contains the largest portion of the specified control.
        /// </summary>
        /// <param name="hwnd">The window handle for which to retrieve the Screen.</param>
        /// <returns>
        /// A Screen for the display that contains the largest region of the object. In multiple display environments
        /// where no display contains any portion of the specified window, the display closest to the object is returned.
        /// </returns>
        public static Screen FromHandle(nint hwnd)
        {
            return MultiMonitorSupport
                       ? new Screen(InteropMethods.MonitorFromWindow(new HandleRef(null, hwnd), 2))
                       : new Screen(PRIMARY_MONITOR);
        }

        /// <summary>
        /// Retrieves a Screen for the display that contains the specified point in pixels.
        /// </summary>
        /// <param name="point">A <see cref="T:System.Windows.Point" /> that specifies the location for which to retrieve a Screen.</param>
        /// <returns>
        /// A Screen for the display that contains the point in pixels. In multiple display environments where no display contains
        /// the point, the display closest to the specified point is returned.
        /// </returns>
        public static Screen FromPoint(Point point)
        {
            if (MultiMonitorSupport)
            {
                var pt = new InteropMethods.POINTSTRUCT(point.X, point.Y);
                return new Screen(InteropMethods.MonitorFromPoint(pt, InteropMethods.MonitorDefault.MONITOR_DEFAULTTONEAREST));
            }

            return new Screen(PRIMARY_MONITOR);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the specified object is equal to this Screen.
        /// </summary>
        /// <param name="obj">The object to compare to this Screen.</param>
        /// <returns>true if the specified object is equal to this Screen; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Screen monitor)
            {
                if (monitorHandle == monitor.monitorHandle)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Computes and retrieves a hash code for an object.
        /// </summary>
        /// <returns>A hash code for an object.</returns>
        public override int GetHashCode()
        {
            return monitorHandle.GetHashCode();
        }

        /// <summary>
        /// The monitor enum callback.
        /// </summary>
        private class MonitorEnumCallback
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MonitorEnumCallback"/> class.
            /// </summary>
            public MonitorEnumCallback()
            {
                Screens = new ArrayList();
            }

            /// <summary>
            /// Gets the screens.
            /// </summary>
            public ArrayList Screens { get; }

            public bool Callback(nint monitor, nint hdc, nint lprcMonitor, nint lparam)
            {
                Screens.Add(new Screen(monitor, hdc));
                return true;
            }
        }
    }
}