using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class TransformPluginViewModel : BasePluginViewModel
    {
       
        public TransformPluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
        }

        public bool CreateNew
        {
            get { return _config.ConfigData["CreateNew"] == "True"; }
            set
            {
                _config.ConfigData["CreateNew"] = value.ToString();
                RaisePropertyChanged(() => OverWrite);
            }
        }

        public bool OverWrite
        {
            get { return !CreateNew; }

        }


        public TransformPluginViewModel()
        {
            
        }
    }
}
