using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class CopyFilePluginViewModel : ViewModelBase
    {
        private AutoExportPluginConfig _config = new AutoExportPluginConfig();
        private string _transformPlugin;

        public CopyFilePluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
        }

        public string Path
        {
            get { return _config.ConfigData["Path"]; }
            set { _config.ConfigData["Path"] = value; }
        }

        public string TransformPlugin
        {
            get { return _config.ConfigData["TransformPlugin"]; }
            set { _config.ConfigData["TransformPlugin"] = value; }
        }
    }
}
