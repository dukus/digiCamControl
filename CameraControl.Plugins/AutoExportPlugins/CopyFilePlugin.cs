using System.IO;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class CopyFilePlugin: IAutoExportPlugin
    {

        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            configData.IsRedy = false;
            configData.IsError = false;
            var filename = item.FileName;
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
            AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile); 
            
            configData.IsRedy = true;
            return true;            
        }

        public string Name
        {
            get { return "Copy File"; } 
        }

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cntr = new CopyFilePluginConfig()
            {
                DataContext = new CopyFilePluginViewModel(configData)
            };
            return cntr;
        }
    }
}
