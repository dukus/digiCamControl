#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Classes
{
    /// <summary>
    /// Manager for external device handing like external shutter release
    /// </summary>
    public class ExternalDeviceManager : BaseFieldClass
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
                NotifyPropertyChanged("ExternalDeviceNames");
                NotifyPropertyChanged("ExternalShutters");
            }
        }

        public void RefreshDeviceLists()
        {
            NotifyPropertyChanged("ExternalDeviceNames");
            NotifyPropertyChanged("ExternalShutters");
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
                return
                    ServiceProvider.Settings.DeviceConfigs.Items.Where(
                        config => config.SourceEnum == SourceEnum.ExternaExternalShutterRelease).ToList();
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
            return
                (from device in ExternalDevices where device.Name == config.DriverName select device.OpenShutter(config))
                    .FirstOrDefault();
        }

        public bool CloseShutter(CustomConfig config)
        {
            return
                (from device in ExternalDevices
                 where device.Name == config.DriverName
                 select device.CloseShutter(config)).FirstOrDefault();
        }

        public bool AssertFocus(CustomConfig config)
        {
            return
                (from device in ExternalDevices where device.Name == config.DriverName select device.AssertFocus(config))
                    .FirstOrDefault();
        }

        public bool DeassertFocus(CustomConfig config)
        {
            return
                (from device in ExternalDevices
                 where device.Name == config.DriverName
                 select device.DeassertFocus(config)).FirstOrDefault();
        }

        public bool Capture(CustomConfig config)
        {
            return
                (from device in ExternalDevices where device.Name == config.DriverName select device.Capture(config)).
                    FirstOrDefault();
        }

        public bool Focus(CustomConfig config)
        {
            return
                (from device in ExternalDevices where device.Name == config.DriverName select device.Focus(config)).
                    FirstOrDefault();
        }
    }
}