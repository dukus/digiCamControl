using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class ExecuteFilePlugin : IAutoExportPlugin
    {
        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            configData.IsRedy = false;
            configData.IsError = false;
            var filename = item.FileName;
            var conf = new ExecuteFilePluginViewModel(configData);
            if (string.IsNullOrEmpty(conf.PathToExe) || !File.Exists(conf.PathToExe))
            {
                configData.IsRedy = true;
                configData.IsError = true;
                configData.Error = "No executable path was set or executable not found";
                return false;
            }
            var outfile = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));

            outfile = AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);

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

        public string Name
        {
            get { return "Execute File"; }
        }

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cntr = new ExecuteFilePluginConfig()
            {
                DataContext = new ExecuteFilePluginViewModel(configData)
            };
            return cntr;
        }
    }
}
