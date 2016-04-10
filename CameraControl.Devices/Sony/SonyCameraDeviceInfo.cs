using System.Collections.Generic;

namespace Kazyx.DeviceDiscovery
{
    public class SonyCameraDeviceInfo
    {
        internal SonyCameraDeviceInfo(string udn, string mname, string fname, Dictionary<string, string> ep)
        {
            UDN = udn;
            ModelName = mname;
            FriendlyName = fname;
            Endpoints = ep;
        }

        private Dictionary<string, string> _Endpoints;

        /// <summary>
        /// K-V pairs of service name and its endpoint URL
        /// </summary>
        public Dictionary<string, string> Endpoints
        {
            private set { _Endpoints = value; }
            get { return new Dictionary<string, string>(_Endpoints); }
        }

        public string FriendlyName { private set; get; }

        public string ModelName { private set; get; }

        public string UDN { private set; get; }
    }
}
