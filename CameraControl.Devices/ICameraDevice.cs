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
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Devices
{
    public interface ICameraDevice
    {
        /// <summary>
        /// If false the camera is ready to take next capture
        /// Should be handled by the user code
        /// </summary>
        bool IsBusy { get; set; }

        /// <summary>
        /// This property will be removed, replaced by GetCapability
        /// </summary>
        bool HaveLiveView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether photos captured are captured in SDRam.
        /// </summary>
        /// <value>
        ///   <c>true</c> if true photos are captured in SDRam; otherwise, <c>false</c> photos will be recorded in card.
        /// </value>
        bool CaptureInSdRam { get; set; }

        PropertyValue<long> FNumber { get; set; }
        PropertyValue<long> IsoNumber { get; set; }
        PropertyValue<long> ShutterSpeed { get; set; }
        PropertyValue<long> WhiteBalance { get; set; }
        PropertyValue<long> Mode { get; set; }
        PropertyValue<long> ExposureCompensation { get; set; }
        PropertyValue<long> CompressionSetting { get; set; }
        PropertyValue<long> ExposureMeteringMode { get; set; }
        PropertyValue<long> FocusMode { get; set; }
        DateTime DateTime { get; set; }
        /// <summary>
        /// Gets or sets a unique indentifier for device based on connection mode.
        /// </summary>
        /// <value>
        /// The name of the port.
        /// </value>
        string PortName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the camera is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; set; }

        bool IsChecked { get; set; }

        /// <summary>
        /// Gets or sets the attached abject to the camera .
        /// </summary>
        /// <value>
        /// The attached photo session.
        /// </value>
        object AttachedPhotoSession { get; set; }

        string DeviceName { get; set; }
        string Manufacturer { get; set; }
        string SerialNumber { get; set; }
        string DisplayName { get; set; }
        int ExposureStatus { get; set; }
        bool PreventShutDown { get; set; }

        /// <summary>
        /// Check is a capability is supported
        /// </summary>
        /// <param name="capabilityEnum">The capability enum.</param>
        /// <returns><c>true</c> if capability supported</returns>
        bool GetCapability(CapabilityEnum capabilityEnum);

        /// <summary>
        /// The current file transfer progress (0-100 %)
        /// </summary>
        /// <value>
        /// The transfer progress.
        /// </value>
        uint TransferProgress { get; set; }

        int Battery { get; set; }
        PropertyValue<long> LiveViewImageZoomRatio { get; set; }

        bool Init(DeviceDescriptor deviceDescriptor);
        void StartLiveView();
        void StopLiveView();
        string GetLiveViewStream();

        LiveViewData GetLiveViewImage();
        void AutoFocus();
        int Focus(int step);
        void Focus(int x, int y);
        void Focus(FocusDirection direction, FocusAmount amount);
        void CapturePhotoNoAf();
        void CapturePhoto();
        void StartRecordMovie();
        void StopRecordMovie();
        /// <summary>
        /// Gets the prohibition condition for the specified operation.
        /// If a operation can be executed empty string will returned,
        /// Else the error code or error description 
        /// </summary>
        /// <param name="operationEnum">The operation enum.</param>
        /// <returns></returns>
        string GetProhibitionCondition(OperationEnum operationEnum);
        bool GetStatus(OperationEnum operationEnum);
        /// <summary>
        /// Support only if capability Bulb is specified
        /// </summary>
        void EndBulbMode();

        /// <summary>
        /// Support only if capability Bulb is specified
        /// </summary>
        void StartBulbMode();

        void LockCamera();
        void UnLockCamera();
        void Close();

        void StartZoom(ZoomDirection direction);
        void StopZoom(ZoomDirection direction);

        /// <summary>
        /// Should be called after file tranferred 
        /// </summary>
        /// <param name="o">The o.</param>
        void ReleaseResurce(object o);

        void TransferFileThumb(object o, string filename);
        void TransferFile(object o, string filename);
        void TransferFile(object o, System.IO.Stream stream);

        /// <summary>
        /// Occurs when photo captured.
        /// </summary>
        event PhotoCapturedEventHandler PhotoCaptured;

        /// <summary>
        /// Occurs when all capture images done.
        /// </summary>
        event EventHandler CaptureCompleted;

        /// <summary>
        /// Occurs when [camera disconnected].
        /// </summary>
        event CameraDisconnectedEventHandler CameraDisconnected;

        event CameraDeviceManager.CameraConnectedEventHandler CameraInitDone;

        /// <summary>
        /// Gets or sets the arbitrary number of advanced properties.
        /// </summary>
        /// <value>
        /// The advanced properties.
        /// </value>
        AsyncObservableCollection<PropertyValue<long>> AdvancedProperties { get; set; }

        AsyncObservableCollection<PropertyValue<long>> Properties { get; set; }
        /// <summary>
        /// Gets files stored in card.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <param name="loadThumbs">Load thumbnail atached to object</param>
        /// <returns></returns>
        AsyncObservableCollection<DeviceObject> GetObjects(object storageId, bool loadThumbs);

        void FormatStorage(object storageId);
        bool DeleteObject(DeviceObject deviceObject);

        void SetCameraField(CameraFieldType cameraFieldType, string comment);

    }
}