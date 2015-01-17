using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Text;
using System.Threading;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class FtpPlugin : IAutoExportPlugin
    {
        public bool Execute(string filename, AutoExportPluginConfig configData)
        {
            Thread thread = new Thread(() => Send(filename, configData));
            thread.Start();
            return true;
        }

        private void Send(string filename, AutoExportPluginConfig configData)
        {
            try
            {
                configData.IsRedy = false;
                configData.IsError = false;
                var conf = new FtpPluginViewModel(configData);
                var outfile = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));
                var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(conf.TransformPlugin);

                outfile = tp != null && conf.TransformPlugin != BasePluginViewModel.EmptyTransformFilter
                    ? tp.Execute(filename, outfile, configData.ConfigData)
                    : filename;
                using (FtpClient conn = new FtpClient())
                {
                    conn.Host = conf.Server;
                    conn.Credentials = new NetworkCredential(conf.User, conf.Pass);
                    if (!string.IsNullOrWhiteSpace(conf.ServerPath))
                        conn.SetWorkingDirectory(conf.ServerPath);
                    using (Stream ostream = conn.OpenWrite(Path.GetFileName(outfile)))
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
            }
            catch (Exception exception)
            {
                Log.Error("Error senf ftp file", exception);
                configData.IsError = true;
                configData.Error = exception.Message;
            }
            configData.IsRedy = true;   
        }

        public bool Configure(AutoExportPluginConfig config)
        {
            FtpPluginConfig wnd = new FtpPluginConfig();
            wnd.DataContext = new FtpPluginViewModel(config);
            wnd.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
            wnd.ShowDialog();
            return true;
        }

        public string Name
        {
            get { return "Ftp"; }
        }
    }
}
