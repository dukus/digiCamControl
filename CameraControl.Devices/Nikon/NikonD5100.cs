using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
  public class NikonD5100 : NikonBase
  {
    public override bool Init(DeviceDescriptor deviceDescriptor)
    {
      bool res = base.Init(deviceDescriptor);
      Capabilities.Clear();
      Capabilities.Add(CapabilityEnum.LiveView);
      Capabilities.Add(CapabilityEnum.RecordMovie);
      Capabilities.Add(CapabilityEnum.CaptureInRam);
      Capabilities.Add(CapabilityEnum.CaptureNoAf);
      //Capabilities.Add(CapabilityEnum.Bulb);
      return res;
    }

    //public override void StartLiveView()
    //{
    //  //SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)1 }, CONST_PROP_RecordingMedia, -1);
    //  //DeviceReady();
    //  base.StartLiveView();
    //}

    //public override void StopLiveView()
    //{
    //  base.StopLiveView();
    //  DeviceReady();
    //  //SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)0 }, CONST_PROP_RecordingMedia, -1);
    //  DeviceReady();
    //}

  }
}
