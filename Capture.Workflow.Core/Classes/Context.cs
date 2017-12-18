using System.Collections.Generic;
using System.IO;
using CameraControl.Devices;
using Capture.Workflow.Core.Scripting;
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

        /// <summary>
        /// Gets or sets a value indicating whether the captured photo transfer is enabled or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [capture enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool CaptureEnabled { get; set; }

        public Dictionary<string, CacheObject> Cache { get; set; }

        public Context()
        {
            CaptureEnabled = true;
            Cache = new Dictionary<string, CacheObject>();
        }

        public void Clear()
        {
            WorkFlow = null;
            foreach (var cacheObject in Cache)
            {
                cacheObject.Value.DisposeObject();
            }
            Cache.Clear();
            ScriptEngine.Instance.ClearCache();
        }
    }
}
