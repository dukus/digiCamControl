using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class ExecuteFilePlugin : IAutoExportPlugin
    {
        public bool Execute(string filename, AutoExportPluginConfig configData)
        {
            configData.IsRedy = false;
            configData.IsError = false;
            var conf = new ExecuteFilePluginViewModel(configData);
            if (string.IsNullOrEmpty(conf.PathToExe) || !File.Exists(conf.PathToExe))
            {
                configData.IsRedy = true;
                configData.IsError = true;
                configData.Error = "No executable path was set or executable not found";
                return false;
            }
            var outfile = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));
            var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(conf.TransformPlugin);

            outfile = tp != null && conf.TransformPlugin != BasePluginViewModel.EmptyTransformFilter
                ? tp.Execute(filename, outfile, configData.ConfigData)
                : filename;

            if (File.Exists(outfile))
            {
                PhotoUtils.Run(conf.PathToExe,
                    string.IsNullOrEmpty(conf.Params) ? outfile : conf.Params.Replace("%1", outfile), ProcessWindowStyle.Normal);
            }
            else
            {
                configData.IsError = true;
                configData.Error = "Output file not found !";
            }
            configData.IsRedy = true;
            return true;  
        }

        public bool Configure(AutoExportPluginConfig config)
        {
            ExecuteFilePluginConfig wnd = new ExecuteFilePluginConfig();
            wnd.DataContext = new ExecuteFilePluginViewModel(config);
            wnd.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
            wnd.ShowDialog();
            return true;
        }

        public string Name
        {
            get { return "Execute File"; }
        }
    }
}
