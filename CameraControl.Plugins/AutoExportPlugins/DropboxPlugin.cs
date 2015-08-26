using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class DropboxPlugin : IAutoExportPlugin
    {
        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            return true;
        }

        public string Name
        {
            get { return "Dropbox"; }
        }

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cnt = new DropboxConfig()
            {
                DataContext = new DropboxViewModel(configData)
            };
            return cnt;
        }
    }
}
