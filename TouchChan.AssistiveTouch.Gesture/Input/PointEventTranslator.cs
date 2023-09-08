﻿using TouchChan.AssistiveTouch.Gesture.Common;

namespace TouchChan.AssistiveTouch.Gesture.Input
{
    public class PointEventTranslator
    {
        private int _lastPointsCount;

        internal Devices SourceDevice { get; private set; }

        internal PointEventTranslator(InputProvider inputProvider)
        {
            inputProvider.PointsIntercepted += TranslateTouchEvent;
        }

        #region Custom Events

        public event EventHandler<InputPointsEventArgs> PointDown;

        protected virtual void OnPointDown(InputPointsEventArgs args)
        {
            if (SourceDevice != Devices.None && SourceDevice != args.PointSource && args.PointSource != Devices.Pen) return;
            SourceDevice = args.PointSource;
            PointDown?.Invoke(this, args);
        }

        public event EventHandler<InputPointsEventArgs> PointUp;

        protected virtual void OnPointUp(InputPointsEventArgs args)
        {
            if (SourceDevice != Devices.None && SourceDevice != args.PointSource) return;

            PointUp?.Invoke(this, args);

            SourceDevice = Devices.None;
        }

        public event EventHandler<InputPointsEventArgs> PointMove;

        protected virtual void OnPointMove(InputPointsEventArgs args)
        {
            if (SourceDevice != args.PointSource) return;
            PointMove?.Invoke(this, args);
        }

        #endregion

        #region Private Methods

        private void TranslateTouchEvent(object sender, RawPointsDataMessageEventArgs e)
        {
            // Console.WriteLine(e.SourceDevice); // Output TouchScreen

            if ((e.SourceDevice & Devices.TouchDevice) != 0)
            {
                //Console.WriteLine("1");
                int releaseCount = e.RawData.Count(rtd => rtd.State == 0);

                if (e.RawData.Count == _lastPointsCount) // Normal process
                {
                    //Console.WriteLine($"2 {releaseCount}");

                    if (releaseCount != 0)
                    {
                        //Console.WriteLine("3");
                        OnPointUp(new InputPointsEventArgs(e.RawData, e.SourceDevice));
                        _lastPointsCount -= releaseCount;
                        return;
                    }
                    OnPointMove(new InputPointsEventArgs(e.RawData, e.SourceDevice));
                }
                else if (e.RawData.Count > _lastPointsCount) // exception: always 1 4 1 4 
                {
                    //Console.WriteLine($"4");
                    if (releaseCount != 0)
                        return;
                    if (PointCapture.Instance.InputPoints.Any(p => p.Count > 10))
                    {
                        OnPointMove(new InputPointsEventArgs(e.RawData, e.SourceDevice));
                        return;
                    }
                    _lastPointsCount = e.RawData.Count;
                    OnPointDown(new InputPointsEventArgs(e.RawData, e.SourceDevice));
                }
                else
                {
                    //Console.WriteLine($"5");
                    OnPointUp(new InputPointsEventArgs(e.RawData, e.SourceDevice));
                    _lastPointsCount = _lastPointsCount - e.RawData.Count > releaseCount ? e.RawData.Count : _lastPointsCount - releaseCount;
                }
            }
        }

        #endregion
    }
}
