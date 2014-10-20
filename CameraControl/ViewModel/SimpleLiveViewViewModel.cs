using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices;
using GalaSoft.MvvmLight;

namespace CameraControl.ViewModel
{
    public class SimpleLiveViewViewModel : ViewModelBase
    {
        private ICameraDevice _cameraDevice;

        public ICameraDevice CameraDevice
        {
            get { return _cameraDevice; }
            set
            {
                _cameraDevice = value;
                RaisePropertyChanged(() => CameraDevice);
            }
        }

        public SimpleLiveViewViewModel()
        {
            
        }

        public SimpleLiveViewViewModel(ICameraDevice device)
        {
            CameraDevice = device;
        }
    }
}
