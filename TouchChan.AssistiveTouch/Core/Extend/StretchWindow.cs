using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Core.Extend
{
    internal static class StretchWindow
    {
        private static IntPtr Menu;
        private static User32.RECT OriginalRectangle;

        //https://github1s.com/Dangetsu/vnr/blob/HEAD/Frameworks/Sakura/py/apps/reader/utilities/displayutil.py
        //https://github1s.com/Codeusa/Borderless-Gaming/blob/HEAD/BorderlessGaming.Logic/Windows/Manipulation.cs
        public static void Stretch(IntPtr window)
        {
            SetWindowBorderVisible(window, false);
            
            User32.GetWindowRect(window, out OriginalRectangle);
            var win = (MainWindow)Application.Current.MainWindow;
            var monitorWidth = SystemParameters.PrimaryScreenWidth * win.Dpi;
            var monitorHeight = SystemParameters.PrimaryScreenHeight * win.Dpi;

            User32.GetClientRect(App.GameWindowHandle, out var rectClient);
            var (clientWidth, clientHeight) = FindContentSize(rectClient.Width, rectClient.Height);
            var overWidth = monitorWidth * clientHeight;
            var overHeight = clientWidth * monitorHeight;

            var (fullScreenWidth, fullScreenHeight) = overWidth > overHeight ? 
                (monitorHeight * clientWidth / clientHeight, monitorHeight) : 
                (monitorWidth, monitorWidth * clientHeight / clientWidth);
            var left = (monitorWidth - fullScreenWidth) / 2;
            var top = (monitorHeight - fullScreenHeight) / 2;

            Menu = User32.GetMenu(window);
            if (Menu != IntPtr.Zero)
                User32.SetMenu(window, IntPtr.Zero);

            User32.SetWindowPos(window, IntPtr.Zero,
                (int)left, (int)top, (int)fullScreenWidth, (int)fullScreenHeight, User32.SetWindowPosFlags.SWP_FRAMECHANGED);
        }

        public static void Restore(IntPtr window)
        {
            SetWindowBorderVisible(window, true);
            User32.SetWindowPos(window, IntPtr.Zero,
                OriginalRectangle.left, OriginalRectangle.top,
                OriginalRectangle.Width, OriginalRectangle.Height, User32.SetWindowPosFlags.SWP_FRAMECHANGED);
            if (Menu != IntPtr.Zero)
                User32.SetMenu(window, Menu);
        }

        private static readonly double[] AspectsRatio = [4.0 / 3, 16.0 / 9];
        private static (int, int) FindContentSize(int width, int height)
        {
            double realAspect = (double)width / height;
            double aspect = AspectsRatio.OrderBy(it => Math.Abs(it - realAspect)).First();

            if (realAspect > aspect)
            {
                int newWidth = (int)(height * aspect);
                return ((int)(height * aspect), height);
            }
            else if (realAspect < aspect)
            {
                int newHeight = (int)(width / aspect);
                return (width, newHeight);
            }
            else // This seldom happens
            {
                return (width, height);
            }
        }
        
        private static void SetWindowBorderVisible(IntPtr window, bool visible)
        {
            const int WS_OVERLAPPEDWINDOW = 0x00CF0000;

            var style = User32.GetWindowLong(window, User32.WindowLongFlags.GWL_STYLE);
            style = visible == false ? (style & ~WS_OVERLAPPEDWINDOW) : style | WS_OVERLAPPEDWINDOW;
            User32.SetWindowLong(window, User32.WindowLongFlags.GWL_STYLE, style);

            User32.SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0,
                User32.SetWindowPosFlags.SWP_NOACTIVATE | User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);
        }
    }
}
