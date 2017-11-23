using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices;
using GalaSoft.MvvmLight;

namespace Capture.Workflow.Core.Classes
{
    public class Context:ViewModelBase
    {
        private ICameraDevice _cameraDevice;
        public WorkFlow WorkFlow { get; set; }

        public ICameraDevice CameraDevice
        {
            get { return _cameraDevice; }
            set
            {
                _cameraDevice = value;
                RaisePropertyChanged(() => CameraDevice);
            }
        }

        public FileItem FileItem { get; set; }
        public ContextTargetEnum Target { get; set; }
        public Stream ImageStream { get; set; }
    }
}
