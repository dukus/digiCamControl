using System;
using System.Linq;
using CameraControl.Core.Classes;
using CameraControl.Devices;

namespace CameraControl.Core.Scripting
{
    public class CommandLineProcessor
    {
        public object Pharse(string[] args)
        {
            var cmd = args[0].ToLower().Trim();
            switch (cmd)
            {
                case "capture":
                    CameraHelper.Capture(ServiceProvider.DeviceManager.SelectedCameraDevice);
                    return null;
                //case "startbulb":
                //    CameraHelper.Capture(ServiceProvider.DeviceManager.SelectedCameraDevice);
                //    return null;
                //case "stopbulb":
                case "set":
                    Set(args.Skip(1).ToArray());
                    return null;
                case "get":
                    return Get(args.Skip(1).ToArray());
                case "list":
                    return List(args.Skip(1).ToArray());
                default:
                    throw new Exception("Unknow parameter " + cmd);
            }
        }


        private object List(string[] args)
        {
            var device = GetDevice();
            var arg = args[0].ToLower().Trim();

            switch (arg)
            {
                case "shutterspeed":
                    return device.ShutterSpeed.Values;
                case "iso":
                    return device.IsoNumber.Values;
                case "exposurecompensation":
                    return device.ExposureCompensation.Values;
                case "aperture":
                    return device.FNumber.Values;
                case "focusmode":
                    return device.FocusMode.Values;
                case "whitebalance":
                    return device.WhiteBalance.Values;
                case "mode":
                    return device.Mode.Values;
                case "compressionsetting":
                    return device.CompressionSetting.Values;
                default:
                    throw new Exception("Unknow parameter");
            }
        }

        private object Get(string[] args)
        {
            var device = GetDevice();
            var arg = args[0].ToLower().Trim();

            switch (arg)
            {
                case "shutterspeed":
                    return device.ShutterSpeed.Value;
                case "iso":
                    return device.IsoNumber.Value;
                case "exposurecompensation":
                    return device.ExposureCompensation.Value;
                case "aperture":
                    return device.FNumber.Value;
                case "focusmode":
                    return device.FocusMode.Value;
                case "whitebalance":
                    return device.WhiteBalance.Value;
                case "mode":
                    return device.Mode.Value;
                case "compressionsetting":
                    return device.CompressionSetting.Value;
                default:
                    throw new Exception("Unknow parameter");
            }
        }


        private void Set(string[] args)
        {
            var device = GetDevice();
            var arg = args[0].ToLower().Trim();

            switch (arg)
            {
                case "shutterspeed":
                    var val = args[1];
                    if (!val.Contains("/") && !val.EndsWith("s") && !val.Equals("bulb"))
                    {
                        val += "s";
                    }
                    if (val.Equals("bulb"))
                    {
                        val = "Bulb";
                    }
                    device.ShutterSpeed.SetValue(val);
                    break;
                case "iso":
                    device.IsoNumber.SetValue(args[1]);
                    break;
                case "exposurecompensation":
                    device.ExposureCompensation.SetValue(args[1]);
                    break;
                case "aperture":
                    device.FNumber.SetValue(args[1]);
                    break;
                case "focusmode":
                    device.FocusMode.SetValue(args[1]);
                    break;
                case "whitebalance":
                    device.WhiteBalance.SetValue(args[1]);
                    break;
                case "mode":
                    device.Mode.SetValue(args[1]);
                    break;
                case "compressionsetting":
                    device.CompressionSetting.SetValue(args[1]);
                    break;
                default:
                    throw new Exception("Unknow parameter");
            }
        }

        private ICameraDevice GetDevice()
        {
            if (ServiceProvider.DeviceManager == null)
                return null;
            return ServiceProvider.DeviceManager.SelectedCameraDevice;
        }
    }
}
