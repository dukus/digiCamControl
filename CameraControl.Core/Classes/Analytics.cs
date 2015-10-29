using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using CameraControl.Devices;
using CameraControl.Devices.Others;
using MeasurementProtocolClient;

namespace CameraControl.Core.Classes
{
    public class Analytics
    {
        private const string TrackId = "UA-36038932-3";

        private string _userAgent;

        private string _applicationVersion;

        public void SetParams(TrackerParameters param)
        {
            param.ApplicationName = ServiceProvider.Branding.ApplicationTitle ?? "digiCamControl";
            param.ApplicationVersion = _applicationVersion;
            param.ScreenResolution = new Size((int) System.Windows.SystemParameters.PrimaryScreenWidth, (int) System.Windows.SystemParameters.PrimaryScreenHeight);
            param.UserLanguage = ServiceProvider.Settings.SelectedLanguage;
        }

        public void Start()
        {
            if (!ServiceProvider.Settings.SendUsageStatistics)
                return;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                _applicationVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
                _userAgent = string.Format("digiCamControl/{0} (Windows NT {1}.{2}) ",FileVersionInfo.GetVersionInfo(assembly.Location).ProductMajorPart + "." +
                                         FileVersionInfo.GetVersionInfo(assembly.Location).ProductMinorPart,
                    Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor);

                var tracker = new PageviewTracker(TrackId, ServiceProvider.Settings.ClientId);
                tracker.UseSsl = true;
                tracker.UserAgent = _userAgent;
                tracker.Parameters.DocumentHostName = "digicamcontrol.com";
                tracker.Parameters.DocumentPath = "/" + _applicationVersion;
                tracker.Parameters.DocumentTitle = "Start";
                tracker.Parameters.SessionControl = TrackerParameters.SessionControlValues.Start;
                SetParams(tracker.Parameters);
                tracker.Send();
                SendEvent("Application", "Start", _applicationVersion);
            }
            catch (Exception exception)
            {
                Log.Error("Analytics", exception);
            }
        }

        public void Stop()
        {
            if (!ServiceProvider.Settings.SendUsageStatistics)
                return;
            try
            {
                
                var tracker = new PageviewTracker(TrackId, ServiceProvider.Settings.ClientId);
                tracker.UseSsl = true;
                tracker.UserAgent = _userAgent;
                tracker.Parameters.DocumentHostName = "digicamcontrol.com";
                tracker.Parameters.DocumentPath = "/" ;
                tracker.Parameters.DocumentTitle = "Stop";
                tracker.Parameters.ApplicationName = "digiCamControl";
                tracker.Parameters.SessionControl = TrackerParameters.SessionControlValues.End;
                SetParams(tracker.Parameters);
                tracker.Send();
                SendEvent("Application", "Stop", null);
            }
            catch (Exception exception)
            {
                //Log.Error("Analytics", exception);
            }
        }

        private void SendEvent(string cat, string action, string label, int value = 0)
        {
            if (!ServiceProvider.Settings.SendUsageStatistics)
                return;
            Task.Factory.StartNew(() => SendEventThread(cat, action, label, value));
        }

        private void SendEventThread(string cat, string action, string label, int value=0)
        {
            try
            {
                var eventTrack = new EventTracker(TrackId, ServiceProvider.Settings.ClientId);
                eventTrack.UseSsl = true;
                eventTrack.UserAgent = _userAgent;
                eventTrack.Parameters.EventCategory = cat;
                eventTrack.Parameters.EventAction = action;
                eventTrack.Parameters.EventLabel = label;
                if (value > 0)
                    eventTrack.Parameters.EventValue = value.ToString();
                SetParams(eventTrack.Parameters);
                eventTrack.Send();
            }
            catch (Exception exception)
            {
              //  Log.Error("Analytics", exception);    
            }
            
        }

        public void CameraConnected(ICameraDevice device)
        {
            if (device is FakeCameraDevice)
                return;
            var cameracount = ServiceProvider.DeviceManager.ConnectedDevices.Count -
                              (ServiceProvider.Settings.AddFakeCamera ? 1 : 0);
            SendEvent("Camera", "Connected" , device.DeviceName, cameracount);
            if (cameracount > 1)
                SendEvent("MultipleCamera", "Connected_" + cameracount, device.DeviceName);
        }

        public void CameraCapture(ICameraDevice device)
        {
            SendEvent("Camera", "Capture", device.DeviceName);
        }

        public void Command(string command)
        {
            SendEvent("Command", command, null);
        }

        public void Command(string command, string label)
        {
            SendEvent("Command", command, label);
        }

        public void Error(Exception exception)
        {
            SendEvent("Error", exception.GetType().ToString(), exception.Message);
        }

        public void PluginExecute(string command)
        {
            SendEvent("Plugin", command, null);
        }

        public void TransformPluginExecute(string command)
        {
            SendEvent("TransformPlugin", command, null);
        }
    }
}
