using System;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using Typesafe.Mailgun;

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
                var filename = item.FileName;
                configData.IsRedy = false;
                configData.IsError = false;
                var conf = new SendEmailPluginViewModel(configData);

                var outfile = Path.Combine(Path.GetTempPath(), Path.GetFileName(filename));
                outfile = AutoExportPluginHelper.ExecuteTransformPlugins(item, configData, outfile);

                var client = new MailgunClient("digicamcontrol.mailgun.org", "key-6n75wci5cpuz74vsxfcwfkf-t8v74g82");
                var message = new MailMessage(conf.From, conf.To)
                {
                    Subject = (string.IsNullOrEmpty(conf.Subject) ? "Your photo":conf.TransformTemplate(item,conf.Subject)),
                    Body = (string.IsNullOrEmpty(conf.Message) ? "." : conf.TransformTemplate(item, conf.Message)),
                    IsBodyHtml = true
                };
                message.Attachments.Add(new Attachment(outfile));

                client.SendMail(message);
                message.Dispose();

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
            get { return "Email"; }
        }

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
