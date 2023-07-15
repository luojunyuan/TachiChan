using System.Runtime.InteropServices;
using TouchChan.AssistiveTouch.Helper;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Core
{
    internal class ModernSleepTimer
    {

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        public static void Start()
        {
            var sleep = new Throttle(3 * 60 * 1000, () =>
            {
                SetSuspendState(false, false, false);
                // 2
                //var HWND_BROADCAST = 0xFFFF;
                //var WM_SYSCOMMAND = 0x112;
                //var SC_MONITORPOWER = 0xF170;
                //User32.SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2); // MONITOR_OFF
                // 3
                //WindowsInput.Simulate.Events()
                //    .ClickChord(KeyCode.LWin, KeyCode.X)
                //    .Wait(5000)
                //    .Click(KeyCode.U)
                //    .Click(KeyCode.S)
                //    .Invoke();

            });
            sleep.Signal();

            var newInputInfo = User32.LASTINPUTINFO.Default;
            User32.GetLastInputInfo(ref newInputInfo);
            var count = newInputInfo.dwTime;

            var modernSleepTimer = new System.Timers.Timer();
            modernSleepTimer.Interval = 1 * 30 * 1000;
            modernSleepTimer.Elapsed += (_, _) =>
            {
                User32.GetLastInputInfo(ref newInputInfo);
                if (count != newInputInfo.dwTime)
                {
                    sleep.Signal();
                }
                count = newInputInfo.dwTime;
            };
            modernSleepTimer.Start();
        }
    }
}
