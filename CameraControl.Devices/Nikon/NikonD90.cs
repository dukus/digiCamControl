using System;
using System.IO;
using CameraControl.Devices.Classes;
using PortableDeviceLib;

namespace CameraControl.Devices.Nikon
{
  public class NikonD90:NikonBase
  {

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

    override public LiveViewData GetLiveViewImage()
    {
      LiveViewData viewData = new LiveViewData();
      viewData.HaveFocusData = true;

      const int headerSize = 128;

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

    /// <summary>
    /// Take picture with no autofocus
    /// If live view runnig the live view is stoped after done restarted
    /// </summary>
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
          // the focus mode can be sett only in host mode
          LockCamera();
          byte oldval = 0;
          byte[] val = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_AFModeSelect, -1);
          if (val != null && val.Length > 0)
            oldval = val[0];

          SetProperty(CONST_CMD_SetDevicePropValue, new[] { (byte)4 }, CONST_PROP_AFModeSelect, -1);

          ErrorCodes.GetException(CaptureInSdRam
                                    ? ExecuteWithNoData(CONST_CMD_InitiateCaptureRecInSdram, 0xFFFFFFFF)
                                    : ExecuteWithNoData(CONST_CMD_InitiateCapture));

          if (val != null && val.Length > 0)
            SetProperty(CONST_CMD_SetDevicePropValue, new[] { oldval }, CONST_PROP_AFModeSelect, -1);

          UnLockCamera();
        }
        catch (Exception)
        {
          IsBusy = false;
          throw;
        }

        //if (live != null && live.Length > 0 && live[0] == 1)
        //  StartLiveView();
      }
    }


  }
}
