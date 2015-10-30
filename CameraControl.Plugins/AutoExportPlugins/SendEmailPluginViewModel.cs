using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class SendEmailPluginViewModel:BasePluginViewModel
    {
        public string To
        {
            get { return _config.ConfigData["To"]; }
            set { _config.ConfigData["To"] = value; }
        }

        public string From
        {
            get { return _config.ConfigData["From"]; }
            set { _config.ConfigData["From"] = value; }
        }

        public string Message
        {
            get { return _config.ConfigData["Message"]; }
            set { _config.ConfigData["Message"] = value; }
        }

        public string Subject
        {
            get { return _config.ConfigData["Subject"]; }
            set { _config.ConfigData["Subject"] = value; }
        }

        public SendEmailPluginViewModel()
        {
            
        }

        public SendEmailPluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
        }

    }
}
