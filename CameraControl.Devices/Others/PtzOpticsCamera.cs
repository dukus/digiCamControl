using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;
using PortableDeviceLib;

namespace CameraControl.Devices.Others
{
    public class PtzOpticsCamera : BaseCameraDevice
    {
        private string _address = "";

        public PtzOpticsCamera(string ip)
        {

            _address = ip;

            IsBusy = false;
            IsConnected = true;
            HaveLiveView = false;
            ExposureStatus = 1;
            ExposureCompensation = new PropertyValue<long>() { IsEnabled = false };
            Mode = new PropertyValue<long> { IsEnabled = false };
            FNumber = new PropertyValue<long> { IsEnabled = false };
            ShutterSpeed = new PropertyValue<long> { IsEnabled = false };
            WhiteBalance = new PropertyValue<long> { IsEnabled = false };
            FocusMode = new PropertyValue<long> { IsEnabled = false };

            IsoNumber = new PropertyValue<long> { IsEnabled = true };
            ExposureMeteringMode = new PropertyValue<long> { IsEnabled = false };
            Battery = 100;
            Capabilities.Add(CapabilityEnum.CaptureNoAf);
            Capabilities.Add(CapabilityEnum.LiveView);
            Capabilities.Add(CapabilityEnum.LiveViewStream);
            LiveViewImageZoomRatio = new PropertyValue<long>();
            LiveViewImageZoomRatio.AddValues("All", 0);
            LiveViewImageZoomRatio.Value = "All";


        }


        public override string GetLiveViewStream()
        {
            return "rtsp://"+_address+"/1";
        }

        public override void StartLiveView()
        {

        }

        public override void StopLiveView()
        {

        }

        public override void CapturePhoto()
        {
            var url = "http://"+_address+"/snapshot.jpg";
            PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
            {
                WiaImageItem = null,
                EventArgs = new PortableDeviceEventArgs(),
                CameraDevice = this,
                FileName = url.Replace('/', '\\'),
                Handle = url
            };
            OnPhotoCapture(this, args);
        }

        public override void TransferFile(object o, string filename)
        {
            TransferProgress = 0;
            HttpHelper.DownLoadFileByWebRequest(((string)o) , filename, this);
        }
    }


}
