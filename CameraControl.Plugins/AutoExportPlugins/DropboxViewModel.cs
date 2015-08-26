using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using DropNet;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class DropboxViewModel : BasePluginViewModel
    {
        private const string Server = "http://localhost:5514/";
        DropNetClient _client = new DropNetClient("71jiamqc1znjurb", "yt7p9rxiq87cpnd");
        private MiniWebServer _miniWebServer ;

        public RelayCommand LoginCommand { get; set; } 

        public DropboxViewModel()
        {
            _config = new AutoExportPluginConfig();
        }

        public DropboxViewModel(AutoExportPluginConfig configData)
        {
            _config = configData;
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            _client.GetToken();
            var url = _client.BuildAuthorizeUrl(Server);
            _miniWebServer = new MiniWebServer(SendResponse, Server);
            _miniWebServer.Run();
            PhotoUtils.Run(url);
        }

        public string SendResponse(HttpListenerRequest request)
        {
            var accessToken = _client.GetAccessToken();
            Application.Current.Dispatcher.Invoke(new Action(() => ((Window)ServiceProvider.PluginManager.SelectedWindow).Activate()));

            return "<HTML><BODY>OK</br><h3>Logis succeed. Please return to digiCamControl</br><a href=\"javascript:window.open('','_self').close();\">close</a></BODY></HTML>";
            return string.Format("<HTML><BODY>Something goes wrong. Please try again</BODY></HTML>");
        }
    }
}
