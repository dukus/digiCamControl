using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public bool Focus(CustomConfig config)
        {
            throw new NotImplementedException();
        }

        public bool CanExecute(CustomConfig config)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool CloseShutter(CustomConfig config)
        {
            throw new NotImplementedException();
        }

        public bool AssertFocus(CustomConfig config)
        {
            throw new NotImplementedException();
        }

        public bool DeassertFocus(CustomConfig config)
        {
            throw new NotImplementedException();
        }

        public ArduinoShutterRelease()
        {
            Name = "Arduino Shutter Release (Serial)";
            DeviceType = SourceEnum.ExternaExternalShutterRelease;
        }
    }
}
