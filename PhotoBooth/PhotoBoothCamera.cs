using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace PhotoBooth
{
    public class PhotoBoothCamera : IDisposable
    {
        private string lastImageFile;
        private bool imageCaptured;
        private List<string> captureFiles;
        private StringBuilder logger = new StringBuilder();

        public bool DeleteFromDevice { get; set; }
        public string InitializationLog { get; private set; }
        public int ImageCount { get; set; }
        public int ImageInterval { get; set; }
        public bool NoAutoFocus { get; set; }

        public event EventHandler CameraDisconnected;
        public event EventHandler CaptureComplete;

        public ICameraDevice Camera
        {
            get { return ServiceProvider.DeviceManager.SelectedCameraDevice; }
        }

        public bool CameraReady
        {
            get
            {
                return ServiceProvider.DeviceManager.SelectedCameraDevice != null && !ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy;
            }
        }
        
        public List<string> CaptureFiles
        {
            get { return this.captureFiles; }
        }

        public PhotoBoothCamera() 
        {
            this.ImageCount = 4;
            this.ImageInterval = 500;
            this.DeleteFromDevice = false;
        }

        ~PhotoBoothCamera()
        {
            Dispose(false);
        }

        public bool Initialize()
        {
            this.logger.Clear();
            bool success = true;
            try
            {
                CameraControl.Devices.Log.LogDebug += Log_LogDebug;
                CameraControl.Devices.Log.LogInfo += Log_LogInfo;
                CameraControl.Devices.Log.LogError += Log_LogError;

                ServiceProvider.Configure();

                ServiceProvider.Settings = new Settings();
                ServiceProvider.Settings = ServiceProvider.Settings.Load();

                ServiceProvider.Settings.DisableNativeDrivers = false;
                ServiceProvider.DeviceManager.DisableNativeDrivers = ServiceProvider.Settings.DisableNativeDrivers;

                //ServiceProvider.Settings.DisableNativeDrivers = true;
                //ServiceProvider.DeviceManager.DisableNativeDrivers = true;
                ServiceProvider.DeviceManager.UseExperimentalDrivers = false;

                ServiceProvider.Settings.LoadSessionData();
                ServiceProvider.Settings.SessionSelected += Settings_SessionSelected;
                ServiceProvider.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
                ServiceProvider.DeviceManager.CameraSelected += DeviceManager_CameraSelected;
                ServiceProvider.DeviceManager.CameraDisconnected += DeviceManager_CameraDisconnected;

                success = ServiceProvider.DeviceManager.ConnectToCamera();
                success &= this.Camera != null && this.Camera.IsConnected;

                if (!success)
                {
                    if (this.Camera != null)
                    {
                        if (this.logger.ToString().Contains("connected"))
                        {
                            success = true;
                        }
                    }
                }

                if (success)
                {
                    ServiceProvider.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
                    this.logger.AppendLine(this.Camera.DisplayName);
                }
            }
            finally
            {
                CameraControl.Devices.Log.LogDebug -= Log_LogDebug;
                CameraControl.Devices.Log.LogInfo -= Log_LogInfo;
                CameraControl.Devices.Log.LogError -= Log_LogError;
            }

            this.InitializationLog = this.logger.ToString();

            return success;
        }

        public void TakePicture()
        {
            this.Capture();
        }

        public void BeginTakePictureSet()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(PictureThreadProc),  this);
        }

        private static void PictureThreadProc(object stateInfo)
        {
            PhotoBoothCamera camera = stateInfo as PhotoBoothCamera;
            if (camera != null)
            {
                camera.TakePictureSet();
            }
        }

        public void TakePictureSet()
        {
            if (ServiceProvider.DeviceManager.SelectedCameraDevice.IsBusy)
            {
                return;
            }

            if (this.ImageCount < 1 || this.ImageCount > 100)
            {
                this.ImageCount = 4;
            }

            if (this.ImageInterval < 250 || this.ImageInterval > 1000)
            {
                this.ImageInterval = 400;
            }

            if (this.CaptureFiles != null && this.CaptureFiles.Count > 0)
            {
                foreach (string filename in this.CaptureFiles)
                {
                    try
                    {
                        Debug.WriteLine("TakePictureSet: deleting {0}", filename);
                        File.Delete(filename);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine("Deleting image exception: {0}", ex);
                    }
                }
            }

            this.CaptureImages();
        }

        private void CaptureImages()
        {
            int currentImage = 0;
            this.captureFiles = new List<string>();
            DateTime nextImageCapture = DateTime.Now;
            while (currentImage < this.ImageCount)
            {
                while (DateTime.Now < nextImageCapture)
                {
                    Thread.Sleep(10);
                }
                DateTime testTime = DateTime.Now;
                Debug.WriteLine("Pre capture: {0}:{1}:{2}:{3}", testTime.Hour, testTime.Minute, testTime.Second, testTime.Millisecond);
                this.Capture();
                testTime = DateTime.Now;
                Debug.WriteLine("Post capture: {0}:{1}:{2}:{3}", testTime.Hour, testTime.Minute, testTime.Second, testTime.Millisecond);
                nextImageCapture = DateTime.Now.AddMilliseconds((double)this.ImageInterval);
                while (!this.imageCaptured)
                {
                    // need to add error time out here
                    Thread.Sleep(10);
                }
                this.captureFiles.Add(this.lastImageFile);
                currentImage++;

                if (this.CaptureComplete != null)
                {
                    this.OnCaptureComplete();
                }
            }
        }

        private void OnCaptureComplete()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(EventThreadProc), this);
        }

        private static void EventThreadProc(object stateInfo)
        {
            PhotoBoothCamera camera = stateInfo as PhotoBoothCamera;
            if (camera.CaptureComplete != null)
            {
                camera.CaptureComplete(camera, EventArgs.Empty);
            }
        }

        private void Capture()
        {
            try
            {
                PhotoBoothCamera.WaitForReady(ServiceProvider.DeviceManager.SelectedCameraDevice);
                this.imageCaptured = false;
                //if (this.NoAutofocus && ServiceProvider.DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.CaptureNoAf))
                if (this.NoAutoFocus)
                    ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhotoNoAf();
                else
                    ServiceProvider.DeviceManager.SelectedCameraDevice.CapturePhoto();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Capture exception: {0}", ex);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                ServiceProvider.DeviceManager.CameraConnected -= DeviceManager_CameraConnected;
                ServiceProvider.DeviceManager.CameraSelected -= DeviceManager_CameraSelected;
                ServiceProvider.DeviceManager.CameraDisconnected -= DeviceManager_CameraDisconnected;
                ServiceProvider.DeviceManager.PhotoCaptured -= DeviceManager_PhotoCaptured;

                ServiceProvider.DeviceManager.CloseAll();
            }
        }

        private void DeviceManager_PhotoCaptured(object sender, CameraControl.Devices.Classes.PhotoCapturedEventArgs eventArgs)
        {
            if (eventArgs != null)
            {
                this.PhotoCaptured(eventArgs);
            }
        }

        private void Log_LogError(LogEventArgs e)
        {
            this.logger.AppendFormat("Error: {0}{1}", e.Message, Environment.NewLine);
        }

        private void Log_LogInfo(LogEventArgs e)
        {
            this.logger.AppendFormat("Info: {0}{1}", e.Message, Environment.NewLine);
        }


        private void Log_LogDebug(LogEventArgs e)
        {
            this.logger.AppendFormat("Debug: {0}{1}", e.Message, Environment.NewLine);
        }

        private void DeviceManager_CameraDisconnected(ICameraDevice cameraDevice)
        {
            if (this.CameraDisconnected != null)
            {
                this.CameraDisconnected(this, EventArgs.Empty);
            }
        }

        private static void WaitForReady(ICameraDevice device)
        {
            if (device != null)
            {
                while (device.IsBusy) { }
            }
        }

        private void PhotoCaptured(PhotoCapturedEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }

            Debug.Assert(eventArgs.CameraDevice != null, "PhotoCaptured: eventArgs.CameraDevice == null");
                 
            ICameraDevice cameraDev = eventArgs.CameraDevice;
            try
            {
                cameraDev.IsBusy = true;
                CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(cameraDev);
                PhotoSession session = (PhotoSession)cameraDev.AttachedPhotoSession ??
                                       ServiceProvider.Settings.DefaultSession;

                if ((property.NoDownload && !eventArgs.CameraDevice.CaptureInSdRam))
                {
                    cameraDev.IsBusy = false;
                    return;
                }

                string fileName = GetTempFilePathWithExtension(".jpg");
                DateTime testTime = DateTime.Now;
                Debug.WriteLine("Pre transfer: {0}:{1}:{2}:{3}", testTime.Hour, testTime.Minute, testTime.Second, testTime.Millisecond);
                cameraDev.TransferFile(eventArgs.Handle, fileName);
                testTime = DateTime.Now;
                Debug.WriteLine("Post transfer: {0}:{1}:{2}:{3}", testTime.Hour, testTime.Minute, testTime.Second, testTime.Millisecond);

                if (this.DeleteFromDevice)
                {
                    try
                    {
                        AsyncObservableCollection<DeviceObject> devObjs = cameraDev.GetObjects(eventArgs.Handle);
                        if (devObjs.Count == 1)
                        {
                            Debug.WriteLine("delete {0}", devObjs[0].FileName);
                            //cameraDev.DeleteObject(devObjs[0]);
                        }
                    }
                    catch (NotImplementedException) { }
                }
                this.lastImageFile = fileName;
                this.imageCaptured = true;
                cameraDev.IsBusy = false;
                Debug.WriteLine("Captured image at {0} to {1}{2}", DateTime.Now.ToLongTimeString(), fileName, Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PhotoCaptured exception: {0}", ex);
                eventArgs.CameraDevice.IsBusy = false;
            }
        }

        private static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }

        private void Settings_SessionSelected(PhotoSession oldvalue, PhotoSession newvalue)
        {
            if (oldvalue != null)
                ServiceProvider.Settings.Save(oldvalue);
            ServiceProvider.QueueManager.Clear();
            if (ServiceProvider.DeviceManager.SelectedCameraDevice != null)
            {
                ServiceProvider.DeviceManager.SelectedCameraDevice.AttachedPhotoSession = newvalue;
            }
        }

        private void DeviceManager_CameraSelected(ICameraDevice oldcameraDevice, ICameraDevice newcameraDevice)
        {
            if (newcameraDevice == null)
                return;
            CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(newcameraDevice);
            // load session data only if not session attached to the selected camera
            if (newcameraDevice.AttachedPhotoSession == null)
            {
                newcameraDevice.AttachedPhotoSession = ServiceProvider.Settings.GetSession(property.PhotoSessionName);
            }
            if (newcameraDevice.AttachedPhotoSession != null)
                ServiceProvider.Settings.DefaultSession = (PhotoSession)newcameraDevice.AttachedPhotoSession;

            if (newcameraDevice.GetCapability(CapabilityEnum.CaptureInRam))
                newcameraDevice.CaptureInSdRam = property.CaptureInSdRam;
        }

        private void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
        {
            CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(cameraDevice);
            cameraDevice.DisplayName = property.DeviceName;
            cameraDevice.AttachedPhotoSession = ServiceProvider.Settings.GetSession(property.PhotoSessionName);
        }
    }
}

