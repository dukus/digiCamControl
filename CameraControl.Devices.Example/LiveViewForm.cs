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
using Timer = System.Windows.Forms.Timer;

namespace CameraControl.Devices.Example
{
    public partial class LiveViewForm : Form
    {
        public ICameraDevice CameraDevice { get; set; }
        private Timer _liveViewTimer = new Timer();

        public LiveViewForm(ICameraDevice cameraDevice)
        {
            //set live view default frame rate to 15
            _liveViewTimer.Interval = 1000 / 15;
            _liveViewTimer.Stop();
            _liveViewTimer.Tick += _liveViewTimer_Tick;
            CameraDevice = cameraDevice;
            CameraDevice.CameraDisconnected += CameraDevice_CameraDisconnected;
            InitializeComponent();
        }

        void CameraDevice_CameraDisconnected(object sender, DisconnectCameraEventArgs eventArgs)
        {
            MethodInvoker method = delegate
            {
                _liveViewTimer.Stop();
                Thread.Sleep(100);
                Close();
            };
            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
        }

        void _liveViewTimer_Tick(object sender, EventArgs e)
        {
            LiveViewData liveViewData = null;
            try
            {
                liveViewData = CameraDevice.GetLiveViewImage();
            }
            catch (Exception)
            {
                return;
            }

            if (liveViewData == null || liveViewData.ImageData == null)
            {
                return;
            }
            try
            {
                pictureBox1.Image = new Bitmap(new MemoryStream(liveViewData.ImageData,
                                                                liveViewData.ImageDataPosition,
                                                                liveViewData.ImageData.Length -
                                                                liveViewData.ImageDataPosition));
            }
            catch (Exception)
            {

            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            new Thread(StartLiveView).Start();
        }

        private void StartLiveView()
        {
            bool retry;
            do
            {
                retry = false;
                try
                {
                    CameraDevice.StartLiveView();
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
            MethodInvoker method = () => _liveViewTimer.Start();
            if (InvokeRequired)
                BeginInvoke(method);
            else
                method.Invoke();
        
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            new Thread(StopLiveView).Start();
        }

        private void StopLiveView()
        {
            bool retry;
            do
            {
                retry = false;
                try
                {
                    _liveViewTimer.Stop();
                    // wait for last get live view image
                    Thread.Sleep(500);
                    CameraDevice.StopLiveView();
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

        private void LiveViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            btn_stop_Click(null, null);
        }
    }
}
