using TouchChan.AssistiveTouch.Helper;
using TouchChan.AssistiveTouch.NativeMethods;

namespace TouchChan.AssistiveTouch.Core.Extend;

internal class ModernSleepTimer
{
    public static void Start()
    {
        const int min = 3;
        const int advanceSeconds = 5000;
        var sleep = new Throttle(min * 60 * 1000 - advanceSeconds, () =>
        {
            var HWND_BROADCAST = 0xFFFF;
            var WM_SYSCOMMAND = 0x112;
            var SC_MONITORPOWER = 0xF170;
            var MONITOR_OFF = 2;
            // FIXME: DESIRE 背徳の螺旋 remaster ver. not work
            User32.SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
        });
        sleep.Signal();


        // Check input per seconds
        var newInputInfo = User32.LASTINPUTINFO.Default;
        User32.GetLastInputInfo(ref newInputInfo);
        var count = newInputInfo.dwTime;

        var modernSleepTimer = new System.Timers.Timer();
        modernSleepTimer.Interval = 1000;// 1 * 60 * 1000;
        modernSleepTimer.Elapsed += (_, _) =>
        {
            User32.GetLastInputInfo(ref newInputInfo);
            if (count != newInputInfo.dwTime)
            {
                count = newInputInfo.dwTime;
                sleep.Signal();
            }
        };
        modernSleepTimer.Start();
    }
}
