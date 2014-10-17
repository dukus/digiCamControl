using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class ExecuteFilePluginViewModel : BasePluginViewModel
    {
        public RelayCommand BrowseCommand { get; set; }

        public ExecuteFilePluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
            BrowseCommand = new RelayCommand(Browse);
        }

        public ExecuteFilePluginViewModel()
        {
            
        }

        public string PathToExe
        {
            get { return _config.ConfigData["PathToExe"]; }
            set
            {
                _config.ConfigData["PathToExe"] = value;
                RaisePropertyChanged(() => PathToExe);
            }
        }

        public string Params
        {
            get { return _config.ConfigData["Params"]; }
            set
            {
                _config.ConfigData["Params"] = value;
                RaisePropertyChanged(() => Params);
            }
        }

        private void Browse()
        {
            OpenFileDialog dialog=new OpenFileDialog();
            dialog.FileName = PathToExe;
            if (dialog.ShowDialog() == true)
            {
                PathToExe = dialog.FileName;
            }
        }
    }
}
