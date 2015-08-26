using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using DropNet;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class DropboxViewModel : BasePluginViewModel
    {
        private const string Server = "http://localhost:5514/";
        DropNetClient _client = new DropNetClient("71jiamqc1znjurb", "yt7p9rxiq87cpnd");
        private MiniWebServer _miniWebServer ;
        private bool _isLogedIn;

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }

        public string AccessToken
        {
            get { return _config.ConfigData["AccessToken"]; }
            set
            {
                _config.ConfigData["AccessToken"] = value;
                LoadData();
            }
        }

        public string Secret
        {
            get { return _config.ConfigData["Secret"]; }
            set { _config.ConfigData["Secret"] = value; }
        }

        public bool IsLogedIn
        {
            get { return _isLogedIn; }
            set
            {
                _isLogedIn = value;
                RaisePropertyChanged(() => IsLogedIn);
                RaisePropertyChanged(() => IsLogedOut);
            }
        }

        public bool IsLogedOut
        {
            get { return !IsLogedIn; }
        }

        public string UserName
        {
            get { return _config.ConfigData["UserName"]; }
            set
            {
                _config.ConfigData["UserName"] = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public DropboxViewModel()
        {
            _config = new AutoExportPluginConfig();
        }

        public DropboxViewModel(AutoExportPluginConfig configData)
        {
            _config = configData;
            LoginCommand = new RelayCommand(Login);
            LogoutCommand = new RelayCommand(LogOut);
            LoadData();
        }

        private void LogOut()
        {
            AccessToken = "";
        }

        private void Login()
        {
            try
            {
                _client.GetToken();
                var url = _client.BuildAuthorizeUrl(Server);
                _miniWebServer = new MiniWebServer(SendResponse, Server);
                _miniWebServer.Run();
                PhotoUtils.Run(url);
            }
            catch (Exception e)
            {
                Log.Error("Unable to login");
            }
        }

        public string SendResponse(HttpListenerRequest request)
        {
            var accessToken = _client.GetAccessToken();
            Application.Current.Dispatcher.Invoke(new Action(() => ((Window)ServiceProvider.PluginManager.SelectedWindow).Activate()));
            Secret = accessToken.Secret;
            AccessToken = accessToken.Token;

            return "<HTML><BODY>OK</br><h3>Logis succeed. Please return to digiCamControl</br><a href=\"javascript:window.open('','_self').close();\">close</a></BODY></HTML>";
            return string.Format("<HTML><BODY>Something goes wrong. Please try again</BODY></HTML>");
        }

        public void Upload(string file, string folder)
        {
            LoadData();
            using (Stream stream = File.Open(file,FileMode.Open,FileAccess.Read))
            {
                _client.UploadFile(folder, Path.GetFileName(file), stream);
            }
        }

        private void LoadData()
        {
            try
            {
                if (!string.IsNullOrEmpty(AccessToken))
                {
                    _client.GetToken();
                    _client.UserLogin.Secret = Secret;
                    _client.UserLogin.Token = AccessToken;
                    var data = _client.AccountInfo();
                    UserName = data.display_name;
                    IsLogedIn = true;
                }
            }
            catch (Exception ex)
            {
                IsLogedIn = false;
            }
        }
    }
}
