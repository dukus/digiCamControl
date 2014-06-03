using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.Windows.Shapes.Path;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for NewVersionWnd.xaml
    /// </summary>
    public partial class NewVersionWnd
    {
        private string _downloadUrl = "";
        private string _logUrl = "";
        public NewVersionWnd(string changelog, string download)
        {
            _downloadUrl = download;
            _logUrl = changelog;
            InitializeComponent();
            if (string.IsNullOrEmpty(download))
                button2.Visibility = Visibility.Hidden;

            //ServiceProvider.Settings.ApplyTheme(this);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(_downloadUrl);
                ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.All_Close);
            }
            catch (Exception)
            {
                MessageBox.Show("Error initialize download !");
            }
        }

        public static void ShowChangeLog()
        {
            var wnd = new NewVersionWnd("http://digicamcontrol.com/updates/changelog.html", "");
            wnd.ShowDialog();
           
        }



        public static bool CheckForUpdate(bool notify)
        {
            try
            {
                string tempfile = System.IO.Path.GetTempFileName();
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("http://digicamcontrol.com/updates/versioninfo.xml", tempfile);
                }

                XmlDocument document = new XmlDocument();
                document.Load(tempfile);
                string ver = document.SelectSingleNode("application/version").InnerText;
                string url = "http://digicamcontrol.com/download";
                string logurl = "";
                var selectSingleNode = document.SelectSingleNode("application/url");
                if (selectSingleNode != null)
                    url = selectSingleNode.InnerText;
                var logNode = document.SelectSingleNode("application/logurl");
                if (logNode != null)
                    logurl = logNode.InnerText;
                Version v_ver = new Version(ver);
                if (v_ver > Assembly.GetExecutingAssembly().GetName().Version)
                {
                    var wnd = new NewVersionWnd(logurl, url);
                    wnd.ShowDialog();
                }
                else
                {
                    if (notify)
                        MessageBox.Show(TranslationStrings.MsgApplicationUpToDate);
                }
                File.Delete(tempfile);
            }
            catch (Exception exception)
            {
                Log.Error("Error download update information", exception);
            }
            return false;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Source = new Uri(_logUrl);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
