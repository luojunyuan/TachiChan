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
            var modernSleepTimer = new System.Timers.Timer();
            modernSleepTimer.Interval = 1 * 60 * 1000;
            modernSleepTimer.Elapsed += (_, _) =>
            {
                User32.GetLastInputInfo(ref newInputInfo);
                Console.WriteLine(newInputInfo.dwTime);
                if (count == newInputInfo.dwTime)
                {
                    User32.SendMessage(0xFFFF, 0x112, 0xF170, 2);
                }
                count = newInputInfo.dwTime;
            };
            modernSleepTimer.Start();
        }
    }
}
