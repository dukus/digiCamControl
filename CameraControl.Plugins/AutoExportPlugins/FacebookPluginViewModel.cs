using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using Facebook;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.AutoExportPlugins
{
    class FacebookPluginViewModel : BasePluginViewModel
    {
        public RelayCommand LoginCommand { get; set; }
        
        public FacebookPluginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        public FacebookPluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            FacebookClient client=new FacebookClient();
            client.AppId = "1389684908024720";
            client.AppSecret = "20a29131265de09bf1421ed1284ca23e";
            PhotoUtils.Run(GenerateLoginUrl(client.AppId,"publish_actions,manage_pages").ToString());
            
        }

        private Uri GenerateLoginUrl(string appId, string extendedPermissions)
        {
            // for .net 3.5
            // var parameters = new Dictionary<string,object>
            // parameters["client_id"] = appId;
            dynamic parameters = new ExpandoObject();
            parameters.client_id = appId;
            parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";

            // The requested response: an access token (token), an authorization code (code), or both (code token).
            parameters.response_type = "token";

            // list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display
            parameters.display = "popup";

            // add the 'scope' parameter only if we have extendedPermissions.
            if (!string.IsNullOrWhiteSpace(extendedPermissions))
                parameters.scope = extendedPermissions;

            // generate the login url
            var fb = new FacebookClient();
            return fb.GetLoginUrl(parameters);
        }
    }
}
