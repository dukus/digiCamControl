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

        protected bool _hostMode;

        public virtual bool HostMode
        {
            get { return _hostMode; }
            set
            {
                _hostMode = value;
                NotifyPropertyChanged("HostMode");
            }
        }

        private PropertyValue<int> _isoNumber;

        public virtual PropertyValue<int> IsoNumber
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

        private PropertyValue<uint> _mode;

        public virtual PropertyValue<uint> Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                NotifyPropertyChanged("Mode");
            }
        }

        private PropertyValue<int> _fNumber;

        public virtual PropertyValue<int> FNumber
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

        private PropertyValue<int> _exposureCompensation;

        public virtual PropertyValue<int> ExposureCompensation
        {
            get { return _exposureCompensation; }
            set
            {
                _exposureCompensation = value;
                NotifyPropertyChanged("ExposureCompensation");
            }
        }

        private PropertyValue<int> _compressionSetting;

        public virtual PropertyValue<int> CompressionSetting
        {
            get { return _compressionSetting; }
            set
            {
                _compressionSetting = value;
                NotifyPropertyChanged("CompressionSetting");
            }
        }

        private PropertyValue<int> _exposureMeteringMode;

        public virtual PropertyValue<int> ExposureMeteringMode
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

        public virtual PropertyValue<int> LiveViewImageZoomRatio { get; set; }

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

        public virtual void ResetDevice()
        {
            
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
    }
}