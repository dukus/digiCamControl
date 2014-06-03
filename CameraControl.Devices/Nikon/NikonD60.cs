using System;
using System.Collections.Generic;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
  public class NikonD60:NikonBase
  {

    protected new Dictionary<int, string> _csTable = new Dictionary<int, string>()
                                                {
                                                  {0, "JPEG (BASIC)"},
                                                  {1, "JPEG (NORMAL)"},
                                                  {2, "JPEG (FINE)"},
                                                  {3, "RAW"},
                                                  {4, "RAW + JPEG (BASIC)"},
                                                };

    

    protected override void InitCompressionSetting()
    {
      try
      {
        byte datasize = 1;
        CompressionSetting = new PropertyValue<int>();
        CompressionSetting.ValueChanged += CompressionSetting_ValueChanged;
        byte[] result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc, CONST_PROP_CompressionSetting);
        byte defval = result[datasize + 5];
        for (int i = 0; i < result.Length - ((2 * datasize) + 6 + 2); i += datasize)
        {
          byte val = result[((2 * datasize) + 6 + 2) + i];
          CompressionSetting.AddValues(_csTable.ContainsKey(val) ? _csTable[val] : val.ToString(), val);
        }
        CompressionSetting.SetValue(defval);
      }
      catch (Exception)
      {

      }
    }

    public override bool Init(DeviceDescriptor deviceDescriptor)
    {
      bool res = base.Init(deviceDescriptor);
      Capabilities.Clear();
      Capabilities.Add(CapabilityEnum.CaptureNoAf);
      Capabilities.Add(CapabilityEnum.CaptureInRam);
      HaveLiveView = false;
      CaptureInSdRam = false;
      PropertyChanged -= NikonBase_PropertyChanged;
      return res;
    }

    public override void ReadDeviceProperties(int prop)
    {
      base.ReadDeviceProperties(prop);
      HaveLiveView = false;
    }


  }
}
