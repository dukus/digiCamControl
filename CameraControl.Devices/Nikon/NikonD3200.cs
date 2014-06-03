using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
  public class NikonD3200: NikonBase
  {
    public override bool Init(DeviceDescriptor deviceDescriptor)
    {
      bool res = base.Init(deviceDescriptor);
      Capabilities.Clear();
      Capabilities.Add(CapabilityEnum.LiveView);
      Capabilities.Add(CapabilityEnum.RecordMovie);
      CaptureInSdRam = false;
      PropertyChanged -= NikonBase_PropertyChanged;
      return res;
    }


    protected override void GetAditionalLIveViewData(LiveViewData viewData, byte[] result)
    {
      viewData.LiveViewImageWidth = ToInt16(result, 8);
      viewData.LiveViewImageHeight = ToInt16(result, 10);

      viewData.ImageWidth = ToInt16(result, 12);
      viewData.ImageHeight = ToInt16(result, 14);

      viewData.FocusFrameXSize = 800;
      viewData.FocusFrameYSize = 800;

      viewData.FocusX = ToInt16(result, 20);
      viewData.FocusY = ToInt16(result, 22);

      viewData.Focused = result[40] != 1;
    }
  }
}
