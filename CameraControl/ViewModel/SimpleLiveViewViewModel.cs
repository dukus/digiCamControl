using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.windows;
using GalaSoft.MvvmLight;

namespace CameraControl.ViewModel
{
    public class SimpleLiveViewViewModel : ViewModelBase
    {
        private ICameraDevice _cameraDevice;
        private BitmapSource _bitmap;

        public ICameraDevice CameraDevice
        {
            get { return _cameraDevice; }
            set
            {
                _cameraDevice = value;
                RaisePropertyChanged(() => CameraDevice);
            }
        }

        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChanged(() => Bitmap);
            }
        }

        public SimpleLiveViewViewModel()
        {
            
        }

        public SimpleLiveViewViewModel(ICameraDevice device)
        {
            CameraDevice = device;
        }

        public void Star()
        {
            bool retry = false;
            int retryNum = 0;
            do
            {
                try
                {
                    LiveViewManager.StartLiveView(CameraDevice);
                }
                catch (DeviceException deviceException)
                {
                    if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                        deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                    {
                        Thread.Sleep(100);
                        retry = true;
                        retryNum++;
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (retry && retryNum < 35);
        }

        public void Stop()
        {
            bool retry = false;
            int retryNum = 0;
            do
            {
                try
                {
                    LiveViewManager.StopLiveView(CameraDevice);
                }
                catch (DeviceException deviceException)
                {
                    if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                        deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                    {
                        Thread.Sleep(500);
                        retry = true;
                        retryNum++;
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (retry && retryNum < 35);
        }

        public void Get()
        {
            try
            {
                var LiveViewData = LiveViewManager.GetLiveViewImage(CameraDevice);
                if (LiveViewData != null && LiveViewData.ImageData != null)
                {
                    MemoryStream stream = new MemoryStream(LiveViewData.ImageData,
                        LiveViewData.
                            ImageDataPosition,
                        LiveViewData.ImageData.
                            Length -
                        LiveViewData.
                            ImageDataPosition);

                    using (var res = new Bitmap(stream))
                    {
                       var writeableBitmap =
                                BitmapFactory.ConvertToPbgra32Format(
                                    BitmapSourceConvert.ToBitmapSource(res));
                        writeableBitmap.Freeze();
                        Bitmap = writeableBitmap;
                    }
                }
            }
            catch (Exception)
            {
                
                
            }
        }
        
    }
}
