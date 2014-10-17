using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class CopyFilePlugin: IAutoExportPlugin
    {

        public bool Execute(string filename, AutoExportPluginConfig configData)
        {
            configData.IsRedy = false;
            configData.IsError = false;
            var conf = new CopyFilePluginViewModel(configData);
            if (string.IsNullOrEmpty(conf.Path))
            {
                configData.IsRedy = true;
                configData.IsError = true;
                configData.Error = "No export path was set";
                return false;
            }
            if (!Directory.Exists(conf.Path))
                Directory.CreateDirectory(conf.Path);
            var outfile = Path.Combine(conf.Path, Path.GetFileName(filename));
            var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(conf.TransformPlugin);
            if (tp != null)
            {
                tp.Execute(filename, outfile, configData.ConfigData);
            }
            configData.IsRedy = true;
            return true;            
        }

        public bool Configure(AutoExportPluginConfig config)
        {
            CopyFilePluginConfig wnd = new CopyFilePluginConfig();
            wnd.DataContext = new CopyFilePluginViewModel(config);
            wnd.ShowDialog();
            return true;
        }

        public string Name
        {
            get { return "Copy File"; } 
        }
    }
}
