using Microsoft.Win32;
using TouchChan.AssistiveTouch.Gesture.Common;

namespace TouchChan.AssistiveTouch.Gesture.Input
{
    // Seems not so important
    internal class InputProvider : IDisposable
    {
        private bool disposedValue = false; // To detect redundant calls
        private MessageWindow _messageWindow;
        private int _stateUpdating;

        public event RawPointsDataMessageEventHandler PointsIntercepted;

        public InputProvider()
        {
            _messageWindow = new MessageWindow();
            _messageWindow.PointsIntercepted += MessageWindow_PointsIntercepted;

            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(OnSessionSwitch);
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(OnPowerModeChanged);
        }

        private void MessageWindow_PointsIntercepted(object sender, RawPointsDataMessageEventArgs e)
        {
            if (e.RawData.Count == 0)
                return;
            PointsIntercepted?.Invoke(this, e);
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
            {
                UpdateDeviceState();
            }
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            // We need to handle sleeping(and other related events)
            // This is so we never lose the lock on the touchpad hardware.
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLogon:
                case SessionSwitchReason.SessionUnlock:
                    UpdateDeviceState();
                    break;
                default:
                    break;
            }
        }

        private void UpdateDeviceState()
        {
            if (0 == System.Threading.Interlocked.Exchange(ref _stateUpdating, 1))
            {
                Task.Delay(600).ContinueWith((t) =>
                {
                    System.Threading.Interlocked.Exchange(ref _stateUpdating, 0);
                    _messageWindow.UpdateRegistration();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                SystemEvents.SessionSwitch -= OnSessionSwitch;
                SystemEvents.PowerModeChanged -= OnPowerModeChanged;
                disposedValue = true;
            }
        }

        ~InputProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
