using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class DropboxPlugin : IAutoExportPlugin
    {
        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            Thread thread = new Thread(() => Send(item, configData));
            thread.Start();
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

        public void Send(FileItem item, AutoExportPluginConfig configData)
        {
            try
            {
                var filename = item.FileName;
                configData.IsRedy = false;
                configData.IsError = false;
                var conf = new DropboxViewModel(configData);
                
                var outfile = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));
                outfile = AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);

                conf.Upload(outfile,ServiceProvider.Settings.DefaultSession.Name);

                // remove unused file
                if (outfile != item.FileName)
                {
                    PhotoUtils.WaitForFile(outfile);
                    File.Delete(outfile);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error send dropbox file", exception);
                configData.IsError = true;
                configData.Error = exception.Message;
            }
            configData.IsRedy = true;
        }
    }
}
