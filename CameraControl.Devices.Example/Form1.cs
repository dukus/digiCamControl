using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Example
{
    public partial class Form1 : Form
    {

        public CameraDeviceManager DeviceManager { get; set; }
        public string FolderForPhotos { get; set; }

        public Form1()
        {
            DeviceManager = new CameraDeviceManager();
            DeviceManager.CameraSelected += DeviceManager_CameraSelected;
            DeviceManager.CameraConnected += DeviceManager_CameraConnected;
            DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
            DeviceManager.CameraDisconnected += DeviceManager_CameraDisconnected;
            // For experimental Canon driver support- to use canon driver the canon sdk files should be copied in application folder
            DeviceManager.UseExperimentalDrivers = false;
            DeviceManager.DisableNativeDrivers = false;
            FolderForPhotos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Test");
            InitializeComponent();
            Log.LogError += Log_LogDebug;
        }

        void Log_LogDebug(LogEventArgs e)
        {
            MethodInvoker method = delegate
                                     {
                                         textBox1.AppendText((string)e.Message);
                                         if (e.Exception != null)
                                             textBox1.AppendText((string)e.Exception.StackTrace);
                                         textBox1.AppendText(Environment.NewLine);
                                     };
            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
        }

        private void RefreshDisplay()
        {
            MethodInvoker method = delegate
            {
                cmb_cameras.BeginUpdate();
                cmb_cameras.Items.Clear();
                foreach (ICameraDevice cameraDevice in DeviceManager.ConnectedDevices)
                {
                    cmb_cameras.Items.Add(cameraDevice);
                }
                cmb_cameras.DisplayMember = "DeviceName";
                cmb_cameras.SelectedItem = DeviceManager.SelectedCameraDevice;
                // check if camera support live view
                btn_liveview.Enabled = DeviceManager.SelectedCameraDevice.GetCapability(CapabilityEnum.LiveView);
                cmb_cameras.EndUpdate();
            };

            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
        }

        private void PhotoCaptured(object o)
        {
            PhotoCapturedEventArgs eventArgs = o as PhotoCapturedEventArgs;
            if (eventArgs == null)
                return;
            try
            {
                string fileName = Path.Combine(FolderForPhotos, eventArgs.FileName);
                // if file exist try to generate a new filename to prevent file lost. 
                // This useful when camera is set to record in ram the the all file names are same.
                if (File.Exists(fileName))
                    fileName =
                      StaticHelper.GetUniqueFilename(
                        Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_", 0,
                        Path.GetExtension(fileName));

                // check the folder of filename, if not found create it
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                eventArgs.CameraDevice.TransferFile(eventArgs.Handle, fileName);
                // the IsBusy may used internally, if file transfer is done should set to false  
                eventArgs.CameraDevice.IsBusy = false;
                img_photo.ImageLocation = fileName;
            }
            catch (Exception exception)
            {
                eventArgs.CameraDevice.IsBusy = false;
                MessageBox.Show("Error download photo from camera :\n" + exception.Message);
            }
        }

        void DeviceManager_CameraDisconnected(ICameraDevice cameraDevice)
        {
            RefreshDisplay();
        }

        void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            // to prevent UI freeze start the transfer process in a new thread
            Thread thread = new Thread(PhotoCaptured);
            thread.Start(eventArgs);
        }

        void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
        {
            RefreshDisplay();
        }

        void DeviceManager_CameraSelected(ICameraDevice oldcameraDevice, ICameraDevice newcameraDevice)
        {
            MethodInvoker method = delegate
            {
                btn_liveview.Enabled = newcameraDevice.GetCapability(CapabilityEnum.LiveView);
            };
            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DeviceManager.ConnectToCamera();
            RefreshDisplay();
        }

        private void btn_capture_Click(object sender, EventArgs e)
        {
            Thread thread=new Thread(Capture);
            thread.Start();
        }

        private void Capture()
        {
            bool retry;
            do
            {
                retry = false;
                try
                {
                    DeviceManager.SelectedCameraDevice.CapturePhoto();
                }
                catch (DeviceException exception)
                {
                    if (exception.ErrorCode == ErrorCodes.MTP_Device_Busy || exception.ErrorCode == ErrorCodes.ERROR_BUSY)
                    {
                        // this may cause infinite loop
                        Thread.Sleep(100);
                        retry = true;
                    }
                    else
                    {
                        MessageBox.Show("Error occurred :" + exception.Message);                        
                    }
                }
   
            } while (retry);
        }

        private void cmb_cameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeviceManager.SelectedCameraDevice = (ICameraDevice)cmb_cameras.SelectedItem;
        }

        private void btn_liveview_Click(object sender, EventArgs e)
        {
            LiveViewForm form = new LiveViewForm(DeviceManager.SelectedCameraDevice);
            form.ShowDialog();
        }



    }
}
