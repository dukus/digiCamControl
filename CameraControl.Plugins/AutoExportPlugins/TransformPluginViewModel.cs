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

        public TransformPluginViewModel()
        {
            
        }
    }
}
