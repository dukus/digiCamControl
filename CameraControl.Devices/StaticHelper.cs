using System;
using System.IO;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices
{
    public class StaticHelper : BaseFieldClass
    {
        private static StaticHelper _instance;

        public static StaticHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StaticHelper();
                return _instance;
            }
            set { _instance = value; }
        }

        public StaticHelper()
        {
            SystemMessage = "System message";
        }

        private string _systemMessage;
        public string SystemMessage
        {
            get { return _systemMessage; }
            set
            {
                _systemMessage = value;
                NotifyPropertyChanged("SystemMessage");
            }
        }

        private int _loadingProgress;
        public int LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                NotifyPropertyChanged("LoadingProgress");
            }
        }

        public static bool GetBit(Int32 b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        public static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        public static bool GetBit(long b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }
        /// <summary>
        /// Return serial number component from a pnp id string
        /// </summary>
        /// <param name="pnpstring"></param>
        /// <returns></returns>
        public static string GetSerial(string pnpstring)
        {
            if (pnpstring == null)
                return "";
            string ret = "";
            if (pnpstring.Contains("#"))
            {
                string[] s = pnpstring.Split('#');
                if (s.Length > 2)
                {
                    ret = s[2];
                }
            }
            return ret;
        }

        /// <summary>
        /// Generate a unique file name 
        /// </summary>
        /// <param name="prefix">First part of file name including full path</param>
        /// <param name="counter">The counter.</param>
        /// <param name="sufix">The last part of file name most cases the file extension.</param>
        /// <returns></returns>
        public static string GetUniqueFilename(string prefix, int counter, string sufix)
        {
            string file = prefix + counter + sufix;
            if (File.Exists(file))
            {
                return GetUniqueFilename(prefix, counter + 1, sufix);
            }
            return file;
        }

    }
}
