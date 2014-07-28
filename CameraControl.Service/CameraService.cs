using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Service.Exceptions;

namespace CameraControl.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class CameraService : ICameraService
    {
        private static AutoResetEvent _autoEvent = new AutoResetEvent(false);
        private int i = 0;
        private static string latestPicturePath = null;


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
            InitApplication();
            if (ServiceProvider.DeviceManager.SelectedCameraDevice != null)
            {
                ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhoto();
            }
        }

        public string TakePhotoAsBase64String(int timeoutSeconds, int resizeWidth, int resizeHight, int rotation)
        {
            try
            {
                InitApplication();
                _autoEvent.Reset();
                latestPicturePath = null;
                if (ServiceProvider.DeviceManager.SelectedCameraDevice == null || !ServiceProvider.DeviceManager.SelectedCameraDevice.IsConnected)
                    throw CreateFaultException("No camera connected!");
                ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhoto();

                if (_autoEvent.WaitOne(TimeSpan.FromSeconds(timeoutSeconds), false))
                {

                    using (var image = (Image)(new Bitmap(new Bitmap(latestPicturePath), new Size(resizeWidth, resizeHight))))
                    {
                        using (var imageStream = new MemoryStream())
                        {
                            image.Save(imageStream, ImageFormat.Jpeg);

                            var imageBase64 = Convert.ToBase64String(imageStream.ToArray());
                            return imageBase64;
                        }
                    }
                }
                else
                {
                    throw CreateFaultException("Time out !");
                }

            }
            finally
            {
                //ServiceProvider.DeviceManager.PhotoCaptured -= DeviceManager_PhotoCaptured;
            }
        }


        private static void InitApplication()
        {
            if (_isInitialized)
                return;
            ServiceProvider.Configure(Path.Combine(Settings.DataFolder, "Log", "service.log"));
            ServiceProvider.DeviceManager.StartInNewThread = true;
            Log.Debug("Command line utility started");
            ServiceProvider.Settings = new Settings();
            ServiceProvider.Settings = ServiceProvider.Settings.Load();
            ServiceProvider.Settings.LoadSessionData();
            ServiceProvider.WindowsManager = new WindowsManager();
            ServiceProvider.DeviceManager.CameraConnected += DeviceManagerCameraConnected;
            ServiceProvider.DeviceManager.ConnectToCamera();
            ServiceProvider.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
            _isInitialized = true;
        }

        private static void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            if (eventArgs == null)
                return;
            try
            {
                string fileName = Path.GetTempFileName();
                eventArgs.CameraDevice.TransferFile(eventArgs.Handle, fileName);


                eventArgs.CameraDevice.IsBusy = false;

                // delete the old file
                if (string.IsNullOrEmpty(latestPicturePath) && File.Exists(latestPicturePath))
                {
                    File.Delete(latestPicturePath);
                }

                latestPicturePath = fileName;

                if (!File.Exists(fileName))
                {
                    throw CreateFaultException(string.Format("File {0} does not exist after transfer", fileName));
                }
            }
            catch (Exception exception)
            {
                eventArgs.CameraDevice.IsBusy = false;
                throw CreateFaultException(exception.Message);
                //MessageBox.Show( "Error download photo from camera :\n" + exception.Message );
            }
            finally
            {
                ServiceProvider.DeviceManager.PhotoCaptured -= DeviceManager_PhotoCaptured;
                _autoEvent.Set();
            }
        }

        private static void DeviceManagerCameraConnected(ICameraDevice cameradevice)
        {

        }

        private static Exception CreateFaultException(string message)
        {
            var ex = new CustomFaultException {Message = message};
            return new FaultException<CustomFaultException>(ex, message);
        }
    }
}
