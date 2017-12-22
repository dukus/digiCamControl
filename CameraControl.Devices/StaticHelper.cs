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
using PortableDeviceLib;

#endregion

namespace CameraControl.Devices
{
    public class StaticHelper : BaseFieldClass
    {
        private static StaticHelper _instance;

        private Timer _timer = new Timer(5*1000);

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

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                NotifyPropertyChanged("IsActive");
            }
        }

        private string _systemMessage;

        public string SystemMessage
        {
            get { return _systemMessage; }
            set
            {
                _systemMessage = value;
                IsActive = !string.IsNullOrWhiteSpace(_systemMessage);
                NotifyPropertyChanged("SystemMessage");
                if (!String.IsNullOrWhiteSpace(_systemMessage))
                    Messages.Add(DateTime.Now.ToShortTimeString() + " - " + _systemMessage);
                _timer.Stop();
                _timer.Start();
            }
        }



        private int _loadingProgress;
        private bool _isActive;

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

        public static int GetDataLength(uint dataType)
        {
            int dataLength = 0;
            switch (dataType)
            {
                    //0x0001	INT8	Signed 8-bit integer
                case 0x0001:
                    dataLength = 1;
                    break;
                    //0x0002	UINT8	Unsigned 8-bit integer
                case 0x0002:
                    dataLength = 1;
                    break;
                    //0x0003	INT16	Signed 16-bit integer
                case 0x0003:
                    dataLength = 2;
                    break;
                    //0x0004	UINT16	Unsigned 16-bit integer
                case 0x0004:
                    dataLength = 2;
                    break;
                    //0x0005	INT32	Signed 32-bit integer
                case 0x0005:
                    dataLength = 4;
                    break;
                    //0x0006	UINT32	Unsigned 32-bit integer
                case 0x0006:
                    dataLength = 4;
                    break;
                    //0x0007	INT64	Signed 64-bit integer
                case 0x0007:
                    dataLength = 8;
                    break;
                    //0x0008	UINT64	Unsigned 64-bit integer
                case 0x0008:
                    dataLength = 8;
                    break;
                    //0x0009	INT128	Signed 128-bit integer
                case 0x0009:
                    dataLength = 16;
                    break;
                    //0x000A	UINT128	Unsigned 128-bit integer
                case 0x000A:
                    dataLength = 16;
                    break;
                    //0x4001	AINT8	Signed 8-bit integer array
                case 0x4001:
                    dataLength = 1;
                    break;
                    //0x4002	AUINT8	Unsigned 8-bit integer array
                case 0x4002:
                    dataLength = 1;
                    break;
                    //0x4003	AINT16	Signed 16-bit integer array
                case 0x4003:
                    dataLength = 2;
                    break;
                    //0x4004	AUINT16	Unsigned 16-bit integer array
                case 0x4004:
                    dataLength = 2;
                    break;
                    //0x4005	AINT32	Signed 32-bit integer array
                case 0x4005:
                    dataLength = 4;
                    break;
                    //0x4006	AUINT32	Unsigned 32-bit integer array
                case 0x4006:
                    dataLength = 4;
                    break;
                    //0x4007	AINT64	Signed 64-bit integer array
                case 0x4007:
                    dataLength = 8;
                    break;
                    //0x4008	AUINT64	Unsigned 64-bit integer array
                case 0x4008:
                    dataLength = 8;
                    break;
                    //0x4009	AINT128	Signed 128-bit integer array
                case 0x4009:
                    dataLength = 16;
                    break;
                    //0x400A	AUINT128	Unsigned 128-bit integer array
                case 0x400A:
                    dataLength = 16;
                    break;
                    //0xFFFF	STR	Variable length Unicode character string
                case 0xFFFF:
                    dataLength = -1;
                    break;
            }
            return dataLength;
        }

        public static long GetValue(MTPDataResponse result, int index, int dataLength)
        {
            long val = 0;
            switch (dataLength)
            {
                case 1:
                    val = result.Data[index];
                    break;
                case 2:
                    val = BitConverter.ToUInt16(result.Data, index);
                    break;
                case 4:
                    val = BitConverter.ToUInt32(result.Data, index);
                    break;
                default:
                    val = (long) BitConverter.ToUInt64(result.Data, index);
                    break;
            }
            return val;
        }
    }
}