using System.IO;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class TransformPlugin : IAutoExportPlugin
    {
        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            configData.IsRedy = false;
            configData.IsError = false;
            var filename = item.FileName;

            var outfile = Path.GetTempFileName();
            AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);
            // wait for file to be not locked
            PhotoUtils.WaitForFile(filename);
            File.Copy(outfile, filename, true);
            File.Delete(outfile);
            item.IsLoaded = false;
            item.RemoveThumbs();
            // remove unused file
            if (outfile != item.FileName)
            {
                PhotoUtils.WaitForFile(outfile);
                File.Delete(outfile);
            }
            configData.IsRedy = true;
            return true; 
        }

        public string Name
        {
            get { return "Transform"; }
        }

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cnt = new TransformPluginConfig
            {
                DataContext = new TransformPluginViewModel(configData)
            };
            return cnt;
        }
    }
}
