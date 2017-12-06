using System;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Threading;
using Capture.Workflow.Core.Classes;
using System.Web;

namespace Capture.Workflow.Classes
{
    public class GoogleAnalytics
    {
        private static GoogleAnalytics _instance;

        private string googleURL = "https://ssl.google-analytics.com/collect";
        private string googleVersion = "1";
        private string googleTrackingID = "UA-36038932-4";
        private string googleClientID = Settings.Instance.ClientId;
        private string AppVersion = "0.0.0.0";

        public static GoogleAnalytics Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GoogleAnalytics();
                return _instance;
            }
        }


        public GoogleAnalytics()
        {
            try
            {
                AppVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            catch { }
        }

        public void TrackEvent(string category, string action, string label = null, string value = null)
        {
            Hashtable ht = baseValues();

            ht.Add("t", "event"); // Event hit type
            ht.Add("ec", category); // Event Category. Required.
            ht.Add("ea", action); // Event Action. Required.
            if (label != null) ht.Add("el", label); // Event label.
            if (value != null) ht.Add("ev", value); // Event value.

            PostData(ht);
        }

        public void TrackScreenView(string screenName)
        {
            Hashtable ht = baseValues();

            ht.Add("t", "screenview");                // Pageview hit type.
            ht.Add("cd", screenName);                 // Screen Name.
            PostData(ht);
        }

        public void TrackPage(string hostname, string page, string title)
        {
            Hashtable ht = baseValues();

            ht.Add("t", "pageview");                // Pageview hit type.
            ht.Add("dh", hostname);                 // Document hostname.
            ht.Add("dp", page);                     // Page.
            ht.Add("dt", title);                    // Title.

            PostData(ht);
        }

        public void EcommerceTransaction(string id, string affiliation, string revenue, string shipping, string tax, string currency)
        {
            Hashtable ht = baseValues();

            ht.Add("t", "transaction");       // Transaction hit type.
            ht.Add("ti", id);                 // transaction ID.            Required.
            ht.Add("ta", affiliation);        // Transaction affiliation.
            ht.Add("tr", revenue);            // Transaction revenue.
            ht.Add("ts", shipping);           // Transaction shipping.
            ht.Add("tt", tax);                // Transaction tax.
            ht.Add("cu", currency);           // Currency code.

            PostData(ht);
        }
        public void EcommerceItem(string id, string name, string price, string quantity, string code, string category, string currency)
        {
            Hashtable ht = baseValues();

            ht.Add("t", "item");              // Item hit type.
            ht.Add("ti", id);                 // transaction ID.            Required.
            ht.Add("in", name);               // Item name.                 Required.
            ht.Add("ip", price);              // Item price.
            ht.Add("iq", quantity);           // Item quantity.
            ht.Add("ic", code);               // Item code / SKU.
            ht.Add("iv", category);           // Item variation / category.
            ht.Add("cu", currency);           // Currency code.

            PostData(ht);
        }

        public void TrackSocial(string action, string network, string target)
        {
            Hashtable ht = baseValues();

            ht.Add("t", "social");                // Social hit type.
            ht.Add("dh", action);                 // Social Action.         Required.
            ht.Add("dp", network);                // Social Network.        Required.
            ht.Add("dt", target);                 // Social Target.         Required.

            PostData(ht);
        }

        public void TrackException(string description, bool fatal)
        {
            Hashtable ht = baseValues();

            ht.Add("t", "exception");             // Exception hit type.
            ht.Add("dh", description);            // Exception description.         Required.
            ht.Add("dp", fatal ? "1" : "0");      // Exception is fatal?            Required.

            PostData(ht);
        }

        private Hashtable baseValues()
        {


            string userAgent = string.Format("CaptureWorkFlow/{0} (Windows NT {1}.{2}) ", AppVersion.Substring(0, AppVersion.LastIndexOf(".")), Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor);
            string screenResolution = string.Format("{0}x{1}", System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
            string userLanguage = Thread.CurrentThread.CurrentCulture.IetfLanguageTag.ToLower();

            Hashtable ht = new Hashtable();
            ht.Add("v", googleVersion);                     // Version.
            ht.Add("tid", googleTrackingID);                // Tracking ID / Web property / Property ID.
            ht.Add("cid", googleClientID);                  // Anonymous Client ID.
            ht.Add("uid", googleClientID);                  // User ID
            ht.Add("ua", userAgent);                        // User-agent 
            ht.Add("sr", screenResolution);                 // Screen Resolution

            ht.Add("an", "CaptureWorkflow");          // Application Name
            ht.Add("aid", "CaptureWorkflow");                 // Application identifier
            ht.Add("av", AppVersion); // Application Version

            ht.Add("ul", userLanguage);                     // User Language

            return ht;
        }

        private bool PostData(Hashtable values)
        {
            if (!Settings.Instance.SendStatistics)
                return true;
            try
            {
                string data = "";
                foreach (var key in values.Keys)
                {
                    if (data != "") data += "&";
                    if (values[key] != null) data += key.ToString() + "=" + HttpUtility.UrlEncode(values[key].ToString());
                }

                using (var client = new WebClient())
                {
                    var result = client.UploadString(googleURL, "POST", data);
                }

            }
            catch (Exception e)
            {
            }
            return true;
        }
    }
}
