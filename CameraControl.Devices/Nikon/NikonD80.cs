using System;
using System.Collections.Generic;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
  public class NikonD80 : NikonBase
  {
    protected new Dictionary<int, string> _csTable = new Dictionary<int, string>()
                                                {
                                                  {0, "JPEG (BASIC)"},
                                                  {1, "JPEG (NORMAL)"},
                                                  {2, "JPEG (FINE)"},
                                                  {3, "RAW"},
                                                  {4, "RAW + JPEG (BASIC)"},
                                                  {5, "RAW + JPEG (NORMAL)"},
                                                  {6, "RAW + JPEG (FINE)"}
                                                };

    public override bool Init(DeviceDescriptor deviceDescriptor)
    {
      bool res = base.Init(deviceDescriptor);
      Capabilities.Clear();
      Capabilities.Add(CapabilityEnum.CaptureNoAf);
      HaveLiveView = false;
      CaptureInSdRam = false;
      return res;
    }

    protected override void InitCompressionSetting()
    {
      try
      {
        byte datasize = 1;
        CompressionSetting = new PropertyValue<int>();
        CompressionSetting.ValueChanged += CompressionSetting_ValueChanged;
        byte[] result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc, CONST_PROP_CompressionSetting);
        int type = BitConverter.ToInt16(result, 2);
        byte formFlag = result[(2 * datasize) + 5];
        byte defval = result[datasize + 5];
        for (int i = 0; i < result.Length - ((2 * datasize) + 6 + 2); i += datasize)
        {
          byte val = result[((2 * datasize) + 6 + 2) + i];
          CompressionSetting.AddValues(_csTable.ContainsKey(val) ? _csTable[val] : val.ToString(), val);
        }
        CompressionSetting.SetValue(defval);
      }
      catch (Exception )
      {
        
      }
    }

    public override void ReadDeviceProperties(int prop)
    {
      base.ReadDeviceProperties(prop);
      HaveLiveView = false;
    }

    /// <summary>
    /// Take picture with no autofocus
    /// If live view runnig the live view is stoped after done restarted
    /// </summary>
    public override void CapturePhotoNoAf()
    {
      lock (Locker)
      {
        byte oldval = 0;
        byte[] val = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue, CONST_PROP_AFModeSelect, -1);
        if (val != null && val.Length > 0)
          oldval = val[0];

        ErrorCodes.GetException(StillImageDevice.ExecuteWriteData(CONST_CMD_SetDevicePropValue, new[] { (byte)4 },
                                                                   CONST_PROP_AFModeSelect, -1));
        ErrorCodes.GetException(StillImageDevice.ExecuteWithNoData(CONST_CMD_InitiateCapture));
        if (val != null && val.Length > 0)
          ErrorCodes.GetException(StillImageDevice.ExecuteWriteData(CONST_CMD_SetDevicePropValue, new[] { oldval },
                                                                     CONST_PROP_AFModeSelect, -1));
      }
    }

  }
}
