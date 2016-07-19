using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

using UserControl = System.Windows.Controls.UserControl;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class SendEmailPlugin : IAutoExportPlugin
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
                configData.IsRedy = false;
                configData.IsError = false;
                var conf = new SendEmailPluginViewModel(configData);

                var outfile = PhotoUtils.ReplaceExtension(Path.GetTempFileName(),Path.GetExtension(item.Name));
                outfile = AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);

                HelpProvider.SendEmail((string.IsNullOrEmpty(conf.Message) ? "." : conf.TransformTemplate(item, conf.Message)), (string.IsNullOrEmpty(conf.Subject) ? "Your photo" : conf.TransformTemplate(item, conf.Subject)),
                    conf.From, conf.To,outfile);

                // remove unused file
                if (outfile != item.FileName)
                {
                    PhotoUtils.WaitForFile(outfile);
                    File.Delete(outfile);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error send email file", exception);
                configData.IsError = true;
                configData.Error = exception.Message;
            }
            configData.IsRedy = true;
        }

        public string Name => "Email";

        public UserControl GetConfig(AutoExportPluginConfig configData)
        {
            var cnt = new SendEmailPluginConfig()
            {
                DataContext = new SendEmailPluginViewModel(configData)
            };
            return cnt;
        }
        
    }
}
