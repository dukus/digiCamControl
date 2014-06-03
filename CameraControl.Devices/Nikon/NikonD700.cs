using System.IO;
using CameraControl.Devices.Classes;
using PortableDeviceLib;

namespace CameraControl.Devices.Nikon
{
  public class NikonD700:NikonBase
  {
    //public const int CONST_PROP_RecordingMedia = 0xD10B;
    public const int CONST_PROP_LiveViewMode = 0xD1A0;

    public override bool Init(DeviceDescriptor deviceDescriptor)
    {
      bool res = base.Init(deviceDescriptor);
      Capabilities.Clear();
      Capabilities.Add(CapabilityEnum.LiveView);
      Capabilities.Add(CapabilityEnum.RecordMovie);
      Capabilities.Add(CapabilityEnum.CaptureInRam);
      Capabilities.Add(CapabilityEnum.CaptureNoAf);
      CaptureInSdRam = true;
      return res;
    }


    public override void StartLiveView()
    {
      // set record media to SDRAM
      SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)1 }, CONST_PROP_RecordingMedia, -1);
      DeviceReady();
      // set to Tripod shooting mode
      SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)1 }, CONST_PROP_LiveViewMode, -1);
      DeviceReady();
      base.StartLiveView();
    }

    public override void StopLiveView()
    {
      base.StopLiveView();
      DeviceReady();
      SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)0 }, CONST_PROP_RecordingMedia, -1);
      DeviceReady();
    }

    public override void CapturePhotoNoAf()
    {
      lock (Locker)
      {
        try
        {
          IsBusy = true;
          MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetDevicePropValue, CONST_PROP_LiveViewStatus, -1);
          ErrorCodes.GetException(response.ErrorCode);
          // test if live view is on 
          if (response.Data != null && response.Data.Length > 0 && response.Data[0] > 0)
          {
            if (CaptureInSdRam)
            {
              DeviceReady();
              ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInSdram, 0xFFFFFFFF));
              return;
            }
            StopLiveView();
          }
          DeviceReady();
          ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_InitiateCapture));
        }
        catch
        {
          IsBusy = false;
          throw;
        }
      }
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
