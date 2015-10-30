using System;
using System.IO;
using System.Net;
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
        private const string AppKey = "71jiamqc1znjurb";
        private const string AppSecret = "yt7p9rxiq87cpnd";
        private const string Server = "http://localhost:5514/";
        private DropNetClient _client;
        private MiniWebServer _miniWebServer ;
        private bool _isLogedIn;

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand ShowFolderCommand { get; set; }

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
            ShowFolderCommand = new RelayCommand(ShowFolder);
            LoadData();
        }

        private void LogOut()
        {
            AccessToken = "";
            Secret = "";
            _client = null;
        }

        private void Login()
        {
            try
            {
                if (_client == null)
                {
                    _client = new DropNetClient(AppKey, AppSecret);
                    _client.GetToken();                    
                }
                var url = _client.BuildAuthorizeUrl(Server);
                _miniWebServer = new MiniWebServer(SendResponse, Server);
                _miniWebServer.Run();
                PhotoUtils.Run(url);
            }
            catch (Exception)
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
        }

        public void Upload(string file, string folder)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new Exception("Not loged in !");
            }
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
                    _client = new DropNetClient(AppKey, AppSecret, AccessToken, Secret);
                    var data = _client.AccountInfo();
                    UserName = data.display_name;
                    IsLogedIn = true;
                }
                else
                {
                    IsLogedIn = false;
                }
            }
            catch (Exception)
            {
                IsLogedIn = false;
            }
        }

        private void ShowFolder()
        {
            PhotoUtils.Run("https://www.dropbox.com/home/Apps/digiCamControl");
        }
    }
}
