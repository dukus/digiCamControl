using System;
using System.Windows.Threading;

namespace PhotoBooth
{
    public class ScreenSaverInhibitor : IDisposable
    {
        private DispatcherTimer timer;

        public ScreenSaverInhibitor(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            this.timer = new DispatcherTimer(DispatcherPriority.Normal, dispatcher);
            this.timer = new DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 1, 0);
            this.timer.Tick += screenSaverTimer_Tick;
            this.timer.IsEnabled = true;
            this.timer.Start();                
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.timer != null)
            {
                this.timer.Stop();
                this.timer.IsEnabled = false;
                this.timer = null;
            }
        }

        private const byte VK_LSHIFT = 0xA0;
        private const int KEYEVENTF_KEYUP = 0x0002;

        // When the timer elapses, send Left Shift Up
        private void screenSaverTimer_Tick(object sender, EventArgs e)
        {
            NativeMethods.keybd_event(Constants.VK_LSHIFT, 0x45, Constants.KEYEVENTF_KEYUP, IntPtr.Zero);
        }
    }
}
