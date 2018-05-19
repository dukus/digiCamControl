using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.ViewModel
{
    public class CameraPropertyViewModel: ViewModelBase
    {
        private CameraProperty _cameraProperty;
        private List<string> _photoSessionNames;
        private List<string> _cameraPresets;

        public CameraProperty CameraProperty
        {
            get { return _cameraProperty; }
            set
            {
                _cameraProperty = value;
                RaisePropertyChanged(() => CameraProperty);
            }
        }

        public List<string> PhotoSessionNames
        {
            get { return _photoSessionNames; }
            set
            {
                _photoSessionNames = value;
                RaisePropertyChanged(()=>PhotoSessionNames);
            }
        }

        public List<string> CameraPresets
        {
            get { return _cameraPresets; }
            set
            {
                _cameraPresets = value;
                RaisePropertyChanged(()=>CameraPresets);
            }
        }

        public CameraPropertyViewModel()
        {
            if (!IsInDesignMode)
            {
                Init();
            }
        }

        public void Init()
        {
            PhotoSessionNames = new List<string> {"(None)"};
            foreach (PhotoSession photoSession in ServiceProvider.Settings.PhotoSessions)
            {
                PhotoSessionNames.Add(photoSession.Name);
            }
            CameraPresets = new List<string> {"(None)"};
            foreach (var cameraPresets in ServiceProvider.Settings.CameraPresets)
            {
                CameraPresets.Add(cameraPresets.Name);
            }
        }
    }
}
