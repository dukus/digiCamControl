using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class FtpPluginViewModel : BasePluginViewModel
    {
        public FtpPluginViewModel()
        {
            
        }

        public FtpPluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
        }

        public string Server
        {
            get { return _config.ConfigData["Server"]; }
            set
            {
                _config.ConfigData["Server"] = value;
                RaisePropertyChanged(() => Server);
            }
        }

        public string User
        {
            get { return _config.ConfigData["User"]; }
            set
            {
                _config.ConfigData["User"] = value;
                RaisePropertyChanged(() => User);
            }
        }


        public string Pass
        {
            get { return _config.ConfigData["Pass"]; }
            set
            {
                _config.ConfigData["Pass"] = value;
                RaisePropertyChanged(() => Pass);
            }
        }

        public string ServerPath
        {
            get { return _config.ConfigData["ServerPath"]; }
            set
            {
                _config.ConfigData["ServerPath"] = value;
                RaisePropertyChanged(() => ServerPath);
            }
        }

    }
}
