using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UserControl = System.Windows.Controls.UserControl;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class CopyFilePluginViewModel : ViewModelBase
    {
        private AutoExportPluginConfig _config = new AutoExportPluginConfig();

        public RelayCommand BrowseCommand { get; set; }

        public CopyFilePluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
            BrowseCommand = new RelayCommand(Browse);
        }

        private void Browse()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Path;
            if (dialog.ShowDialog() == DialogResult.OK)
                Path = dialog.SelectedPath;
        }

        public CopyFilePluginViewModel()
        {
            
        }

        public string Path
        {
            get { return _config.ConfigData["Path"]; }
            set
            {
                _config.ConfigData["Path"] = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public string TransformPlugin
        {
            get
            {
                var pl = _config.ConfigData["TransformPlugin"];
                return string.IsNullOrEmpty(pl) ? "NoTransform" : pl;
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

        public List<string> ImageTransformPlugins
        {
            get
            {
                var l = ServiceProvider.PluginManager.ImageTransformPlugins.Select(x => x.Name).ToList();
                return l;
            }
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

    }
}
