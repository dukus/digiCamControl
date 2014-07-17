using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class CameraService : ICameraService
    {
        private static bool _isInitialized = false;

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public void Initialize()
        {
            InitApplication();
        }

        public void Capture()
        {
            if (ServiceProvider.DeviceManager.SelectedCameraDevice != null)
            {
                ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhoto();
            }
        }

        private static void InitApplication()
        {
            if (_isInitialized)
                return;
            ServiceProvider.Configure(Path.Combine(Settings.DataFolder, "Log", "service.log"));
            Log.Debug("Command line utility started");
            ServiceProvider.Settings = new Settings();
            ServiceProvider.Settings = ServiceProvider.Settings.Load();
            ServiceProvider.Settings.LoadSessionData();
            ServiceProvider.WindowsManager = new WindowsManager();
            ServiceProvider.DeviceManager.CameraConnected += DeviceManagerCameraConnected;
            ServiceProvider.DeviceManager.ConnectToCamera();
            ServiceProvider.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
            if (ServiceProvider.DeviceManager.SelectedCameraDevice.AttachedPhotoSession != null)
                ServiceProvider.Settings.DefaultSession = (PhotoSession)
                                                          ServiceProvider.DeviceManager.SelectedCameraDevice.
                                                              AttachedPhotoSession;
            //foreach (ICameraDevice cameraDevice in ServiceProvider.DeviceManager.ConnectedDevices)
            //{
            //    cameraDevice.CaptureCompleted += SelectedCameraDevice_CaptureCompleted;
            //}
            //ServiceProvider.ScriptManager.OutPutMessageReceived += ScriptManager_OutPutMessageReceived;
            //ServiceProvider.DeviceManager.SelectedCameraDevice.CaptureCompleted += SelectedCameraDevice_CaptureCompleted;
            _isInitialized = true;
        }

        private static void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventargs)
        {
            
        }

        private static void DeviceManagerCameraConnected(ICameraDevice cameradevice)
        {
            
        }
    }
}
