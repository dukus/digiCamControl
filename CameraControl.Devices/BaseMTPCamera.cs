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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CameraControl.Devices.Classes;
using CameraControl.Devices.TransferProtocol;
using CameraControl.Devices.Xml;
using PortableDeviceLib;
using Timer = System.Timers.Timer;

#endregion

namespace CameraControl.Devices
{
    public class DeviceEventArgs : EventArgs
    {
        public uint Code { get; set; }
    }

    public class BaseMTPCamera : BaseCameraDevice
    {
        /// <summary>
        /// The number of times to retry failed transfers.
        /// </summary>
        public const int TransferRetries = 10;

        public delegate void DeviceEventHandler(object sender, DeviceEventArgs eventArgs);

        public event DeviceEventHandler DeviceEvent;

        public void OnDeviceEvent(DeviceEventArgs eventargs)
        {
            DeviceEventHandler handler = DeviceEvent;
            if (handler != null) handler(this, eventargs);
        }

        protected const string AppName = "CameraControl";
        protected const int AppMajorVersionNumber = 1;
        protected const int AppMinorVersionNumber = 0;

        // common MTP commands
        public const uint CONST_CMD_GetDevicePropValue = 0x1015;
        public const uint CONST_CMD_SetDevicePropValue = 0x1016;
        public const uint CONST_CMD_GetDevicePropDesc = 0x1014;
        public const uint CONST_CMD_GetObject = 0x1009;
        public const uint CONST_CMD_GetObjectHandles = 0x1007;
        public const uint CONST_CMD_GetObjectInfo = 0x1008;
        public const uint CONST_CMD_GetThumb = 0x100A;
        public const uint CONST_CMD_DeleteObject = 0x100B;
        public const uint CONST_CMD_InitiateCapture = 0x100E;
        public const uint CONST_CMD_FormatStore = 0x100F;
        public const uint CONST_CMD_GetStorageIDs = 0x1004;

        public const uint CONST_Event_ObjectAdded = 0x4002;
        public const uint CONST_Event_ObjectAddedInSdram = 0xC101;

        public const uint CONST_PROP_BatteryLevel = 0x5001;

        private const int CONST_READY_TIME = 1;
        private const int CONST_LOOP_TIME = 100;

        protected ITransferProtocol StillImageDevice = null;
        protected bool DeviceIsBusy = false;

        /// <summary>
        /// The timer for get periodically the event list
        /// </summary>
        protected Timer _timer = new Timer(1000 / 10);

        /// <summary>
        /// Variable to check if event processing is in progress 
        /// </summary>
        protected bool _eventIsbusy = false;

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            StillImageDevice = deviceDescriptor.StillImageDevice;
            StillImageDevice imageDevice = StillImageDevice as StillImageDevice;
            if (imageDevice != null)
                imageDevice.DeviceEvent += StillImageDevice_DeviceEvent;
            IsConnected = true;
            DeviceName = StillImageDevice.Model;
            Manufacturer = StillImageDevice.Manufacturer;
            var data = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropValue,
                                                                               CONST_PROP_BatteryLevel);
            if (data.Data != null && data.Data.Length > 0)
                Battery = data.Data[0];
            return true;
        }

        public override void CapturePhoto()
        {
            Monitor.Enter(Locker);
            try
            {
                IsBusy = true;
                ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_InitiateCapture));
            }
            catch (COMException comException)
            {
                IsBusy = false;
                ErrorCodes.GetException(comException);
            }
            catch
            {
                IsBusy = false;
                throw;
            }
            finally
            {
                Monitor.Exit(Locker);
            }
        }

        public void StopEventTimer()
        {
            while (_eventIsbusy)
            {
            }
            _timer.Stop();
        }

        public void StartEventTimer()
        {
            _timer.Start();
        }

        private void StillImageDevice_DeviceEvent(object sender, PortableDeviceEventArgs e)
        {
            if (e.EventType.EventGuid == PortableDeviceGuids.WPD_EVENT_DEVICE_REMOVED)
            {
                _timer.Stop();
                StillImageDevice.Disconnect();
                StillImageDevice.IsConnected = false;
                IsConnected = false;
                OnCameraDisconnected(this, new DisconnectCameraEventArgs { StillImageDevice = StillImageDevice });
            }
        }

        public override bool DeleteObject(DeviceObject deviceObject)
        {
            uint res = ExecuteWithNoData(CONST_CMD_DeleteObject, (uint)deviceObject.Handle);
            return res == 0 || res == ErrorCodes.MTP_OK;
        }

        public override AsyncObservableCollection<DeviceObject> GetObjects(object storageId, bool loadThumbs)
        {
            AsyncObservableCollection<DeviceObject> res = new AsyncObservableCollection<DeviceObject>();
            MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetObjectHandles, 0xFFFFFFFF);
            if (response.Data == null)
            {
                Log.Debug("Get object error :" + response.ErrorCode.ToString("X"));
                ErrorCodes.GetException(response.ErrorCode);
                return res;
            }
            int objCount = BitConverter.ToInt32(response.Data, 0);
            for (int i = 0; i < objCount; i++)
            {
                DeviceObject deviceObject = new DeviceObject();
                uint handle = BitConverter.ToUInt32(response.Data, 4 * i + 4);
                deviceObject.Handle = handle;
                MTPDataResponse objectdata = ExecuteReadDataEx(CONST_CMD_GetObjectInfo, handle);
                if (objectdata.Data != null)
                {
                    uint objFormat = BitConverter.ToUInt16(objectdata.Data, 4);
                    if (objFormat == 0x3000 || objFormat == 0x3801 || objFormat == 0x3800)
                    {
                        deviceObject.FileName = Encoding.Unicode.GetString(objectdata.Data, 53, 12 * 2);
                        if (deviceObject.FileName.Contains("\0"))
                            deviceObject.FileName = deviceObject.FileName.Split('\0')[0];
                        try
                        {
                            string datesrt = Encoding.Unicode.GetString(objectdata.Data, 53 + (12 * 2) + 3, 30);
                            //datesrt = datesrt.Replace("T", "");
                            DateTime date = DateTime.MinValue;
                            if (DateTime.TryParseExact(datesrt, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture,
                                                       DateTimeStyles.None, out date))
                            {
                                deviceObject.FileDate = date;
                            }
                        }
                        catch (Exception)
                        {
                        }

                        if (loadThumbs)
                        {
                            MTPDataResponse thumbdata = ExecuteReadDataEx(CONST_CMD_GetThumb, handle);
                            deviceObject.ThumbData = thumbdata.Data;
                        }
                        res.Add(deviceObject);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Transfers the file and saves it to the specified filename.
        /// </summary>
        /// <param name="o">The object handle.</param>
        /// <param name="filename">The filename.</param>
        public override void TransferFile(object o, string filename)
        {
            using (var fs = File.Open(filename, FileMode.Create))
            {
                TransferFile(o, fs);
            }
        }

        /// <summary>
        /// Transfers the file and writes it to the specified stream.
        /// </summary>
        /// <param name="o">The object handle.</param>
        /// <param name="stream">The stream.</param>
        public override void TransferFile(object o, Stream stream)
        {
            // Sanity checks.
            if (!stream.CanWrite)
                throw new ArgumentException("Specified stream is not writable.", "stream");
            lock (Locker)
            {

                string s = o as string;
                if (s != null)
                {
                    ((StillImageDevice)StillImageDevice).SaveFile(s, stream);
                    return;
                }
                int retryes = TransferRetries;
                _timer.Stop();
                MTPDataResponse result = new MTPDataResponse();
                //=================== managed file write
                do
                {
                    try
                    {
                        //using (MemoryStream mStream = new MemoryStream())
                        //{

                            result = StillImageDevice.ExecuteReadBigData(CONST_CMD_GetObject, stream,
                                (total, current) =>
                                {
                                    double i = (double) current / total;
                                    TransferProgress =
                                        Convert.ToUInt32(i * 100);
                                }, Convert.ToUInt32(o));
                            //if (result != null && result.Data != null)
                            //{
                            //    stream.Write(result.Data, 0, result.Data.Length);
                            //}
                            //stream.Write(mStream.ToArray(), 0, (int)mStream.Length);
                            break;
                        //}
                    }
                    catch (COMException ex)
                    {
                        Log.Error("Transfer error", ex);
                        Thread.Sleep(200);
                        retryes--;
                    }

                } while (retryes > 0);

                //==================================================================
                //=================== direct file write
                //StillImageDevice.ExecuteReadBigDataWriteToFile(CONST_CMD_GetObject,
                //                                                     Convert.ToInt32(o), -1,
                //                                                     (total, current) =>
                //                                                     {
                //                                                       double i = (double)current / total;
                //                                                       TransferProgress =
                //                                                         Convert.ToUInt32(i * 100);

                //                                                     }, filename);

                //==================================================================
                _timer.Start();
                TransferProgress = 0;
            }
        }

        public override void FormatStorage(object storageId)
        {
            MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_GetStorageIDs);
            if (response.Data.Length > 4)
            {
                int objCount = BitConverter.ToInt32(response.Data, 0);
                for (int i = 0; i < objCount; i++)
                {
                    uint handle = BitConverter.ToUInt32(response.Data, 4 * i + 4);
                    ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_FormatStore, handle));
                }
            }
        }

        public MTPDataResponse ExecuteReadDataEx(uint code, uint param1, uint param2)
        {
            return ExecuteReadDataEx(code, param1, param2, CONST_LOOP_TIME, 0);
        }

        public uint ExecuteWithNoData(uint code, uint param1)
        {
            return ExecuteWithNoData(code, param1, CONST_LOOP_TIME, 0);
        }

        public uint ExecuteWithNoData(uint code, uint param1, uint param2, uint param3)
        {
            return ExecuteWithNoData(code, param1, param2, param3, CONST_LOOP_TIME, 0);
        }

        public uint ExecuteWithNoData(uint code)
        {
            return ExecuteWithNoData(code, CONST_LOOP_TIME, 0);
        }


        public uint ExecuteWithNoData(uint code, uint param1, int loop, int counter)
        {
            uint res = 0;
            bool allok;
            do
            {
                allok = true;
                res = StillImageDevice.ExecuteWithNoData(code, param1);
                if ((res == ErrorCodes.MTP_Device_Busy || res == PortableDeviceErrorCodes.ERROR_BUSY) && counter < loop)
                {
                    Thread.Sleep(CONST_READY_TIME);
                    counter++;
                    allok = false;
                }
            } while (!allok);
            return res;
        }

        public uint ExecuteWithNoData(uint code, uint param1, uint param2, uint param3, int loop, int counter)
        {
            uint res = 0;
            bool allok;
            do
            {
                allok = true;
                res = StillImageDevice.ExecuteWithNoData(code, param1, param2, param3);
                if ((res == ErrorCodes.MTP_Device_Busy || res == PortableDeviceErrorCodes.ERROR_BUSY) && counter < loop)
                {
                    Thread.Sleep(CONST_READY_TIME);
                    counter++;
                    allok = false;
                }
            } while (!allok);
            return res;
        }

        public uint ExecuteWithNoData(uint code, int loop, int counter)
        {
            uint res = 0;
            bool allok;
            do
            {
                allok = true;
                res = StillImageDevice.ExecuteWithNoData(code);
                if ((res == ErrorCodes.MTP_Device_Busy || res == PortableDeviceErrorCodes.ERROR_BUSY) && counter < loop)
                {
                    Thread.Sleep(CONST_READY_TIME);
                    counter++;
                    allok = false;
                }
            } while (!allok);
            return res;
        }

        public uint ExecuteWithNoData(uint code, uint param1, uint param2)
        {
            uint res = StillImageDevice.ExecuteWithNoData(code, param1, param2);
            return res;
        }

        public MTPDataResponse ExecuteReadDataEx(uint code, uint param1, uint param2, int loop, int counter)
        {
            DeviceIsBusy = true;
            MTPDataResponse res = new MTPDataResponse();
            bool allok;
            do
            {
                allok = true;
                res = StillImageDevice.ExecuteReadData(code, param1, param2);
                if ((res.ErrorCode == ErrorCodes.MTP_Device_Busy || res.ErrorCode == PortableDeviceErrorCodes.ERROR_BUSY) &&
                    counter < loop)
                {
                    Thread.Sleep(CONST_READY_TIME);
                    counter++;
                    allok = false;
                }
            } while (!allok);
            DeviceIsBusy = false;
            return res;
        }

        public MTPDataResponse ExecuteReadDataEx(uint code, uint param1, uint param2, uint param3, int loop, int counter)
        {
            DeviceIsBusy = true;
            MTPDataResponse res = new MTPDataResponse();
            bool allok;
            do
            {
                allok = true;
                res = StillImageDevice.ExecuteReadData(code, param1, param2, param3);
                if ((res.ErrorCode == ErrorCodes.MTP_Device_Busy || res.ErrorCode == PortableDeviceErrorCodes.ERROR_BUSY) &&
                    counter < loop)
                {
                    Thread.Sleep(CONST_READY_TIME);
                    counter++;
                    allok = false;
                }
            } while (!allok);
            DeviceIsBusy = false;
            return res;
        }

        public MTPDataResponse ExecuteReadDataEx(uint code, uint param1)
        {
            int counter = 0;
            DeviceIsBusy = true;
            MTPDataResponse res = new MTPDataResponse();
            bool allok;
            do
            {
                res = StillImageDevice.ExecuteReadData(code, param1);
                allok = true;
                if ((res.ErrorCode == ErrorCodes.MTP_Device_Busy || res.ErrorCode == PortableDeviceErrorCodes.ERROR_BUSY) &&
                    counter < CONST_LOOP_TIME)
                {
                    Thread.Sleep(CONST_READY_TIME);
                    counter++;
                    allok = false;
                }
            } while (!allok);
            DeviceIsBusy = false;
            return res;
        }

        public MTPDataResponse ExecuteReadDataEx(uint code)
        {
            int counter = 0;
            DeviceIsBusy = true;
            MTPDataResponse res = new MTPDataResponse();
            bool allok;
            do
            {
                res = StillImageDevice.ExecuteReadData(code);
                allok = true;
                if ((res.ErrorCode == ErrorCodes.MTP_Device_Busy || res.ErrorCode == PortableDeviceErrorCodes.ERROR_BUSY) &&
                    counter < CONST_LOOP_TIME)
                {
                    Thread.Sleep(CONST_READY_TIME);
                    counter++;
                    allok = false;
                }
            } while (!allok);
            DeviceIsBusy = false;
            return res;
        }


        public void SetProperty(uint code, byte[] data, uint param1)
        {
            bool timerstate = _timer.Enabled;
            _timer.Stop();
            bool retry = false;
            int retrynum = 0;
            //DeviceReady();
            do
            {
                if (retrynum > 5)
                {
                    return;
                }
                try
                {
                    retry = false;
                    uint resp = StillImageDevice.ExecuteWriteData(code, data, param1);
                    if (resp != 0 || resp != ErrorCodes.MTP_OK)
                    {
                        //Console.WriteLine("Retry ...." + resp.ToString("X"));
                        if (resp == ErrorCodes.MTP_Device_Busy || resp == 0x800700AA)
                        {
                            Thread.Sleep(50);
                            retry = true;
                            retrynum++;
                        }
                        else
                        {
                            ErrorCodes.GetException(resp);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Debug("Error set property :" + param1.ToString("X"), exception);
                }
            } while (retry);
            if (timerstate)
                _timer.Start();
        }

        public void SetProperty(uint code, byte[] data, uint param1, uint param2)
        {
            bool timerstate = _timer.Enabled;
            _timer.Stop();
            bool retry = false;
            int retrynum = 0;
            //DeviceReady();
            do
            {
                if (retrynum > 5)
                {
                    return;
                }
                try
                {
                    retry = false;
                    uint resp = StillImageDevice.ExecuteWriteData(code, data, param1, param2);
                    if (resp != 0 || resp != ErrorCodes.MTP_OK)
                    {
                        //Console.WriteLine("Retry ...." + resp.ToString("X"));
                        if (resp == ErrorCodes.MTP_Device_Busy || resp == 0x800700AA)
                        {
                            Thread.Sleep(50);
                            retry = true;
                            retrynum++;
                        }
                        else
                        {
                            ErrorCodes.GetException(resp);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Debug("Error set property :" + param1.ToString("X"), exception);
                }
            } while (retry);
            if (timerstate)
                _timer.Start();
        }

        public void SetProperty(uint code, byte[] data, uint param1, uint param2, uint param3)
        {
            bool timerstate = _timer.Enabled;
            _timer.Stop();
            bool retry = false;
            int retrynum = 0;
            //DeviceReady();
            do
            {
                if (retrynum > 5)
                {
                    return;
                }
                try
                {
                    retry = false;
                    uint resp = StillImageDevice.ExecuteWriteData(code, data, param1, param2, param3);
                    if (resp != 0 || resp != ErrorCodes.MTP_OK)
                    {
                        //Console.WriteLine("Retry ...." + resp.ToString("X"));
                        if (resp == ErrorCodes.MTP_Device_Busy || resp == 0x800700AA)
                        {
                            Thread.Sleep(100);
                            retry = true;
                            retrynum++;
                        }
                        else
                        {
                            ErrorCodes.GetException(resp);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Debug("Error set property :" + param1.ToString("X"), exception);
                }
            } while (retry);
            if (timerstate)
                _timer.Start();
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            int i = (short)(value[startIndex] << 8 | value[startIndex + 1]);
            return (short)(i);
            //return System.BitConverter.ToInt16(value.Reverse().ToArray(), value.Length - sizeof(Int16) - startIndex);
        }

        public static UInt16 ToUInt16(byte[] value, int startIndex)
        {
            uint i = (uint)(value[startIndex] << 8 | value[startIndex + 1]);
            return (UInt16)(i);
            //return System.BitConverter.ToInt16(value.Reverse().ToArray(), value.Length - sizeof(Int16) - startIndex);
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            int i = (value[startIndex] << 24 | value[startIndex + 1] << 16 | value[startIndex + 2] << 8 | value[startIndex + 3]);
            return i;
            //return System.BitConverter.ToInt16(value.Reverse().ToArray(), value.Length - sizeof(Int16) - startIndex);
        }

        public decimal ToDeciaml(byte[] value, int startIndex)
        {
            int i = ToUInt16(value, startIndex);
            int d = ToUInt16(value, startIndex + 2);
            if (d == 0 && i == 0)
                return 0;
            string s = i + "." + d;
            return Convert.ToDecimal(s, new CultureInfo("en-US"));
        }


        protected XmlDeviceData LoadDeviceData(MTPDataResponse res)
        {
            XmlDeviceData deviceInfo = new XmlDeviceData();
            ErrorCodes.GetException(res.ErrorCode);
            deviceInfo.Manufacturer = Manufacturer;
            int index = 2 + 4 + 2;
            int vendorDescCount = res.Data[index];
            index += vendorDescCount * 2;
            index += 3;
            int comandsCount = res.Data[index];
            index += 2;
            // load commands
            for (int i = 0; i < comandsCount; i++)
            {
                index += 2;
                deviceInfo.AvaiableCommands.Add(new XmlCommandDescriptor() { Code = BitConverter.ToUInt16(res.Data, index) });
            }
            index += 2;
            int eventcount = res.Data[index];
            index += 2;
            // load events
            for (int i = 0; i < eventcount; i++)
            {
                index += 2;
                deviceInfo.AvaiableEvents.Add(new XmlEventDescriptor() { Code = BitConverter.ToUInt16(res.Data, index) });
            }
            index += 2;
            int propertycount = res.Data[index];
            index += 2;
            // load properties codes
            for (int i = 0; i < propertycount; i++)
            {
                index += 2;
                deviceInfo.AvaiableProperties.Add(new XmlPropertyDescriptor() { Code = BitConverter.ToUInt16(res.Data, index) });
            }
            try
            {
                MTPDataResponse vendor_res = ExecuteReadDataEx(0x90CA);
                if (vendor_res.Data.Length > 0)
                {
                    index = 0;
                    propertycount = vendor_res.Data[index];
                    index += 2;
                    for (int i = 0; i < propertycount; i++)
                    {
                        index += 2;
                        deviceInfo.AvaiableProperties.Add(new XmlPropertyDescriptor() { Code = BitConverter.ToUInt16(vendor_res.Data, index) });
                    }
                }
            }
            catch (Exception)
            {
            }
            return deviceInfo;
        }
    }
}