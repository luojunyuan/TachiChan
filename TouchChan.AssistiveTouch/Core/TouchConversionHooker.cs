using TouchChan.AssistiveTouch.NativeMethods;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

namespace TouchChan.AssistiveTouch.Core;

public static class TouchConversionHooker
{
    private const uint MOUSEEVENTF_FROMTOUCH = 0xFF515700;

    private static IntPtr _hookId;

    public static void Install()
    {
        var moduleHandle = Kernel32.GetModuleHandle(); // get current exe instant handle

        _hookId = User32.SetWindowsHookEx(User32.HookType.WH_MOUSE_LL, Hook, moduleHandle, 0); // tid 0 set global hook
        if (_hookId == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    public static void UnInstall() => User32.UnhookWindowsHookEx(_hookId);

    private static IntPtr Hook(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);

        var info = Marshal.PtrToStructure<User32.MSLLHOOKSTRUCT>(lParam);

        var extraInfo = (uint)info.dwExtraInfo;
        if ((extraInfo & MOUSEEVENTF_FROMTOUCH) != MOUSEEVENTF_FROMTOUCH)
            return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);
        
        var isGameWindow = User32.GetForegroundWindow() == App.GameWindowHandle;
        if (!isGameWindow)
            return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);

        var win = (MainWindow)Application.Current.MainWindow;
        var winOrigin = new System.Drawing.Point();
        User32.MapWindowPoints(MainWindow.Handle, IntPtr.Zero, ref winOrigin);
        var relativePoint = new Point((info.pt.X - winOrigin.X) / win.Dpi, (info.pt.Y - winOrigin.Y) / win.Dpi);
        // Hit test if its touchchan itself
        if (VisualTreeHelper.HitTest((Grid)win.Content, relativePoint) != null 
            || relativePoint.Y < 0)
            return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);

        switch ((int)wParam)
        {
            case 0x202:
                Task.Run(() =>
                {
                    User32.SetCursorPos(info.pt.X, info.pt.Y);
                    User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN, info.pt.X, info.pt.Y, 0, IntPtr.Zero);
                    Thread.Sleep(UserTimerMinimum);
                    User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_LEFTUP, info.pt.X, info.pt.Y, 0, IntPtr.Zero);
                    // I didn't get it
                    // Simulate.Click(Simulate.ButtonCode.Left, UserTimerMinimum);
                });
                break;
            case 0x205:
                Task.Run(() =>
                {
                    User32.SetCursorPos(info.pt.X, info.pt.Y);
                    Simulate.Click(Simulate.ButtonCode.Right, UserTimerMinimum);
                });
                break;
        }

        return User32.CallNextHookEx(_hookId!, nCode, wParam, lParam);
    }

    private const int UserTimerMinimum = 0x0000000A;
}
