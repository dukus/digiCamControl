using System;
using System.Threading;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Nikon
{
    public class NikonD600 : NikonD800
    {
        public override LiveViewData GetLiveViewImage()
        {
            LiveViewData viewData = new LiveViewData();
            if (Monitor.TryEnter(Locker, 10))
            {
                try
                {
                    //DeviceReady();
                    viewData.HaveFocusData = true;

                    const int headerSize = 384;

                    byte[] result = StillImageDevice.ExecuteReadData(CONST_CMD_GetLiveViewImage);
                    if (result == null || result.Length <= headerSize)
                        return null;
                    GetAditionalLIveViewData(viewData, result);
                    viewData.ImageDataPosition = headerSize;
                    viewData.ImageData = result;
                }
                finally
                {
                    Monitor.Exit(Locker);
                }
            }
            return viewData;
        }

        protected override void GetAditionalLIveViewData(LiveViewData viewData, byte[] result)
        {
            viewData.LiveViewImageWidth = ToInt16(result, 8);
            viewData.LiveViewImageHeight = ToInt16(result, 10);

            viewData.ImageWidth = ToInt16(result, 12);
            viewData.ImageHeight = ToInt16(result, 14);

            viewData.FocusFrameXSize = ToInt16(result, 24);
            viewData.FocusFrameYSize = ToInt16(result, 26);

            viewData.FocusX = ToInt16(result, 28);
            viewData.FocusY = ToInt16(result, 30);

            viewData.Focused = result[48] != 1;
            viewData.MovieIsRecording = result[70] == 1;
            
            if (result[37] == 1)
                viewData.Rotation = -90;
            if (result[37] == 2)
                viewData.Rotation = 90;
        }

        protected override PropertyValue<long> InitStillCaptureMode()
        {
            PropertyValue<long> res = new PropertyValue<long>()
                                          {
                                              Name = "Still Capture Mode",
                                              IsEnabled = true,
                                              Code = 0x5013,
                                              SubType = typeof (UInt16)
                                          };
            res.AddValues("Single shot (single-frame shooting)", 0x0001);
            res.AddValues("Continuous high-speed shooting (CH)", 0x0002);
            res.AddValues("Continuous low-speed shooting (CL)", 0x8010);
            res.AddValues("Self-timer", 0x8011);
            res.AddValues("Mirror-up", 0x8012);
            res.AddValues("Quiet shooting", 0x8016);
            res.AddValues("Remote control", 0x8017);
            res.ValueChanged +=
                (sender, key, val) => SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes(val),
                                                  res.Code, -1);
            return res;
        }



    }
}
