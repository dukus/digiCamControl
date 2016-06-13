using System;
using System.IO;
using System.Net;
using System.Net.FtpClient;
using System.Threading;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class FtpPlugin : IAutoExportPlugin
    {
        public bool Execute(FileItem item, AutoExportPluginConfig configData)
        {
            Thread thread = new Thread(() => Send(item, configData));
            thread.Start();
            return true;
        }

        private void Send(FileItem item, AutoExportPluginConfig configData)
        {
            try
            {
                configData.IsRedy = false;
                configData.IsError = false;
                var conf = new FtpPluginViewModel(configData);

                var outfile = PhotoUtils.ReplaceExtension(Path.GetTempFileName(), Path.GetExtension(item.Name));
                outfile = AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);

                using (FtpClient conn = new FtpClient())
                {
                    conn.Host = conf.Server;
                    conn.Credentials = new NetworkCredential(conf.User, conf.Pass);
                    if (!string.IsNullOrWhiteSpace(conf.ServerPath))
                        conn.SetWorkingDirectory(conf.ServerPath);
                    using (Stream ostream = conn.OpenWrite(item.Name))
                    {
                        try
                        {
                            var data = File.ReadAllBytes(outfile);
                            ostream.Write(data, 0, data.Length);
                        }
                        finally
                        {
                            ostream.Close();
                        }
                    }
                }
                // remove unused file
                if (outfile != item.FileName)
                {
                    PhotoUtils.WaitForFile(outfile);
                    File.Delete(outfile);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error senf ftp file", exception);
                configData.IsError = true;
                configData.Error = exception.Message;
            }
            configData.IsRedy = true;   
        }

        public string Name
        {
            get { return "Ftp"; }
        }

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cnt = new FtpPluginConfig()
            {
                DataContext = new FtpPluginViewModel(configData)
            };
            return cnt;
        }
    }
}
