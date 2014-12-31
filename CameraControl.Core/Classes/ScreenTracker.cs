using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeasurementProtocolClient;

namespace CameraControl.Core.Classes
{
    public class ScreenTracker : Tracker
    {
        public override TrackerParameters.HitTypes HitType
        {
            get { return TrackerParameters.HitTypes.appview; }
        }

        public ScreenTracker(string trackingId, string clientId)
            : base(trackingId, clientId)
        {
        }
    }
}
