using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class BasePluginViewModel : ViewModelBase
    {

        protected AutoExportPluginConfig _config = new AutoExportPluginConfig();
        public const string EmptyTransformFilter = "NoTransform";

        public string TransformPlugin
        {
            get
            {
                var pl = _config.ConfigData["TransformPlugin"];
                return string.IsNullOrEmpty(pl) ? EmptyTransformFilter : pl;
            }
            set
            {
                _config.ConfigData["TransformPlugin"] = value;
                RaisePropertyChanged(() => ConfigControl);
            }
        }

        public string Name
        {
            get { return _config.Name; }
            set { _config.Name = value; }
        }

        public UserControl ConfigControl
        {
            get
            {
                var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(TransformPlugin);
                if (tp != null)
                {
                    return tp.GetConfig(_config.ConfigData);
                }
                return null;
            }
        }

        public int ToInt(string name)
        {
            int i = 0;
            int.TryParse(_config.ConfigData[name], out i);
            return i;
        }
    }
}
