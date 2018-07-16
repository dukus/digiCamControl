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
using CameraControl.Devices.Classes;
using System.Text;

#endregion

namespace CameraControl.Devices
{
    public class BaseCameraDevice : BaseFieldClass, ICameraDevice
    {
        #region Implementation of ICameraDevice

        protected List<CapabilityEnum> Capabilities = new List<CapabilityEnum>();
        protected object Locker = new object(); // object used to lock multi threaded methods 

        private bool _haveLiveView;

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged("IsBusy");
            }
        }

        public virtual bool HaveLiveView
        {
            get { return _haveLiveView; }
            set
            {
                _haveLiveView = value;
                NotifyPropertyChanged("HaveLiveView");
            }
        }

        private bool _captureInSdRam;

        public virtual bool CaptureInSdRam
        {
            get { return _captureInSdRam; }
            set
            {
                _captureInSdRam = value;
                NotifyPropertyChanged("CaptureInSdRam");
            }
        }

        private PropertyValue<long> _isoNumber;

        public virtual PropertyValue<long> IsoNumber
        {
            get { return _isoNumber; }
            set
            {
                _isoNumber = value;
                NotifyPropertyChanged("IsoNumber");
            }
        }

        private PropertyValue<long> _shutterSpeed;

        public virtual PropertyValue<long> ShutterSpeed
        {
            get { return _shutterSpeed; }
            set
            {
                _shutterSpeed = value;
                NotifyPropertyChanged("ShutterSpeed");
            }
        }

        private PropertyValue<long> _mode;

        public virtual PropertyValue<long> Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                NotifyPropertyChanged("Mode");
            }
        }

        private PropertyValue<long> _fNumber;

        public virtual PropertyValue<long> FNumber
        {
            get { return _fNumber; }
            set
            {
                _fNumber = value;
                NotifyPropertyChanged("FNumber");
            }
        }

        private PropertyValue<long> _whiteBalance;

        public virtual PropertyValue<long> WhiteBalance
        {
            get { return _whiteBalance; }
            set
            {
                _whiteBalance = value;
                NotifyPropertyChanged("WhiteBalance");
            }
        }

        private PropertyValue<long> _exposureCompensation;

        public virtual PropertyValue<long> ExposureCompensation
        {
            get { return _exposureCompensation; }
            set
            {
                _exposureCompensation = value;
                NotifyPropertyChanged("ExposureCompensation");
            }
        }

        private PropertyValue<long> _compressionSetting;

        public virtual PropertyValue<long> CompressionSetting
        {
            get { return _compressionSetting; }
            set
            {
                _compressionSetting = value;
                NotifyPropertyChanged("CompressionSetting");
            }
        }

        private PropertyValue<long> _exposureMeteringMode;

        public virtual PropertyValue<long> ExposureMeteringMode
        {
            get { return _exposureMeteringMode; }
            set
            {
                _exposureMeteringMode = value;
                NotifyPropertyChanged("ExposureMeteringMode");
            }
        }

        private PropertyValue<long> _focusMode;

        public virtual PropertyValue<long> FocusMode
        {
            get
            {
                return _focusMode;
            }
            set
            {
                _focusMode = value;
                NotifyPropertyChanged("FocusMode");
            }
        }

        private DateTime _dateTime;

        public virtual DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                NotifyPropertyChanged("DateTime");
            }
        }

        public string PortName { get; set; }

        private string _deviceName;

        private bool _isChecked;

        public virtual bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        private object _attachedPhotoSession;

        public virtual object AttachedPhotoSession
        {
            get { return _attachedPhotoSession; }
            set
            {
                _attachedPhotoSession = value;
                NotifyPropertyChanged("AttachedPhotoSession");
            }
        }

        public virtual string DeviceName
        {
            get { return _deviceName; }
            set
            {
                _deviceName = value;
                NotifyPropertyChanged("DeviceName");
            }
        }

        private string _manufacturer;

        public virtual string Manufacturer
        {
            get { return _manufacturer; }
            set
            {
                _manufacturer = value;
                NotifyPropertyChanged("Manufacturer");
            }
        }

        private string _serialNumber;

        public virtual string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                NotifyPropertyChanged("SerialNumber");
            }
        }

        private string _displayName;

        public virtual string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                    return DeviceName + " (" + SerialNumber + ")";
                return _displayName;
            }
            set
            {
                _displayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }


        private int _exposureStatus;

        public virtual int ExposureStatus
        {
            get { return _exposureStatus; }
            set
            {
                _exposureStatus = value;
                NotifyPropertyChanged("ExposureStatus");
            }
        }

        public bool PreventShutDown
        {
            get { return _preventShutDown; }
            set
            {
                _preventShutDown = value;
                NotifyPropertyChanged("PreventShutDown");
            }
        }

        private bool _isConnected;

        public virtual bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                NotifyPropertyChanged("IsConnected");
            }
        }


        private int _battery;

        public virtual int Battery
        {
            get { return _battery; }
            set
            {
                _battery = value;
                NotifyPropertyChanged("Battery");
            }
        }

        private uint _transferProgress;

        public uint TransferProgress
        {
            get { return _transferProgress; }
            set
            {
                _transferProgress = value;
                NotifyPropertyChanged("TransferProgress");
            }
        }

        public virtual bool GetCapability(CapabilityEnum capabilityEnum)
        {
            return Capabilities.Contains(capabilityEnum);
        }

        public virtual PropertyValue<long> LiveViewImageZoomRatio { get; set; }

        public virtual bool Init(DeviceDescriptor deviceDescriptor)
        {
            return true;
        }

        public virtual void StartLiveView()
        {
        }

        public virtual void StopLiveView()
        {
        }

        public virtual string GetLiveViewStream()
        {
            throw new NotImplementedException();
        }

        public virtual LiveViewData GetLiveViewImage()
        {
            return null;
        }

        public virtual void AutoFocus()
        {
        }

        public virtual int Focus(int step)
        {
            return 0;
        }

        public virtual void Focus(int x, int y)
        {
        }

        public virtual void Focus(FocusDirection direction, FocusAmount amount)
        {
            throw new NotImplementedException();
        }

        public virtual void CapturePhotoNoAf()
        {
        }

        public virtual void CapturePhoto()
        {
        }

        public virtual void StartRecordMovie()
        {
        }

        public virtual void StopRecordMovie()
        {
        }

        public virtual string GetProhibitionCondition(OperationEnum operationEnum)
        {
            return "";
        }

        public virtual bool GetStatus(OperationEnum operationEnum)
        {
            throw new NotImplementedException();
        }

        public virtual void EndBulbMode()
        {
        }

        public virtual void StartBulbMode()
        {
        }

        public virtual void LockCamera()
        {
        }

        public virtual void UnLockCamera()
        {
        }

        public virtual void Close()
        {
        }

        public virtual void StartZoom(ZoomDirection direction)
        {
            throw new NotImplementedException();
        }

        public virtual void StopZoom(ZoomDirection direction)
        {
            throw new NotImplementedException();
        }

        public virtual void ResetDevice()
        {
            
        }

        public virtual void ReleaseResurce(object o)
        {
            
        }

        public virtual void TransferFileThumb(object o, string filename)
        {
            TransferFile(o, filename);
        }

        public virtual void ReadDeviceProperties(uint prop)
        {
        }

        public virtual void TransferFile(object o, string filename)
        {
        }

        public virtual void TransferFile(object o, System.IO.Stream stream)
        {
        }

        public void OnCaptureCompleted(object sender, EventArgs args)
        {
            if (CaptureCompleted != null)
            {
                CaptureCompleted(sender, args);
            }
        }

        public void OnPhotoCapture(object sender, PhotoCapturedEventArgs args)
        {
            if (PhotoCaptured != null)
            {
                PhotoCaptured(sender, args);
            }
        }

        public void OnCameraDisconnected(object sender, DisconnectCameraEventArgs eventHandler)
        {
            if (CameraDisconnected != null)
            {
                CameraDisconnected(sender, eventHandler);
            }
        }

        public event PhotoCapturedEventHandler PhotoCaptured;
        public event EventHandler CaptureCompleted;
        public event CameraDisconnectedEventHandler CameraDisconnected;
        public event CameraDeviceManager.CameraConnectedEventHandler CameraInitDone;

        public void OnCameraInitDone()
        {
            try
            {
                CameraDeviceManager.CameraConnectedEventHandler handler = CameraInitDone;
                if (handler != null) handler(this);
            }
            catch (Exception ex)
            {
                Log.Error("OnCameraInitDone", ex);
            }
        }

        private AsyncObservableCollection<PropertyValue<long>> _advancedProperties;
        private AsyncObservableCollection<PropertyValue<long>> _properties;
        private bool _preventShutDown;

        public AsyncObservableCollection<PropertyValue<long>> AdvancedProperties
        {
            get { return _advancedProperties; }
            set
            {
                _advancedProperties = value;
                NotifyPropertyChanged("AdvancedProperties");
            }
        }

        public AsyncObservableCollection<PropertyValue<long>> Properties
        {
            get { return _properties; }
            set
            {
                _properties = value;
                NotifyPropertyChanged("Properties");
            }
        }

        public virtual AsyncObservableCollection<DeviceObject> GetObjects(object storageId, bool loadThumbs)
        {
            throw new NotImplementedException();
        }

        public virtual void FormatStorage(object storageId)
        {
            throw new NotImplementedException();
        }

        public virtual bool DeleteObject(DeviceObject deviceObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the in comment field.
        /// </summary>
        /// <param name="cameraFieldType"> </param>
        /// <param name="comment">The comment.</param>
        public virtual void SetCameraField(CameraFieldType cameraFieldType, string comment)
        {
        }

        public virtual void WaitForReady()
        {
            
        }

        #endregion

        public BaseCameraDevice()
        {
            IsChecked = true;
            AdvancedProperties = new AsyncObservableCollection<PropertyValue<long>>();
            Properties = new AsyncObservableCollection<PropertyValue<long>>();
            Capabilities = new List<CapabilityEnum>();
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public virtual string ToStringCameraData()
        {
            // return DisplayName;
            StringBuilder c = new StringBuilder("\n\nPost shot camera data:\n");
            try
            {
            if (PortName != null)
                c.AppendFormat("Port: {0}\n", PortName);
            c.AppendFormat("    {0}{1}", String.IsNullOrWhiteSpace(Manufacturer) ? "" : "(" + Manufacturer + ") ", DeviceName);
            c.AppendFormat("\n\tSerial number........ {0}", SerialNumber);
            c.AppendFormat("\n\tBusy..................{0}", IsBusy ? "Yes" : "No");
            c.AppendFormat("\n\tHave LiveView.........{0}", HaveLiveView ? "Yes" : "No");
            c.AppendFormat("\n\tCapture SDRam.........{0}", CaptureInSdRam ? "Yes" : "No");
            c.AppendFormat("\n\tIs Connected..........{0}", IsConnected ? "Yes" : "No");
            c.AppendFormat("\n\tMode..................{0}, {1} focus", Mode?.Value, FocusMode?.Value);
            c.AppendFormat("\n\texposure..............{0} f{1} +/-{2}, ISO{3}",
                    ShutterSpeed.Value,
                    FNumber.Value,
                    ExposureCompensation.Value,
                    IsoNumber.Value);
            c.AppendFormat("\n\tmetering mode.........{0}", ExposureMeteringMode.Value);
            c.AppendFormat("\n\twhite balance.........{0}", WhiteBalance.Value);
            c.AppendFormat("\n\tTransfer progress.....{0}%", TransferProgress);

            c.AppendFormat("\n\tExposure status.......{0}", ExposureStatus);
            c.AppendFormat("\n\tBattery...............{0}%", Battery);
            c.AppendFormat("\n\tCompression...........{0}%", CompressionSetting);

            if (Capabilities.Count > 0)
            {
                c.Append(String.Format("\n\tCapabilities ({0} present of {1}):", Capabilities.Count, Enum.GetValues(typeof(CapabilityEnum)).Length));
                foreach (CapabilityEnum x in Enum.GetValues(typeof(CapabilityEnum)))
                {
                    c.AppendFormat("\n\t  {0,-20}{1}", x.ToString(""), Capabilities.Contains(x) ? "Yes" : "No");
                }
            }

            }
            catch (Exception ex)
            {
            }

            return c.ToString();
        }
    }
}