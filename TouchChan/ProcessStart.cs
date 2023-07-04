using System.Diagnostics;

namespace TouchChan
{
    internal class ProcessStart
    {
        public static void StartMagTouch(int pid, IntPtr gameWindowHandle)
        {
            const string MagTouchSystemPath = @"C:\Windows\TouchChan.MagTouch.exe";

            if (!File.Exists(MagTouchSystemPath))
            {
                MessageBox.Show("Please install MagTouch first.", "TouchChan");
                return;
            }

            try
            {
                // Send current pid and App.GameWindowHandle
                Process.Start(new ProcessStartInfo()
                {
                    FileName = MagTouchSystemPath,
                    Arguments = pid + " " + gameWindowHandle.ToString(),
                    Verb = "runas",
                });
            }
            catch (SystemException ex)
            {
                MessageBox.Show("Error with Launching TouchChan.MagTouch.exe\r\n" +
                    "\r\n" +
                    "Please check it installed properly. TouchChan would continue run.\r\n" +
                    "\r\n" +
                    ex.Message,
                    "TouchChan");
                return;
            }
        }

        public static void GlobalKeyHook(int pid, IntPtr gameWindowHandle)
        {
            var KeyboardHooker = Path.Combine(AppContext.BaseDirectory, "TouchChan.KeyMapping.exe");

            if (!File.Exists(KeyboardHooker))
            {
                MessageBox.Show("TouchChan.KeyMapping.exe not exist.", "TouchChan");
                return;
            }

            try
            {
                // Send current pid and App.GameWindowHandle
                Process.Start(new ProcessStartInfo()
                {
                    FileName = KeyboardHooker,
                    Arguments = pid + " " + gameWindowHandle.ToString(),
                });
            }
            catch (SystemException ex)
            {
                MessageBox.Show("Error with Launching TouchChan.KeyMapping.exe\r\n" +
                    ex.Message,
                    "TouchChan");
                return;
            }
        }

    }
}
