﻿using System.Diagnostics;
using System.Windows;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Core.Startup;

internal static class TouchGestureHooker
{
    public static void Start(string pipeHandle)
    {
#if !NET472
        var gestureHooker = "..\\TouchChan.AssistiveTouch.Gesture\\TouchChan.AssistiveTouch.Gesture.exe";
        var pid = Environment.ProcessId;
#else
        var gestureHooker = "TouchChan.AssistiveTouch.Gesture.exe";
        var pid = Process.GetCurrentProcess().Id;
#endif
        try
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = gestureHooker,
                Arguments = pipeHandle + " " + pid,
#if NET472
                UseShellExecute = false // unless handle would fail
#endif
            });
        }
        catch (SystemException)
        {
            //MessageBox.Show($"Error while Launching TouchChan.AssistiveTouch.Gesture.exe{Environment.NewLine}" + ex.Message, "TachiChan");
            return;
        }
    }

    public static void SendRightClick(Point p)
    {
        if (User32.GetForegroundWindow() != App.GameWindowHandle)
            return;

        User32.SetCursorPos((int)p.X, (int)p.Y);
        Simulate.Pretend(Simulate.ButtonCode.Right);
    }

    public static void SendSpaceKey()
    {
        if (User32.GetForegroundWindow() != App.GameWindowHandle)
            return;

        Simulate.Pretend(Simulate.KeyCode.Space);
    }
}
