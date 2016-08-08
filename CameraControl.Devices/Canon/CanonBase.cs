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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CameraControl.Devices.Classes;
using PortableDeviceLib;
using PortableDeviceLib.Model;

#endregion

namespace CameraControl.Devices.Canon
{
    public class CanonBase : BaseMTPCamera
    {
        public const int CONST_CMD_CANON_EOS_RemoteRelease = 0x910F;
        public const int CONST_CMD_CANON_EOS_BulbStart = 0x9125;
        public const int CONST_CMD_CANON_EOS_BulbEnd = 0x9126;
        public const int CONST_CMD_CANON_EOS_SetEventMode = 0x9115;
        public const int CONST_CMD_CANON_EOS_SetRemoteMode = 0x9114;
        public const int CONST_CMD_CANON_EOS_GetEvent = 0x9116;
        public const int CONST_CMD_CANON_EOS_DoAf = 0x9154;
        public const int CONST_CMD_CANON_EOS_GetViewFinderData = 0x9153;
        public const int CONST_CMD_CANON_EOS_GetObjectInfo = 0x9103;

        public const int CONST_CMD_CANON_EOS_SetDevicePropValueEx = 0x9110;
        public const int CONST_CMD_CANON_EOS_RequestDevicePropValue = 0x9109;

        public const int CONST_PROP_EOS_ShutterSpeed = 0xD102;
        public const int CONST_PROP_EOS_LiveView = 0xD1B0;

        public const int CONST_Event_CANON_EOS_PropValueChanged = 0xc189;
        public const int CONST_Event_CANON_EOS_ObjectAddedEx = 0xc181;

        //private bool _eventIsbusy = false;

        protected Dictionary<uint, string> _shutterTable = new Dictionary<uint, string>
                                                               {
                                                                   {0, "30"},
                                                                   {1, "25"},
                                                                   {2, "20"},
                                                                   {3, "15"},
                                                                   {4, "13"},
                                                                   {5, "10"},
                                                                   {6, "8"},
                                                                   {7, "6"},
                                                                   {8, "5"},
                                                                   {9, "4"},
                                                                   {10, "3.2"},
                                                                   {11, "2.5"},
                                                                   {12, "2"},
                                                                   {13, "1.6"},
                                                                   {14, "1.3"},
                                                                   {15, "1"},
                                                                   {16, "0.8"},
                                                                   {17, "0.6"},
                                                                   {18, "0.5"},
                                                                   {19, "0.4"},
                                                                   {20, "0.3"},
                                                                   {21, "1/4"},
                                                                   {22, "1/5"},
                                                                   {23, "1/6"},
                                                                   {24, "1/8"},
                                                                   {25, "1/10"},
                                                                   {26, "1/13"},
                                                                   {27, "1/15"},
                                                                   {28, "1/20"},
                                                                   {29, "1/25"},
                                                                   {30, "1/30"},
                                                                   {31, "1/40"},
                                                                   {32, "1/50"},
                                                                   {33, "1/60"},
                                                                   {34, "1/80"},
                                                                   {35, "1/100"},
                                                                   {36, "1/125"},
                                                                   {37, "1/160"},
                                                                   {38, "1/200"},
                                                                   {39, "1/250"},
                                                                   {40, "1/320"},
                                                                   {41, "1/400"},
                                                                   {42, "1/500"},
                                                                   {43, "1/640"},
                                                                   {44, "1/800"},
                                                                   {45, "1/1000"},
                                                                   {46, "1/1250"},
                                                                   {47, "1/1600"},
                                                                   {48, "1/2000"},
                                                                   {49, "1/2500"},
                                                                   {50, "1/3200"},
                                                                   {51, "1/4000"},
                                                                   {52, "1/5000"},
                                                                   {53, "1/6400"},
                                                                   {54, "1/8000"}
                                                               };

        public CanonBase()
        {
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            try
            {
                lock (Locker)
                {
                    Thread thread = new Thread(getEvent);
                    thread.Start();
                }
            }
            catch (Exception)
            {
            }
            //
        }


        private void getEvent()
        {
            try
            {
                if (_eventIsbusy)
                    return;
                _eventIsbusy = true;
                //DeviceReady();
                MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_CANON_EOS_GetEvent);

                if (response.Data == null)
                {
                    Log.Debug("Get event error :" + response.ErrorCode.ToString("X"));
                    return;
                }
                int eventCount = BitConverter.ToInt16(response.Data, 0);
                Log.Debug("Number of events " + eventCount);
                if (eventCount > 0)
                {
                    Console.WriteLine("Event queue length " + eventCount);
                    for (int i = 0; i < eventCount; i++)
                    {
                        try
                        {
                            uint eventCode = BitConverter.ToUInt16(response.Data, 6*i + 2);
                            ushort eventParam = BitConverter.ToUInt16(response.Data, 6*i + 4);
                            int longeventParam = BitConverter.ToInt32(response.Data, 6*i + 4);
                            switch (eventCode)
                            {
                                case CONST_Event_CANON_EOS_PropValueChanged:
                                    Log.Debug("EOS property changed " + eventParam.ToString("X"));
                                    //ReadDeviceProperties(eventParam);
                                    break;
                                case CONST_Event_CANON_EOS_ObjectAddedEx:
                                case CONST_Event_ObjectAddedInSdram:
                                case CONST_Event_ObjectAdded:
                                    {
                                        try
                                        {
                                            MTPDataResponse objectdata =
                                                ExecuteReadDataEx(CONST_CMD_CANON_EOS_GetObjectInfo,
                                                                  (uint) longeventParam);
                                            string filename = "DSC_0000.JPG";
                                            if (objectdata.Data != null)
                                            {
                                                filename = Encoding.Unicode.GetString(objectdata.Data, 53, 12*2);
                                                if (filename.Contains("\0"))
                                                    filename = filename.Split('\0')[0];
                                            }
                                            else
                                            {
                                                Log.Error("Error getting file name");
                                            }
                                            Log.Debug("File transfer " + filename);
                                            PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                                                                              {
                                                                                  WiaImageItem = null,
                                                                                  EventArgs =
                                                                                      new PortableDeviceEventArgs(new PortableDeviceEventType
                                                                                                                      ()
                                                                                                                      {
                                                                                                                          ObjectHandle
                                                                                                                              =
                                                                                                                              (
                                                                                                                              uint
                                                                                                                              )
                                                                                                                              longeventParam
                                                                                                                      }),
                                                                                  CameraDevice = this,
                                                                                  FileName = filename,
                                                                                  Handle = (uint) longeventParam
                                                                              };
                                            OnPhotoCapture(this, args);
                                        }
                                        catch (Exception exception)
                                        {
                                            Log.Error("Object added error", exception);
                                        }
                                    }
                                    break;
                                    //case CONST_Event_CaptureComplete:
                                    //case CONST_Event_CaptureCompleteRecInSdram:
                                    //    {
                                    //        OnCaptureCompleted(this, new EventArgs());
                                    //    }
                                    //    break;
                                    //case CONST_Event_ObsoleteEvent:
                                    //    break;
                                default:
                                    //Console.WriteLine("Unknown event code " + eventCode.ToString("X"));
                                    Log.Debug("Unknown event code :" + eventCode.ToString("X") + "|" +
                                              longeventParam.ToString("X"));
                                    break;
                            }
                        }
                        catch (Exception exception)
                        {
                            Log.Error("Event queue processing error ", exception);
                        }
                    }
                }
            }
            catch (InvalidComObjectException)
            {
                return;
            }
            catch (Exception)
            {
                //Log.Error("Event exception ", exception);
            }
            _eventIsbusy = false;
            _timer.Start();
        }

        public override bool Init(DeviceDescriptor deviceDescriptor)
        {
            StillImageDevice = deviceDescriptor.StillImageDevice;
            //StillImageDevice.ConnectToDevice(AppName, AppMajorVersionNumber, AppMinorVersionNumber);
            //StillImageDevice.DeviceEvent += _stillImageDevice_DeviceEvent;
            DeviceName = StillImageDevice.Model;
            Manufacturer = StillImageDevice.Manufacturer;
            Capabilities.Add(CapabilityEnum.Bulb);
            Capabilities.Add(CapabilityEnum.LiveView);
            InitShutterSpeed();
            IsConnected = true;
            ExecuteWithNoData(CONST_CMD_CANON_EOS_SetRemoteMode, 1);
            ExecuteWithNoData(CONST_CMD_CANON_EOS_SetEventMode, 1);
            _timer.Start();
            return true;
        }

        private void InitShutterSpeed()
        {
            ShutterSpeed = new PropertyValue<long>();
            ShutterSpeed.Name = "ShutterSpeed";
            ShutterSpeed.ValueChanged += ShutterSpeed_ValueChanged;
            ReInitShutterSpeed();
        }

        private void ReInitShutterSpeed()
        {
            lock (Locker)
            {
                try
                {
                    //byte datasize = 4;
                    ShutterSpeed.Clear();
                    foreach (KeyValuePair<uint, string> keyValuePair in _shutterTable)
                    {
                        ShutterSpeed.AddValues(keyValuePair.Value, keyValuePair.Key);
                    }
                    //byte[] result = StillImageDevice.ExecuteReadData(CONST_CMD_GetDevicePropDesc, CONST_PROP_EOS_ShutterSpeed);
                    //int type = BitConverter.ToInt16(result, 2);
                    //byte formFlag = result[(2 * datasize) + 5];
                    //UInt32 defval = BitConverter.ToUInt32(result, datasize + 5);
                    //for (int i = 0; i < result.Length - ((2 * datasize) + 6 + 2); i += datasize)
                    //{
                    //    UInt32 val = BitConverter.ToUInt32(result, ((2 * datasize) + 6 + 2) + i);
                    //    ShutterSpeed.AddValues(_shutterTable.ContainsKey(val) ? _shutterTable[val] : val.ToString(), val);
                    //}
                    //ShutterSpeed.SetValue(defval);
                }
                catch (Exception ex)
                {
                    Log.Debug("EOS Shutter speed init", ex);
                }
            }
        }

        private void ShutterSpeed_ValueChanged(object sender, string key, long val)
        {
            SetProperty(CONST_CMD_CANON_EOS_SetDevicePropValueEx, BitConverter.GetBytes(val),
                        CONST_PROP_EOS_ShutterSpeed);
            SetEOSProperty(CONST_PROP_EOS_ShutterSpeed, (uint) val);
        }

        public override void CapturePhoto()
        {
            Monitor.Enter(Locker);
            try
            {
                IsBusy = true;
                ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_CANON_EOS_RemoteRelease));
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

        private void _stillImageDevice_DeviceEvent(object sender, PortableDeviceEventArgs e)
        {
            if (e.EventType.EventGuid == PortableDeviceGuids.WPD_EVENT_DEVICE_REMOVED)
            {
                StillImageDevice.Disconnect();
                StillImageDevice.IsConnected = false;
                IsConnected = false;

                OnCameraDisconnected(this, new DisconnectCameraEventArgs {StillImageDevice = StillImageDevice});
            }
            else
            {
                getEvent();
            }
        }

        public override void StartBulbMode()
        {
            ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_CANON_EOS_BulbStart));
        }

        public override void EndBulbMode()
        {
            ErrorCodes.GetException(ExecuteWithNoData(CONST_CMD_CANON_EOS_BulbEnd));
        }

        public override LiveViewData GetLiveViewImage()
        {
            _timer.Stop();
            LiveViewData viewData = new LiveViewData();
            if (Monitor.TryEnter(Locker, 10))
            {
                try
                {
                    //DeviceReady();
                    viewData.HaveFocusData = false;
                    MTPDataResponse response = ExecuteReadDataEx(CONST_CMD_CANON_EOS_GetViewFinderData, 0x00100000);
                    ErrorCodes.GetException(response.ErrorCode);
                    if (response.Data == null)
                    {
                        _timer.Start();
                        return null;
                    }
                    viewData.ImageDataPosition = 0;
                    viewData.ImageData = response.Data;
                }
                catch (Exception e)
                {
                    Log.Error("Error get live view image ", e);
                }
                finally
                {
                    Monitor.Exit(Locker);
                }
            }
            _timer.Start();
            return viewData;
        }

        public override void StartLiveView()
        {
            SetProperty(CONST_CMD_CANON_EOS_SetDevicePropValueEx, BitConverter.GetBytes(2),
                        CONST_PROP_EOS_LiveView);
            SetEOSProperty(CONST_PROP_EOS_LiveView, (uint) 2);
        }

        public override void StopLiveView()
        {
            SetProperty(CONST_CMD_CANON_EOS_SetDevicePropValueEx, BitConverter.GetBytes(0),
                        CONST_PROP_EOS_LiveView);
            SetEOSProperty(CONST_PROP_EOS_LiveView, (uint) 0);
        }


        public void SetEOSProperty(uint prop, uint val)
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
                    uint resp = ExecuteWithNoData(CONST_CMD_CANON_EOS_SetDevicePropValueEx, 0x0000000C, (int) prop,
                                                  (int) val);

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
                    Log.Debug("Error EOS set property :" + prop.ToString("X"), exception);
                }
            } while (retry);
            if (timerstate)
                _timer.Start();
        }

        public override string ToString()
        {
            /* Canon Base, just note it for now */
            return base.ToString() + "\n\tType..................Canon (base)";
        }
    }
}