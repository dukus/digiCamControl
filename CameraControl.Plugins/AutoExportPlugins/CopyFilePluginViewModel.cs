using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class CopyFilePluginViewModel : ViewModelBase
    {
        private AutoExportPluginConfig _config = new AutoExportPluginConfig();

        public CopyFilePluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
        }

        public CopyFilePluginViewModel()
        {
            
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

        public string Name
        {
            get { return _config.Name; }
            set { _config.Name = value; }
        }

        public List<string> ImageTransformPlugins
        {
            get
            {
                var l = ServiceProvider.PluginManager.ImageTransformPlugins.Select(x => x.Name).ToList();
                return l;
            }
        }
    }
}
