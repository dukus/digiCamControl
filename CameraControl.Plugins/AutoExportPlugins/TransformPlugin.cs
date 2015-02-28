using System.IO;
using System.Windows;
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
            var conf = new TransformPluginViewModel(configData);


            var outfile = Path.GetTempFileName();
            var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(conf.TransformPlugin);
            if (tp != null)
            {
                tp.Execute(item, outfile, configData.ConfigData);
            }
            // wait for file to be not locked
            PhotoUtils.WaitForFile(filename);
            File.Copy(outfile, filename, true);
            File.Delete(outfile);
            item.IsLoaded = false;
            item.RemoveThumbs();
            configData.IsRedy = true;
            return true; 
        }

        public bool Configure(AutoExportPluginConfig config)
        {
            var wnd = new TransformPluginConfig
            {
                DataContext = new TransformPluginViewModel(config),
                Owner = ServiceProvider.PluginManager.SelectedWindow as Window
            };
            wnd.ShowDialog();
            return true;
        }

        public string Name
        {
            get { return "Transform"; }
        }
    }
}
