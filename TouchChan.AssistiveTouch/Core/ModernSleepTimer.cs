using TouchChan.AssistiveTouch.Helper;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Core
{
    internal class ModernSleepTimer
    {
        public static void Start()
        {
            var newInputInfo = User32.LASTINPUTINFO.Default;
            User32.GetLastInputInfo(ref newInputInfo);
            var count = newInputInfo.dwTime;

            var sleep = new Throttle(1 * 60 * 1000, () =>
            {
                User32.GetLastInputInfo(ref newInputInfo);
                if (count == newInputInfo.dwTime)
                {
                    User32.SendMessage(0xFFFF, 0x112, 0xF170, 2);
                }
                count = newInputInfo.dwTime;
            });

            var modernSleepTimer = new System.Timers.Timer();
            modernSleepTimer.Interval = 5 * 60 * 1000;
            modernSleepTimer.Elapsed += (_, _) => sleep.Signal();
            modernSleepTimer.Start();
        }
    }
}
