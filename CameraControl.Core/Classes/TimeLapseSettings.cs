using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraControl.Core.Classes
{
    public class TimeLapseSettings
    {
        public bool StartNow { get; set; }
        public bool StartAt { get; set; }
        public bool StartIn { get; set; }
        public bool StartDaily { get; set; }
        public DateTime StartDate { get; set; }
        public bool StartDay0 { get; set; }
        public bool StartDay1 { get; set; }
        public bool StartDay2 { get; set; }
        public bool StartDay3 { get; set; }
        public bool StartDay4 { get; set; }
        public bool StartDay5 { get; set; }
        public bool StartDay6 { get; set; }

        public bool StopAtPhotos { get; set; }
        public bool StopIn { get; set; }
        public bool StopAt { get; set; }

        public int StopCaptureCount { get; set; }
        public DateTime StopDate { get; set; }
        public int TimeBetweenShots { get; set; }

        public bool Capture { get; set; }
        public bool CaptureAll { get; set; }
        public bool CaptureScript { get; set; }
        public string ScriptFile { get; set; }
        public bool Bracketing { get; set; }

        public TimeLapseSettings()
        {
            StartNow = true;
            StartDate = DateTime.Now;
            StopAtPhotos = true;
            StopDate = DateTime.Now.AddHours(1);
            TimeBetweenShots = 15;
            Capture = true;
        }
    }
}
