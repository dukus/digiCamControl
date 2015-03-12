using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CameraControl.Core.Scripting
{
    public class CommandLineProcessor
    {
        public void Pharse(string[] args)
        {
            var cmd = args[0].ToLower().Trim();
            switch (cmd)
            {
                case "set" :
                    Set(args.Skip(1).ToArray());
                    break;
            }
        }

        private void Set(string[] args)
        {
            var device = ServiceProvider.DeviceManager.SelectedCameraDevice;
            var arg = args[0].ToLower().Trim();
             
            switch (arg)
            {
                case "shutterspeed":
                    device.ShutterSpeed.SetValue(args[1]);
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
                default:
                    throw new Exception("Unknow parameter");
            }
        }
    }
}
