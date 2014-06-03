using System;
using CameraControl.Devices.Classes;

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

        bool HostMode { get; set; }

        PropertyValue<int> FNumber { get; set; }
        PropertyValue<int> IsoNumber { get; set; }
        PropertyValue<long> ShutterSpeed { get; set; }
        PropertyValue<long> WhiteBalance { get; set; }
        PropertyValue<uint> Mode { get; set; }
        PropertyValue<int> ExposureCompensation { get; set; }
        PropertyValue<int> CompressionSetting { get; set; }
        PropertyValue<int> ExposureMeteringMode { get; set; }
        PropertyValue<long> FocusMode { get; set; }
        DateTime DateTime { get; set; }

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

        /// <summary>
        /// Check is a capability is supported
        /// </summary>
        /// <param name="capabilityEnum">The capability enum.</param>
        /// <returns><c>true</c> if capability supported</returns>
        bool GetCapability(CapabilityEnum capabilityEnum);
        uint TransferProgress { get; set; }

        int Battery { get; set; }
        PropertyValue<int> LiveViewImageZoomRatio { get; set; }

        bool Init(DeviceDescriptor deviceDescriptor);
        void StartLiveView();
        void StopLiveView();
        LiveViewData GetLiveViewImage();
        void AutoFocus();
        void Focus(int step);
        void Focus(int x, int y);
        void CapturePhotoNoAf();
        void CapturePhoto();
        void StartRecordMovie();
        void StopRecordMovie();
        string GetProhibitionCondition(OperationEnum operationEnum);
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

        void TransferFile(object o, string filename);

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

        /// <summary>
        /// Gets files stored in card.
        /// </summary>
        /// <param name="storageId">The storage id.</param>
        /// <returns></returns>
        AsyncObservableCollection<DeviceObject> GetObjects(object storageId);
        void FormatStorage(object storageId);
        bool DeleteObject(DeviceObject deviceObject);

        void SetCameraField(CameraFieldType cameraFieldType, string comment);

    }
}