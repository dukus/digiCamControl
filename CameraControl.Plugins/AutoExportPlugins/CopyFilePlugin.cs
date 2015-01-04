using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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
            string name = Path.GetFileName(filename);

            if (!string.IsNullOrEmpty(conf.FileName))
            {
                name = conf.FileName;
                if (name.Contains("%1"))
                    name = name.Replace("%1", Path.GetFileNameWithoutExtension(filename));
                if (!name.Contains("."))
                    name = name + Path.GetExtension(filename);
            }

            var outfile = Path.Combine(conf.Path, name);
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
            wnd.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
            wnd.ShowDialog();
            return true;
        }

        public string Name
        {
            get { return "Copy File"; } 
        }
    }
}
