using System.IO;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
  public class NikonD3X : NikonBase
  {
    public override bool Init(DeviceDescriptor deviceDescriptor)
    {
      bool ret = base.Init(deviceDescriptor);
      Capabilities.Add(CapabilityEnum.LiveView);
      return ret;
    }

    override public LiveViewData GetLiveViewImage()
    {
      LiveViewData viewData = new LiveViewData();
      viewData.HaveFocusData = true;

      const int headerSize = 64;

      byte[] result = StillImageDevice.ExecuteReadData(CONST_CMD_GetLiveViewImage);
      if (result == null || result.Length <= headerSize)
        return null;
      int cbBytesRead = result.Length;
      GetAditionalLIveViewData(viewData, result);

      MemoryStream copy = new MemoryStream((int)cbBytesRead - headerSize);
      copy.Write(result, headerSize, (int)cbBytesRead - headerSize);
      copy.Close();
      viewData.ImageData = copy.GetBuffer();

      return viewData;
    } 
  }
}
