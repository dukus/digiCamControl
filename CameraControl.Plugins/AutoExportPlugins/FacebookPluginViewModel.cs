using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Facebook;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;

namespace CameraControl.Plugins.AutoExportPlugins
{
    class FacebookPluginViewModel : BasePluginViewModel
    {
        private const string Server = "http://localhost:15514/";
        private const string ClientId = "1389681264691751";
        private const string ClientSecret = "032eb21c825b83e04c1dc63e1689c7d1";

        private MiniWebServer _miniWebServer ;
        private bool _isLogedIn;
        private AsyncObservableCollection<ValuePair> _pages;
        private AsyncObservableCollection<ValuePair> _albums;
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand ShowAlbumCommand { get; set; }



        public AsyncObservableCollection<ValuePair> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                RaisePropertyChanged(()=>Pages);
            }
        }

        public AsyncObservableCollection<ValuePair> Albums
        {
            get { return _albums; }
            set
            {
                _albums = value;
                RaisePropertyChanged(() => Albums);
            }
        }

        public string AccessToken
        {
            get { return _config.ConfigData["AccessToken"]; }
            set
            {
                _config.ConfigData["AccessToken"] = value;
                LoadData();
            }
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

        public string SelectedPage
        {
            get { return _config.ConfigData["SelectedPage"]; }
            set
            {
                _config.ConfigData["SelectedPage"] = value;
                RaisePropertyChanged(() => SelectedPage);
                LoadAlbums();
            }
        }

        public string SelectedAlbum
        {
            get { return _config.ConfigData["SelectedAlbum"]; ; }
            set
            {
                _config.ConfigData["SelectedAlbum"] = value;
                RaisePropertyChanged(() => SelectedAlbum);
            }
        }

        public FacebookPluginViewModel()
        {
            _config = new AutoExportPluginConfig();
            IsLogedIn = true;
            Init();
        }

        public FacebookPluginViewModel(AutoExportPluginConfig config, bool loadData = true)
        {
            _config = config;
            Init();
            if (!string.IsNullOrEmpty(AccessToken) && loadData)
            {
                LoadData();
                LoadAlbums();
            }
        }

        private void Init()
        {
            ShowAlbumCommand = new RelayCommand(ShowAlbum);
            LoginCommand = new RelayCommand(Login);
            LogoutCommand = new RelayCommand(Logout);
            Pages = new AsyncObservableCollection<ValuePair>();
        }

        private void ShowAlbum()
        {
            if (!string.IsNullOrEmpty(SelectedAlbum) && SelectedAlbum.Contains("||"))
            {
                string id = SelectedAlbum.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries)[0];
                PhotoUtils.Run("https://www.facebook.com/" + id);
            }
        }

        private void Login()
        {
            try
            {
                FacebookClient client = new FacebookClient();
                client.AppId = ClientId;
                client.AppSecret = ClientSecret;
                //PhotoUtils.Run(GenerateLoginUrl(client.AppId,"publish_actions,manage_pages").ToString());
                _miniWebServer = new MiniWebServer(SendResponse, Server);
                _miniWebServer.Run();
                PhotoUtils.Run(
                    GenerateLoginUrl(client.AppId, "publish_actions,manage_pages,user_photos,publish_pages").ToString());
            }
            catch (Exception ex)
            {
                Log.Error("Facebook login error ", ex);
            }
        }

        private void Logout()
        {
            //FacebookClient client = new FacebookClient(AccessToken);
            //client.AppId = ClientId;
            //client.AppSecret = ClientSecret;
            //var logoutParameters = new Dictionary<string, object>
            //      {
            //          { "next", "http://www.facebook.com" }
            //      };

            ////PhotoUtils.Run(client.GetLogoutUrl(logoutParameters).ToString());
            SelectedAlbum = "";
            SelectedPage = "";
            AccessToken = "";
        }

        private void LoadData()
        {
            if (!string.IsNullOrEmpty(AccessToken))
            {
                try
                {
                    var client = new FacebookClient
                    {
                        AccessToken = AccessToken,
                        AppId = ClientId,
                        AppSecret = ClientSecret
                    };
                    var result = (IDictionary<string, object>) client.Get("me");
                    UserName = (string) result["name"];
                    dynamic fbAccounts = client.Get("/me/accounts");
                    Pages = new AsyncObservableCollection<ValuePair>
                    {
                        new ValuePair()
                        {
                            Name = "Profile page",
                            Value = (string) result["id"]+"||"+AccessToken
                        }
                    };
                    
                    if (string.IsNullOrEmpty(SelectedPage))
                        SelectedPage = Pages[0].Value;

                    foreach (dynamic data in fbAccounts.data)
                    {
                        Pages.Add(
                            new ValuePair()
                            {
                                Name = data.name,
                                Value = data.id + "||" + data.access_token
                            });
                    }
                    IsLogedIn = true;
                    
                }
                catch (Exception ex)
                {
                    Log.Error("Unable to login Facebook", ex);
                    AccessToken = null;
                }
            }
            else
            {
                IsLogedIn = false;
            }
        }

        private void LoadAlbums()
        {
            try
            {
                String url = string.Format("/{0}/albums", SelectedPage.Split(new []{"||"},StringSplitOptions.RemoveEmptyEntries)[0]);
                string token = SelectedPage.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries)[1];
                var client = new FacebookClient
                {
                    AccessToken = AccessToken,
                    AppId = ClientId,
                    AppSecret = ClientSecret
                };
                Albums = new AsyncObservableCollection<ValuePair>();
                do
                {
                    dynamic albums = client.Get(url, new { limit = 25, offset = 0 });

                    foreach (dynamic data in albums.data)
                    {
                        Albums.Add(new ValuePair() { Value = data.id+"||"+token, Name = data.name }); 
                    }

                    if (albums.paging != null && !String.IsNullOrEmpty(albums.paging.next))
                        url = albums.paging.next;
                    else
                        url = String.Empty;
                } while (!String.IsNullOrEmpty(url));
                if (string.IsNullOrEmpty(SelectedAlbum) && Albums.Any())
                {
                    SelectedAlbum = Albums[0].Value;
                }
            }
            catch (Exception)
            {

            }
        }

        public void UploadFile(string file, string filename)
        {
            if(string.IsNullOrEmpty(SelectedAlbum))
                throw  new Exception("Upload album not set");
            string id = SelectedAlbum.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0];
            string token = SelectedAlbum.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[1];
            var client = new FacebookClient
            {
                AccessToken = token,
                AppId = ClientId,
                AppSecret = ClientSecret
            };

            dynamic parameters = new ExpandoObject();
            //parameters.message = txtMessage.Text;
            parameters.source = new FacebookMediaObject
            {
                ContentType = "image/jpeg",
                FileName = filename,

            }.SetValue(File.ReadAllBytes(file));
            
            client.Post(id+"/photos", parameters);
        }

        private Uri GenerateLoginUrl(string appId, string extendedPermissions)
        {
            // for .net 3.5
            // var parameters = new Dictionary<string,object>
            // parameters["client_id"] = appId;
            dynamic parameters = new ExpandoObject();
            parameters.client_id = appId;
            parameters.redirect_uri = Server;

            // The requested response: an access token (token), an authorization code (code), or both (code token).
            parameters.response_type = "code";

            // list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display
            parameters.display = "page";

            // add the 'scope' parameter only if we have extendedPermissions.
            if (!string.IsNullOrWhiteSpace(extendedPermissions))
                parameters.scope = extendedPermissions;

            // generate the login url
            var fb = new FacebookClient();
            return fb.GetLoginUrl(parameters);
        }

        public string SendResponse(HttpListenerRequest request)
        {
            try
            {
                if (request.QueryString.HasKeys() && request.QueryString.Get("code") != null)
                {
                    string code = request.QueryString.Get("code");
                    string token_url =
                        string.Format(
                            "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&%20client_secret={2}&code={3}",
                            ClientId, Server, ClientSecret, code);
                    using (WebClient webclient = new WebClient())
                    {
                        var data = webclient.DownloadString(token_url);
                        dynamic obj= JsonConvert.DeserializeObject(data);
                        AccessToken = obj.access_token;
                        if (AccessToken != null)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => ((Window)ServiceProvider.PluginManager.SelectedWindow).Activate()));
                            return "<HTML><BODY>OK</br><h3>Login succeed. Please return to digiCamControl</br><a href=\"javascript:window.open('','_self').close();\">close</a></BODY></HTML>";
                        }
                    }
                }
             
            }
            catch (Exception e)
            {
                return string.Format("<HTML><BODY>Something goes wrong. Please try again "+e.Message +"</BODY></HTML>");
            }
            return string.Format("<HTML><BODY>Something goes wrong. Please try again</BODY></HTML>");
        }
    }
}
