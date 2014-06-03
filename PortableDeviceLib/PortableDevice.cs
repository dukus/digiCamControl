#region License
/*
PortableDevice.cs
Copyright (C) 2009 Vincent Lainé
 
This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using PortableDeviceApiLib;
using System.Runtime.InteropServices;
using PortableDeviceLib.Model;
using System.ComponentModel;

namespace PortableDeviceLib
{
    /// <summary>
    /// Represent a portable device
    /// </summary>
    public class PortableDevice : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Event sended when portable device raise an event
        /// </summary>
        public event EventHandler<PortableDeviceEventArgs> DeviceEvent;

        /// <summary>
        /// <see cref="System.ComponentModel.INotifyPropertyChanged"/>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private object dispatcher; //Use an object for thread safety

      protected PortableDeviceApiLib.PortableDeviceClass portableDeviceClass;
        private Dictionary<string, object> values;
        private PortableDeviceCapabilities deviceCapabilities;
        private PortableDeviceFonctionalObject content;

        private string adviseCookie;
        private PortableDeviceEventCallback eventCallback;

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="deviceId"></param>
        internal PortableDevice(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("deviceId");
            this.dispatcher = new object();
            this.portableDeviceClass = new PortableDeviceApiLib.PortableDeviceClass();
            this.values = new Dictionary<string, object>();

            this.DeviceId = deviceId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the device ID
        /// </summary>
        public string DeviceId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value 
        /// </summary>
        public bool IsConnected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Friendly name of the device
        /// </summary>
        public string FriendlyName
        {
            get
            {
                return GetStringProperty(PortableDevicePKeys.WPD_DEVICE_FRIENDLY_NAME);
            }
        }

        /// <summary>
        /// Gets the battery level of the device
        /// </summary>
        public int BatteryLevel
        {
            get
            {
                return GetIntegerProperty(PortableDevicePKeys.WPD_DEVICE_POWER_LEVEL);
            }
        }

        public int FNumber
        {
          get
          {
            return GetIntegerProperty("StillCapture", PortableDevicePKeys.WPD_STILL_IMAGE_EXPOSURE_TIME);
          }
        }

        /// <summary>
        /// Gets the device model
        /// </summary>
        public string Model
        {
            get
            {
                return GetStringProperty(PortableDevicePKeys.WPD_DEVICE_MODEL);
            }
        }

        /// <summary>
        /// Gets the device Manufacturer
        /// </summary>
        public string Manufacturer
        {
          get
          {
            return GetStringProperty(PortableDevicePKeys.WPD_DEVICE_MANUFACTURER);
          }
        }

        /// <summary>
        /// Gets the firmware version
        /// </summary>
        public string FirmwareVersion
        {
            get
            {
                return GetStringProperty(PortableDevicePKeys.WPD_DEVICE_FIRMWARE_VERSION);
            }
        }

        /// <summary>
        /// Gets the serial number of device
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return GetStringProperty(PortableDevicePKeys.WPD_DEVICE_SERIAL_NUMBER);
            }
        }

        /// <summary>
        /// Gets the device type
        /// </summary>
        public string DeviceType
        {
            get
            {
                return this.GetStringProperty(PortableDevicePKeys.WPD_DEVICE_TYPE);
            }
        }

        /// <summary>
        /// Gets the capabilities of the device
        /// </summary>
        public PortableDeviceCapabilities DeviceCapabilities
        {
            get
            {
                return deviceCapabilities;
            }
        }

        /// <summary>
        /// Gets all content from device
        /// If return is null be sure you call <see cref="PortableDevice.RefreshContent()"/> before
        /// </summary>
        public PortableDeviceFonctionalObject Content
        {
            get
            {
                return content;
            }
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Connect to the portable device
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="majorVersionNumber"></param>
        /// <param name="minorVersionNumber"></param>
        public void ConnectToDevice(string appName, float majorVersionNumber, float minorVersionNumber)
        {
            if (string.IsNullOrEmpty(appName))
                throw new ArgumentNullException("appName");

          if(IsConnected)
            return;
            //Creating propValues for connection
            IPortableDeviceValues clientValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();

            //Set the application name
            _tagpropertykey prop = PortableDevicePKeys.WPD_CLIENT_NAME;
            clientValues.SetStringValue(ref prop, appName);
            //Set the App version
            prop = PortableDevicePKeys.WPD_CLIENT_MAJOR_VERSION;
            clientValues.SetFloatValue(ref prop, majorVersionNumber);
            //Set the app minor version
            prop = PortableDevicePKeys.WPD_CLIENT_MINOR_VERSION;
            clientValues.SetFloatValue(ref prop, minorVersionNumber);

            //Open connection
            this.portableDeviceClass.Open(this.DeviceId, clientValues);

            //Extract device capabilities
            //this.ExtractDeviceCapabilities();

            this.eventCallback = new PortableDeviceEventCallback(this);
            // According to documentation pParameters should be null (see http://msdn.microsoft.com/en-us/library/dd375684%28v=VS.85%29.aspx )
            this.portableDeviceClass.Advise(0, this.eventCallback, null, out this.adviseCookie);

            IsConnected = true;
        }

        /// <summary>
        /// Disconnect from device
        /// </summary>
        public void Disconnect()
        {
            if (!this.IsConnected)
                return;

            this.portableDeviceClass.Unadvise(this.adviseCookie);
            this.eventCallback = null;
            this.IsConnected = false;
        }

        /// <summary>
        /// Refresh content from device
        /// </summary>
        public void RefreshContent()
        {
            this.StartEnumerate();
        }

        /// <summary>
        /// Execute the specified command
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommand(_tagpropertykey command)
        {
            IPortableDeviceValues commandValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
            IPortableDeviceValues results;

            commandValues.SetGuidValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, ref command.fmtid);
            commandValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID, command.pid);

            // According to documentation, first parameter should be 0 (see http://msdn.microsoft.com/en-us/library/dd375691%28v=VS.85%29.aspx)
            this.portableDeviceClass.SendCommand(0, commandValues, out results);

        }

        //view-source:http://www.experts-exchange.com/Programming/Languages/C_Sharp/Q_26860397.html

        //public void StartLiveView()
        //{
        //  IPortableDeviceValues commandValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
        //  IPortableDevicePropVariantCollection propVariant =
        //    (IPortableDevicePropVariantCollection)new PortableDeviceTypesLib.PortableDevicePropVariantCollection();
        //  IPortableDeviceValues results;

        //  //commandValues.SetGuidValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, ref command.fmtid);
        //  commandValues.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY,
        //                                   PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITHOUT_DATA_PHASE.fmtid);
        //  commandValues.SetUnsignedIntegerValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID,
        //                         PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITHOUT_DATA_PHASE.pid);

        //  commandValues.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);
        //  commandValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_CODE, 0x9201);

        //  // According to documentation, first parameter should be 0 (see http://msdn.microsoft.com/en-us/library/dd375691%28v=VS.85%29.aspx)
        //  this.portableDeviceClass.SendCommand(0, commandValues, out results);
        //  int pvalue = 0;
        //  results.GetSignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_RESPONSE_CODE, out pvalue);

        //}


       // public byte[] GetLiveView()
       // {
       //   // source: http://msdn.microsoft.com/en-us/library/windows/desktop/ff384843(v=vs.85).aspx
       //   // and view-source:http://www.experts-exchange.com/Programming/Languages/C_Sharp/Q_26860397.html
       //   // error codes http://msdn.microsoft.com/en-us/library/windows/desktop/dd319335(v=vs.85).aspx
       //   byte[] imgdate = new byte[921600];

       //   IPortableDeviceValues commandValues = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();
       //   IPortableDeviceValues pParameters = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValues();

       //   IPortableDevicePropVariantCollection propVariant =
       //     (IPortableDevicePropVariantCollection)new PortableDeviceTypesLib.PortableDevicePropVariantCollection();
       //   IPortableDeviceValues pResults;

       //   //commandValues.SetGuidValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, ref command.fmtid);
       //   commandValues.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY,
       //                                    PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_READ.fmtid);
       //   commandValues.SetUnsignedIntegerValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID,
       //                          PortableDevicePKeys.WPD_COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_READ.pid);
       //   commandValues.SetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ref imgdate[0], (uint)imgdate.Length);

         
       //   commandValues.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);
       //   commandValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_CODE, 0x9203);

       //   // According to documentation, first parameter should be 0 (see http://msdn.microsoft.com/en-us/library/dd375691%28v=VS.85%29.aspx)
       //   this.portableDeviceClass.SendCommand(0, commandValues, out pResults);
          
       //   try
       //   {
       //     int pValue = 0;
       //     pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
       //     if (pValue!=0)
       //     {
       //       return null;
       //     }
       //   }
       //   catch (Exception ex)
       //   {
       //   }
       //   string pwszContext = string.Empty;
       //   pResults.GetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT,out pwszContext);
       //   uint cbReportedDataSize = 0;
       //   pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_TOTAL_DATA_SIZE, out cbReportedDataSize);


       //   uint tmpBufferSize = 0;
       //   uint tmpTransferSize = 0;
       //   string tmpTransferContext = string.Empty;
       //   {
       //     pResults.GetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, out tmpTransferContext);
       //     pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_TOTAL_DATA_SIZE, out tmpBufferSize);
       //     pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPTIMAL_TRANSFER_BUFFER_SIZE, out tmpTransferSize);

       //     try
       //     {
       //       int pValue;
       //       pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
       //       if(pValue!=0)
       //       {
       //         return null;
       //       }
       //     }
       //     catch
       //     {
       //     }
       //   }

       //   pParameters.Clear();
       //   pResults.Clear();

       //   byte[] tmpData = new byte[(int)tmpTransferSize];
       //   //CCustomReadContext{81CD75F1-A997-4DA2-BAB1-FF5EC514E355}
       //   pParameters.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_READ_DATA.fmtid);
       //   pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_READ_DATA.pid);
       //   pParameters.SetStringValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, tmpTransferContext);
       //   pParameters.SetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ref tmpData[0], (uint)tmpTransferSize);
       //   pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_TO_READ, (uint)tmpTransferSize);
       //   pParameters.SetIPortableDevicePropVariantCollectionValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_OPERATION_PARAMS, propVariant);


       //   portableDeviceClass.SendCommand(0, pParameters, out pResults);


       //   uint cbBytesRead = 0;

       //   try
       //   {
       //     int pValue = 0;
       //     pResults.GetErrorValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out pValue);
       //     if (pValue != 0)
       //       return null;
       //   }
       //   catch(Exception ex)
       //   {
       //   }
       //   // 24,142,174,9
       //   // 18, 8E  
       //   GCHandle pinnedArray = GCHandle.Alloc(imgdate, GCHandleType.Pinned);
       //   IntPtr ptr = pinnedArray.AddrOfPinnedObject();
          
       //   uint dataread =0;
       //   pResults.GetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_READ, out dataread);
       //   pResults.GetBufferValue(ref PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_DATA, ptr, out cbBytesRead);

       //   IntPtr tmpPtr = new IntPtr(Marshal.ReadInt64(ptr));
       //   byte[] res = new byte[(int)cbBytesRead];
       //   for (int i = 0; i < cbBytesRead; i++)
       //   {
       //     res[i] = Marshal.ReadByte(tmpPtr, i);
       //   }

       //   pParameters.Clear();
       //   pResults.Clear();
       //   {
       //     pParameters.SetGuidValue(PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_CATEGORY, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_END_DATA_TRANSFER.fmtid);
       //     pParameters.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_COMMAND_ID, PortableDevicePKeys.WPD_COMMAND_MTP_EXT_END_DATA_TRANSFER.pid);
       //     pParameters.SetStringValue(PortableDevicePKeys.WPD_PROPERTY_MTP_EXT_TRANSFER_CONTEXT, tmpTransferContext);
       //   }

       //   portableDeviceClass.SendCommand(0, pParameters, out pResults);

       //   Marshal.FreeHGlobal(tmpPtr);
       //   pinnedArray.Free();
       //   //Marshal.FreeHGlobal(ptr);

       //   try
       //   {
       //     int tmpResult = 0;

       //     pResults.GetErrorValue(ref PortableDevicePKeys.WPD_PROPERTY_COMMON_HRESULT, out tmpResult);
       //     if(tmpResult!=0)
       //     {
              
       //     }
       //   }
       //   catch
       //   {
       //   }
       //   return res;
       //}
      
        /// <summary>
        /// Transfer from device to computer
        /// Source : http://cgeers.com/2011/08/13/wpd-transferring-content/
        /// </summary>
        /// <param name="deviceObject"></param>
        /// <param name="fileName"></param>
        public void SaveFile(PortableDeviceObject deviceObject, string fileName)
        {
          IPortableDeviceContent content;
          portableDeviceClass.Content(out content);
          IPortableDeviceResources resources;
          content.Transfer(out resources);

          PortableDeviceApiLib.IStream wpdStream = null;
          uint optimalTransferSize = 0;

          var property = PortableDevicePKeys.WPD_RESOURCE_DEFAULT;

          
          try
          {
            resources.GetStream(deviceObject.ID, ref property, 0, ref optimalTransferSize,
                    out wpdStream);
          }
          catch (COMException comException)
          {
            // check if the device is busy, this may hapen when a another transfer not finished 
            if ((uint)comException.ErrorCode == PortableDeviceErrorCodes.ERROR_BUSY)
            {
              Thread.Sleep(500);
              SaveFile(deviceObject, fileName);
              return;
            }
            throw comException;
          }

          System.Runtime.InteropServices.ComTypes.IStream sourceStream =
              (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

          FileStream targetStream = new FileStream(fileName,
              FileMode.Create, FileAccess.Write);

          unsafe
          {
            var buffer = new byte[1024*256];
            int bytesRead;
            do
            {
              sourceStream.Read(buffer, buffer.Length, new IntPtr(&bytesRead));
              targetStream.Write(buffer, 0, bytesRead);
            } while (bytesRead > 0);

            targetStream.Close();
          }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsConnected)
                return FriendlyName;
            else
                return DeviceId;
        }

        /// <summary>
        /// Dispose the unmanaged resource
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected functions

        /// <summary>
        /// Raise the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Internal functions

        /// <summary>
        /// Raise event from device
        /// </summary>
        internal void RaiseEvent(PortableDeviceEventType eventType)
        {
            if (this.DeviceEvent != null)
            {
                this.DeviceEvent(this, new PortableDeviceEventArgs(eventType));
            }
        }

        #endregion

        #region Private functions

        private string GetStringProperty(_tagpropertykey propertyKey)
        {
            //Ensure we are connected to device
            CheckIfIsConnected();

            IPortableDeviceContent content;
            IPortableDeviceProperties properties;
            PortableDeviceApiLib.IPortableDeviceValues propertyValues;

            this.portableDeviceClass.Content(out content);
            content.Properties(out properties);

            properties.GetValues("DEVICE", null, out propertyValues);

            string val = string.Empty;
            propertyValues.GetStringValue(ref propertyKey, out val);

            return val;
        }

        //

        private int GetIntegerProperty(string devisd, _tagpropertykey propertyKey)
        {
          //Ensure we are connected to device
          CheckIfIsConnected();

          IPortableDeviceContent content;
          IPortableDeviceProperties properties;
          PortableDeviceApiLib.IPortableDeviceValues propertyValues;

          this.portableDeviceClass.Content(out content);

          content.Properties(out properties);
          properties.GetValues(devisd, null, out propertyValues);
          float val = -1;
          propertyValues.GetFloatValue(ref propertyKey, out val);
          return Convert.ToInt32(val);
        }

        private int GetIntegerProperty(_tagpropertykey propertyKey)
        {
            //Ensure we are connected to device
            CheckIfIsConnected();

            IPortableDeviceContent content;
            IPortableDeviceProperties properties;
            PortableDeviceApiLib.IPortableDeviceValues propertyValues;

            this.portableDeviceClass.Content(out content);
          
            content.Properties(out properties);
            properties.GetValues("DEVICE", null, out propertyValues);
          float val = -1;
            propertyValues.GetFloatValue(ref propertyKey, out val);
            return Convert.ToInt32(val);
        }

        private void CheckIfIsConnected()
        {
            if (!this.IsConnected)
                throw new Exception("Not connected");
        }

        private void ExtractDeviceCapabilities()
        {
            deviceCapabilities = new PortableDeviceCapabilities();
            deviceCapabilities.ExtractDeviceCapabilities(this.portableDeviceClass);
            deviceCapabilities.ExtractCommands(this.portableDeviceClass);
            deviceCapabilities.ExtractEvents(this.portableDeviceClass);
        }

        private void StartEnumerate()
        {
            lock (this.dispatcher)
            {

                PortableDeviceApiLib.IPortableDeviceContent pContent;
                this.portableDeviceClass.Content(out pContent);

                this.content = new PortableDeviceFonctionalObject("DEVICE");
                Enumerate(ref pContent, "DEVICE", this.content);

                this.RaisePropertyChanged("Content");
            }
        }

        private void Enumerate(ref PortableDeviceApiLib.IPortableDeviceContent pContent, string parentID, PortableDeviceContainerObject node)
        {
            PortableDeviceApiLib.IPortableDeviceProperties properties;
            pContent.Properties(out properties);

            PortableDeviceApiLib.IEnumPortableDeviceObjectIDs pEnum;
            pContent.EnumObjects(0, parentID, null, out pEnum);

            uint cFetched = 0;
            PortableDeviceObject current;
            do
            {
                string objectID;
                pEnum.Next(1, out objectID, ref cFetched);

                if (cFetched > 0)
                {
                    current = this.ExtractInformation(properties, objectID);
                    node.AddChild(current);
                    if (current is PortableDeviceContainerObject)
                        Enumerate(ref pContent, objectID, (PortableDeviceContainerObject)current);
                }

            } while (cFetched > 0);
        }

        private PortableDeviceObject ExtractInformation(PortableDeviceApiLib.IPortableDeviceProperties properties, string objectId)
        {
            PortableDeviceApiLib.IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            PortableDeviceApiLib.IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            string tmpVal;
            string name;
            values.GetStringValue(ref PortableDevicePKeys.WPD_OBJECT_NAME, out name);

            values.GetStringValue(ref PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, out tmpVal);
            var guid = new Guid(tmpVal);
            string contentType = PortableDeviceHelpers.GetKeyNameFromGuid(guid);

            values.GetStringValue(ref PortableDevicePKeys.WPD_OBJECT_FORMAT, out tmpVal);
            string formatType = PortableDeviceHelpers.GetKeyNameFromGuid(new Guid(tmpVal));

            return Factories.PortableDeviceObjectFactory.Instance.CreateInstance(guid, objectId, name, contentType, formatType);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(this.adviseCookie))
                    this.portableDeviceClass.Unadvise(this.adviseCookie);

                if (this.IsConnected)
                    this.portableDeviceClass.Close();
            }

            this.portableDeviceClass = null;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PortableDevice()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
