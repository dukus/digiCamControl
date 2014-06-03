using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Others
{
    public class FakeCameraDevice : BaseCameraDevice
    {
        #region Implementation of ICameraDevice


        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            return true;
        }

        public override void CapturePhoto()
        {
            
        }

        public override void CapturePhotoNoAf()
        {
            
        }

        #endregion

        public FakeCameraDevice()
        {
            HaveLiveView = false;
            IsBusy = false;
            DeviceName = "Fake camera";
            SerialNumber = "00000000";
            IsConnected = true;
            HaveLiveView = false;
            ExposureStatus = 1;
            ExposureCompensation = new PropertyValue<int>() { IsEnabled = false };
            Mode = new PropertyValue<uint> { IsEnabled = false };
            FNumber = new PropertyValue<int> { IsEnabled = false };
            ShutterSpeed = new PropertyValue<long> { IsEnabled = false };
            WhiteBalance = new PropertyValue<long> { IsEnabled = false };
            FocusMode = new PropertyValue<long> { IsEnabled = false };
            CompressionSetting = new PropertyValue<int> { IsEnabled = false };
            IsoNumber = new PropertyValue<int> { IsEnabled = false };
            ExposureMeteringMode = new PropertyValue<int> { IsEnabled = false };
            Battery = 100;
            Capabilities.Add(CapabilityEnum.CaptureNoAf);
        }
    }
}
