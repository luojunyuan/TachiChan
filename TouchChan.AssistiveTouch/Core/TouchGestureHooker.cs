using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TouchChan.AssistiveTouch.Core
{
    internal static class TouchGestureHooker
    {
        public static void Start(string pipeHandle, int pid)
        {
            var gestureHooker = Path.Combine(AppContext.BaseDirectory, "TouchChan.AssistiveTouch.Gesture.exe");

            if (!File.Exists(gestureHooker))
            {
                MessageBox.Show("TouchChan.AssistiveTouch.Gesture.exe not exist.", "TachiChan");
                return;
            }

            try
            {
                // Send current pid and App.GameWindowHandle
                Process.Start(new ProcessStartInfo()
                {
                    FileName = gestureHooker,
                    Arguments = pipeHandle + " " + pid,
                });
            }
            catch (SystemException ex)
            {
                MessageBox.Show($"Error with Launching TouchChan.AssistiveTouch.Gesture.exe{Environment.NewLine}" +
                    ex.Message,
                    "TachiChan");
                return;
            }
        }

    }
}
