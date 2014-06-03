using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    /// <summary>
    /// Manager for external device handing like external shutter release
    /// </summary>
    public class ExternalDeviceManager:BaseFieldClass
    {
        //private AsyncObservableCollection<IExternalDevice> _externalShutterReleaseSources;
        //public AsyncObservableCollection<IExternalDevice> ExternalShutterReleaseSources
        //{
        //    get { return _externalShutterReleaseSources; }
        //    set
        //    {
        //        _externalShutterReleaseSources = value;
        //        NotifyPropertyChanged("ExternalShutterReleaseSources");
        //    }
        //}

        private AsyncObservableCollection<IExternalDevice> _externalDevices;
        public AsyncObservableCollection<IExternalDevice> ExternalDevices
        {
            get { return _externalDevices; }
            set
            {
                _externalDevices = value;
                NotifyPropertyChanged("ExternalDevices");
            }
        }

        public AsyncObservableCollection<string> ExternalDeviceNames
        {
            get
            {
                var res = new AsyncObservableCollection<string>();
                foreach (IExternalDevice externalShutterReleaseSource in ExternalDevices)
                {
                    res.Add(externalShutterReleaseSource.Name);
                }
                return res;
            }
        }

        public List<CustomConfig> ExternalShutters
        {
            get
            {
                return ServiceProvider.Settings.DeviceConfigs.Items.Where(config => config.SourceEnum == SourceEnum.ExternaExternalShutterRelease).ToList();
            }
            
        }


        public IExternalDevice Get(string name)
        {
            return ExternalDevices.FirstOrDefault(external => external.Name == name);
        }

        public ExternalDeviceManager()
        {
            ExternalDevices = new AsyncObservableCollection<IExternalDevice>();
        }

        public bool OpenShutter(CustomConfig config)
        {
            return (from device in ExternalDevices where device.Name == config.DriverName select device.OpenShutter(config)).FirstOrDefault();
        }

        public bool CloseShutter(CustomConfig config)
        {
            return (from device in ExternalDevices where device.Name == config.DriverName select device.CloseShutter(config)).FirstOrDefault();
        }

        public bool AssertFocus(CustomConfig config)
        {
            return (from device in ExternalDevices where device.Name == config.DriverName select device.AssertFocus(config)).FirstOrDefault();
        }

        public bool DeassertFocus(CustomConfig config)
        {
            return (from device in ExternalDevices where device.Name == config.DriverName select device.DeassertFocus(config)).FirstOrDefault();
        }

        public bool Capture(CustomConfig config)
        {
            return (from device in ExternalDevices where device.Name == config.DriverName select device.Capture(config)).FirstOrDefault();
        }

        public bool Focus(CustomConfig config)
        {
            return (from device in ExternalDevices where device.Name == config.DriverName select device.Focus(config)).FirstOrDefault();
        }

    }
}
