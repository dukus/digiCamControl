using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.ExternalDevices
{
    public class ArduinoShutterRelease : IExternalDevice
    {
        public string Name { get; set; }

        public bool Capture(CustomConfig config)
        {
            //throw new NotImplementedException();
            return true;
        }

        public bool Focus(CustomConfig config)
        {
            return true;
        }

        public bool CanExecute(CustomConfig config)
        {
            return true;
        }

        public UserControl GetConfig(CustomConfig config)
        {
            try
            {
                return new ArduinoShutterReleaseConfig(config);
            }
            catch (Exception exception)
            {
                Log.Error("", exception);
            }
            return null;
        }

        public SourceEnum DeviceType { get; set; }
        public bool OpenShutter(CustomConfig config)
        {
            try
            {
                if (config.AttachedObject != null)
                    CloseShutter(config);
                SerialPort serialPort = new SerialPort(config.Get("Port"));
                serialPort.Open();
                serialPort.BaudRate = 9600;
                serialPort.WriteTimeout = 3500;
                if (config.Get("IsHexa") == "True")
                {
                    var data = StringToByteArray(config.Get("CaptureOn"));
                    serialPort.Write(data, 0, data.Length);
                }
                else
                {
                    serialPort.WriteLine(config.Get("CaptureOn"));
                }
                config.AttachedObject = serialPort;
            }
            catch (Exception ex)
            {
                Log.Debug("ArduinoShutterRelease", ex);
                StaticHelper.Instance.SystemMessage = "Error Shutter " + ex.Message;
            }
            return true;
        }

        public bool CloseShutter(CustomConfig config)
        {
            try
            {
                if (config.AttachedObject == null)
                    return false;
                SerialPort serialPort = config.AttachedObject as SerialPort;
                if (serialPort == null) throw new ArgumentNullException("serialPort");
                if (config.Get("IsHexa") == "True")
                {
                    var data = StringToByteArray(config.Get("CaptureOff"));
                    serialPort.Write(data, 0, data.Length);
                }
                else
                {
                    serialPort.WriteLine(config.Get("CaptureOff"));
                }
                serialPort.Close();
                config.AttachedObject = null;
            }
            catch (Exception ex)
            {
                Log.Debug("ArduinoShutterRelease", ex);
                StaticHelper.Instance.SystemMessage = "Error Shutter " + ex.Message;
            }
            return true;
        }

        public bool AssertFocus(CustomConfig config)
        {
            return true;
        }

        public bool DeassertFocus(CustomConfig config)
        {
            return true;
        }

        public ArduinoShutterRelease()
        {
            Name = "Arduino Shutter Release (Serial)";
            DeviceType = SourceEnum.ExternaExternalShutterRelease;
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
