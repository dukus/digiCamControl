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
using System.IO;
using System.Timers;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices
{
    public class StaticHelper : BaseFieldClass
    {
        private static StaticHelper _instance;

        private Timer _timer = new Timer(60*1000);

        public AsyncObservableCollection<string> Messages { get; set; }

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
            SystemMessage = "";
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
            Messages = new AsyncObservableCollection<string>();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SystemMessage = "";
        }

        private string _systemMessage;

        public string SystemMessage
        {
            get { return _systemMessage; }
            set
            {
                _systemMessage = value;
                NotifyPropertyChanged("SystemMessage");
                if (!string.IsNullOrWhiteSpace(_systemMessage))
                    Messages.Add(DateTime.Now.ToShortTimeString() + " - " + _systemMessage);
                _timer.Stop();
                _timer.Start();
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