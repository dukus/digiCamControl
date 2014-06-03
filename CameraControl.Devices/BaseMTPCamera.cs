using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CameraControl.Devices.Classes;
using PortableDeviceLib;
using Timer = System.Timers.Timer;

namespace CameraControl.Devices
{
    public class DeviceEventArgs : EventArgs
    {
        public uint Code { get; set; }
    }

    public class BaseMTPCamera : BaseCameraDevice
    {
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
        public const int CONST_CMD_GetDevicePropValue = 0x1015;
        public const int CONST_CMD_SetDevicePropValue = 0x1016;
        public const int CONST_CMD_GetDevicePropDesc = 0x1014;
        public const int CONST_CMD_GetObject = 0x1009;
        public const int CONST_CMD_GetObjectHandles = 0x1007;
        public const int CONST_CMD_GetObjectInfo = 0x1008;
        public const int CONST_CMD_GetThumb = 0x100A;
        public const int CONST_CMD_DeleteObject = 0x100B;
        public const int CONST_CMD_FormatStore = 0x100F;
        public const int CONST_CMD_GetStorageIDs = 0x1004;

        public const int CONST_Event_ObjectAdded = 0x4002;
        public const int CONST_Event_ObjectAddedInSdram = 0xC101;

        private const int CONST_READY_TIME = 1;
        private const int CONST_LOOP_TIME = 100;

        protected StillImageDevice StillImageDevice = null;
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
            StillImageDevice = new StillImageDevice(deviceDescriptor.WpdId);
            StillImageDevice.ConnectToDevice(AppName, AppMajorVersionNumber, AppMinorVersionNumber);
            StillImageDevice.DeviceEvent += StillImageDevice_DeviceEvent;
            IsConnected = true;
            return true;
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

        void StillImageDevice_DeviceEvent(object sender, PortableDeviceEventArgs e)
        {
            if (e.EventType.EventGuid == PortableDeviceGuids.WPD_EVENT_DEVICE_REMOVED)
            {
                _timer.Stop();
                StillImageDevice.Disconnect();
                StillImageDevice.IsConnected = false;
                IsConnected = false;
                OnCameraDisconnected(this, new DisconnectCameraEventArgs { StillImageDevice = StillImageDevice });
            }
            else
            {
                //Thread thread = new Thread(getEvent);
                //thread.Start();
            }
        }

        public override bool DeleteObject(DeviceObject deviceObject)
        {
            uint res = ExecuteWithNoData(CONST_CMD_DeleteObject, (uint)deviceObject.Handle);
            return res == 0 || res == ErrorCodes.MTP_OK;
        }

        public override AsyncObservableCollection<DeviceObject> GetObjects(object storageId)
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
                            if (DateTime.TryParseExact(datesrt, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                            {
                                deviceObject.FileDate = date;
                            }
                        }
                        catch (Exception)
                        {
                        }

                        MTPDataResponse thumbdata = ExecuteReadDataEx(CONST_CMD_GetThumb, handle);
                        deviceObject.ThumbData = thumbdata.Data;
                        res.Add(deviceObject);
                    }
                }
            }
            return res;
        }

        public override void TransferFile(object o, string filename)
        {
            int retryes = 10;
            lock (Locker)
            {
                _timer.Stop();
                MTPDataResponse result = new MTPDataResponse();
                //=================== managed file write
                do
                {
                    try
                    {
                        result = StillImageDevice.ExecuteReadBigData(CONST_CMD_GetObject,
                                                                     Convert.ToInt32(o), -1,
                                                                     (total, current) =>
                                                                     {
                                                                         double i = (double)current / total;
                                                                         TransferProgress =
                                                                           Convert.ToUInt32(i * 100);

                                                                     });

                    }
                    // if not enough memory for transfer catch it and wait and try again
                    catch (OutOfMemoryException)
                    {

                    }
                    catch(COMException)
                    {
                        
                    }
                    if (result != null && result.Data != null)
                    {
                        using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
                        {
                            writer.Write(result.Data);
                        }
                    }
                    else
                    {
                        Log.Error("Transfer error code retrying " + result.ErrorCode.ToString("X"));
                        Thread.Sleep(200);
                        retryes--;
                    }
                } while (result.Data == null && retryes>0);
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

        public MTPDataResponse ExecuteReadDataEx(int code)
        {
            return ExecuteReadDataEx(code, -1, -1, CONST_LOOP_TIME, 0);
        }

        public MTPDataResponse ExecuteReadDataEx(int code, int param1, int param2)
        {
            return ExecuteReadDataEx(code, param1, param2, CONST_LOOP_TIME, 0);
        }

        public uint ExecuteWithNoData(int code, uint param1)
        {
            return ExecuteWithNoData(code, param1, CONST_LOOP_TIME, 0);
        }

        public uint ExecuteWithNoData(int code, uint param1, uint param2, uint param3)
        {
            return ExecuteWithNoData(code, param1, param2, param3, CONST_LOOP_TIME, 0);
        }

        public uint ExecuteWithNoData(int code)
        {
            return ExecuteWithNoData(code, CONST_LOOP_TIME, 0);
        }


        public uint ExecuteWithNoData(int code, uint param1, int loop, int counter)
        {
            WaitForReady();
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

        public uint ExecuteWithNoData(int code, uint param1, uint param2, uint param3, int loop, int counter)
        {
            WaitForReady();
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

        public uint ExecuteWithNoData(int code, int loop, int counter)
        {
            WaitForReady();
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

        public uint ExecuteWithNoData(int code, uint param1, uint param2)
        {
            uint res = StillImageDevice.ExecuteWithNoData(code, param1, param2);
            return res;
        }

        public MTPDataResponse ExecuteReadDataEx(int code, int param1, int param2, int loop, int counter)
        {
            WaitForReady();
            DeviceIsBusy = true;
            MTPDataResponse res = new MTPDataResponse();
            bool allok;
            do
            {
                allok = true;
                res = StillImageDevice.ExecuteReadDataEx(code, param1, param2);
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

        public MTPDataResponse ExecuteReadDataEx(int code, uint param1, uint param2, uint param3, int loop, int counter)
        {
            WaitForReady();
            DeviceIsBusy = true;
            MTPDataResponse res = new MTPDataResponse();
            bool allok;
            do
            {
                allok = true;
                res = StillImageDevice.ExecuteReadDataEx(code, param1, param2, param3);
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

        public MTPDataResponse ExecuteReadDataEx(int code, uint param1)
        {
            int counter = 0;
            WaitForReady();
            DeviceIsBusy = true;
            MTPDataResponse res = new MTPDataResponse();
            bool allok;
            do
            {
                res = StillImageDevice.ExecuteReadDataEx(code, param1);
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


        protected void WaitForReady()
        {
            //while (DeviceIsBusy)
            //{
            //  Thread.Sleep(1);
            //}
        }

        public void SetProperty(int code, byte[] data, int param1, int param2)
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

        public void SetProperty(int code, byte[] data, uint param1, uint param2, uint param3)
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
    }

}
