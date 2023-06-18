using System.Diagnostics;

namespace ErogeHelper
{
    internal class ProcessStart
    {
        public static void StartMagTouch(int pid, IntPtr gameWindowHandle)
        {
            const string MagTouchSystemPath = @"C:\Windows\ErogeHelper.MagTouch.exe";

            if (!File.Exists(MagTouchSystemPath))
            {
                MessageBox.Show("Please install MagTouch first.", "ErogeHelper");
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
                MessageBox.Show("Error with Launching ErogeHelper.MagTouch.exe\r\n" +
                    "\r\n" +
                    "Please check it installed properly. ErogeHelper would continue run.\r\n" +
                    "\r\n" +
                    ex.Message,
                    "ErogeHelper");
                return;
            }
        }

        public static void GlobalKeyHook(int pid, IntPtr gameWindowHandle)
        {
            var KeyboardHooker = Path.Combine(AppContext.BaseDirectory, "ErogeHelper.KeyMapping.exe");

            if (!File.Exists(KeyboardHooker))
            {
                MessageBox.Show("ErogeHelper.KeyMapping.exe not exist.", "ErogeHelper");
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
                MessageBox.Show("Error with Launching ErogeHelper.KeyMapping.exe\r\n" +
                    ex.Message,
                    "ErogeHelper");
                return;
            }
        }

    }
}
