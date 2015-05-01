using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class FacebookPlugin : IAutoExportPlugin
    {
        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            Thread thread = new Thread(() => Send(item, configData));
            thread.Start();
            return true;
        }

        public void Send(FileItem item, AutoExportPluginConfig configData)
        {
            try
            {
                var filename = item.FileName;
                configData.IsRedy = false;
                configData.IsError = false;
                var conf = new FacebookPluginViewModel(configData, false);

                var outfile = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));
                outfile = AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);

                conf.UploadFile(outfile);

                // remove unused file
                if (outfile != item.FileName)
                {
                    PhotoUtils.WaitForFile(outfile);
                    File.Delete(outfile);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error send facebook file", exception);
                configData.IsError = true;
                configData.Error = exception.Message;
            }
            configData.IsRedy = true;
        }

        public string Name
        {
            get { return "Facebook"; }
        }

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cnt = new FacebookPluginConfig()
            {
                DataContext = new FacebookPluginViewModel(configData)
            };
            return cnt;
        }
    }
}
