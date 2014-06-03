using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
  public class NikonD7000:NikonBase
  {
    public const int CONST_PROP_AfModeAtLiveView = 0xD061;

    public override bool Init(DeviceDescriptor deviceDescriptor)
    {
      bool res = base.Init(deviceDescriptor);
      Capabilities.Clear();
      Capabilities.Add(CapabilityEnum.LiveView);
      Capabilities.Add(CapabilityEnum.RecordMovie);
      Capabilities.Add(CapabilityEnum.CaptureInRam);
      Capabilities.Add(CapabilityEnum.CaptureNoAf);
      return res;
    }

    public override void StartLiveView()
    {
      base.StartLiveView();
      SetProperty(CONST_CMD_SetDevicePropValue, new[] {(byte) 0}, CONST_PROP_AfModeAtLiveView, -1);
    }

    public override void Focus(int step)
    {
      if (step == 0)
        return;
      lock (Locker)
      {
        //DeviceReady();
        ErrorCodes.GetException(step > 0
                                  ? ExecuteWithNoData(CONST_CMD_MfDrive, 0x00000001, (uint)step)
                                  : ExecuteWithNoData(CONST_CMD_MfDrive, 0x00000002, (uint)-step));
        DeviceReady();
      }
    }
  }
}
