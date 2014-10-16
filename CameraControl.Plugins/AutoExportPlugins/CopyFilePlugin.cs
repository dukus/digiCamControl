using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class CopyFilePlugin: IAutoExportPlugin
    {

        public bool Execute(string filename, AutoExportPluginConfig configData)
        {
            return true;            
        }

        public bool Configure(AutoExportPluginConfig config)
        {
            CopyFilePluginConfig wnd = new CopyFilePluginConfig();
            wnd.ShowDialog();
            return true;
        }

        public string Name
        {
            get { return "Copy File"; } 
        }
    }
}
