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
using System.Net;
using System.Net.Mail;
using CameraControl.Devices;
using Ionic.Zip;

#endregion

namespace CameraControl.Core.Classes
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
                                {HelpSections.MainMenu, "http://digicamcontrol.com/doc/userguide"},
                                {HelpSections.Bracketig, "http://digicamcontrol.com/doc/userguide/interface/bracketing"},
                                {
                                    HelpSections.FocusStacking,
                                    "http://digicamcontrol.com/doc/userguide/interface/liveview#focus-stacking"
                                    },
                                {HelpSections.Settings, "http://digicamcontrol.com/doc/userguide/settings"},
                                {HelpSections.TimeLapse, "http://digicamcontrol.com/doc/userguide/interface/timelapse"},
                                {HelpSections.LiveView, "http://digicamcontrol.com/doc/userguide/interface/liveview"},
                                {HelpSections.Session, "http://digicamcontrol.com/doc/userguide/session"},
                                {HelpSections.Bulb, "http://digicamcontrol.com/doc/userguide/interface/astromod"},
                                {HelpSections.MultipleCamera, "http://digicamcontrol.com/doc/userguide/interface/multicam"},
                                {HelpSections.DownloadPhotos, "http://digicamcontrol.com/doc/userguide/interface/downph"},
                            };
        }


        public static void Run(HelpSections sections)
        {
            if (_helpData == null)
                Init();
            PhotoUtils.Run(_helpData[sections], "");
        }

        public static void SendEmail(string body, string subject, string from, string to, string file = null)
        {
            using (SmtpClient mailClient = new SmtpClient("smtp.sendgrid.net", 587))
            {
                // Set the network credentials.
                mailClient.Credentials = new NetworkCredential(CameraControl.Private.Ids.SendgridUser, CameraControl.Private.Ids.SendgridPass);

                //Enable SSL.
                //mailClient.EnableSsl = true;

                var message = new MailMessage("admin@digicamcontrol.com", to)
                {
                    Subject = subject,
                    Body = body ?? "",                    
                    IsBodyHtml = false
                };
                message.ReplyToList.Add(from);
                if (File.Exists(file))
                    message.Attachments.Add(new Attachment(file));

                mailClient.Send(message);
                message.Dispose();
            }
        }

        public static void SendCrashReport(string body, string type, string email = null)
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
                body = "Version :" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "\n" +
                       "Client Id" + (ServiceProvider.Settings.ClientId ?? "") + "\n" + body;
                SendEmail(body, (type ?? "Log file"),
                    string.IsNullOrWhiteSpace(email) ? "admin@digicamcontrol.com" : email, "error_report@digicamcontrol.com", destfile);
            }
            catch (Exception ex)
            {
                Log.Error("Error send email", ex);
            }
        }
    }
}