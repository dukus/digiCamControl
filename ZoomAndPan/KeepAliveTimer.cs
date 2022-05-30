using System;
using System.Windows.Threading;

namespace ZoomAndPan
{
    public class KeepAliveTimer
    {
        private readonly DispatcherTimer _timer;
        private DateTime _startTime;
        private TimeSpan? _runTime;

        public TimeSpan Time { get; set; }
        public Action Action { get; set; }
        public bool Running { get; private set; }

        public KeepAliveTimer(TimeSpan time, Action action)
        {
            Time = time;
            Action = action;
            _timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle) { Interval = time };
            _timer.Tick += TimerExpired;
        }

        private void TimerExpired(object sender, EventArgs e)
        {
            lock (_timer)
            {
                Running = false;
                _timer.Stop();
                _runTime = DateTime.UtcNow.Subtract(_startTime);
                Action();
            }
        }

        public void Nudge()
        {
            lock (_timer)
            {
                if (!Running)
                {
                    _startTime = DateTime.UtcNow;
                    _runTime = null;
                    _timer.Start();
                    Running = true;
                }
                else
                {
                    //Reset the timer
                    _timer.Stop();
                    _timer.Start();
                }
            }
        }

        public TimeSpan GetTimeSpan()
        {
            return _runTime ?? DateTime.UtcNow.Subtract(_startTime);
        }
    }
}
