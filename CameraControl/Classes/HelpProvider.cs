#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using CameraControl.Core;
using CameraControl.Core.Classes;
using Ionic.Zip;
using Typesafe.Mailgun;

#endregion

namespace CameraControl.Classes
{
    public enum HelpSections
    {
        MainMenu,
        FocusStacking,
        Bracketig,
        Settings,
        TimeLapse,
        LiveView,
        Session,
        Bulb,
        MultipleCamera,
        DownloadPhotos
    }

    public class HelpProvider
    {
        private static Dictionary<HelpSections, string> _helpData;

        private static void Init()
        {
            _helpData = new Dictionary<HelpSections, string>
                            {
                                {HelpSections.MainMenu, "http://www.digicamcontrol.com/manual"},
                                {HelpSections.Bracketig, "http://www.digicamcontrol.com/manual/bracketing"},
                                {
                                    HelpSections.FocusStacking,
                                    "http://digicamcontrol.com/manual/focus-stacking"
                                    },
                                {HelpSections.Settings, "http://digicamcontrol.com/wiki/index.php/Settings"},
                                {HelpSections.TimeLapse, "http://www.digicamcontrol.com/manual/time-lapse"},
                                {HelpSections.LiveView, "http://www.digicamcontrol.com/manual/live-view"},
                                {HelpSections.Session, "http://digicamcontrol.com/wiki/index.php/Session"},
                                {HelpSections.Bulb, "http://www.digicamcontrol.com/manual/bulb-mode"},
                                {HelpSections.MultipleCamera, "http://www.digicamcontrol.com/manual/multiple-cameras"},
                                {HelpSections.DownloadPhotos, "http://www.digicamcontrol.com/manual/download-photos"},
                            };
        }


        public static void Run(HelpSections sections)
        {
            if (_helpData == null)
                Init();
            PhotoUtils.Run(_helpData[sections], "");
        }

        public static void SendCrashReport(string body, string type, string email=null)
        {
            try
            {
                string destfile = Path.Combine(Path.GetTempPath(), "error_report.zip");
                if (File.Exists(destfile))
                    File.Delete(destfile);

                using (var zip = new ZipFile(destfile))
                {
                    zip.AddFile(ServiceProvider.LogFile, "");
                    zip.Save(destfile);
                }
                var client = new MailgunClient("digicamcontrol.mailgun.org", "key-6n75wci5cpuz74vsxfcwfkf-t8v74g82");
                var message = new MailMessage(email ?? "error@digicamcontrol.mailgun.org",
                    "error_report@digicamcontrol.com")
                {
                    Subject = (type ?? "Log file"),
                    Body = "Version :" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "\n" +
                           "Client Id" + (ServiceProvider.Settings.ClientId ?? "") + "\n" + body,
                };
                message.Attachments.Add(new Attachment(destfile));

                client.SendMail(message);
                message.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}