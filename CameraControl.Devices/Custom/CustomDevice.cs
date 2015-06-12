using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CameraControl.Devices.Classes;
using PortableDeviceLib;
using PortableDeviceLib.Model;

namespace CameraControl.Devices.Custom
{
    public class CustomDevice:BaseMTPCamera
    {
        public CustomDevice()
        {
            
        }

        public bool Init(DeviceDescriptor deviceDescriptor,DeviceDescription description)
        {
            base.Init(deviceDescriptor);
            StillImageDevice imageDevice = StillImageDevice as StillImageDevice;
            if (imageDevice != null)
                imageDevice.DeviceEvent += StillImageDevice_DeviceEvent;
            return true;
        }

        private void StillImageDevice_DeviceEvent(object sender, PortableDeviceEventArgs e)
        {
            if (e.EventType.EventGuid == PortableDeviceGuids.WPD_EVENT_OBJECT_ADDED)
            {
                var id = e.EventType.DeviceObject.ID;
                PhotoCapturedEventArgs args = new PhotoCapturedEventArgs
                {
                    WiaImageItem = null,
                    CameraDevice = this,
                    FileName = e.EventType.DeviceObject.Name,
                    Handle = e.EventType.DeviceObject.ID
                };
                OnPhotoCapture(this, args);
            }
        }

        public override void TransferFile(object o, Stream stream)
        {
            ((StillImageDevice) StillImageDevice).SaveFile((string) o, stream);
        }

        public override void CapturePhoto()
        {
            Monitor.Enter(Locker);
            try
            {
                IsBusy = true;
                ErrorCodes.GetException( ExecuteWithNoData(CONST_CMD_InitiateCapture));
            }
            catch (COMException comException)
            {
                IsBusy = false;
                ErrorCodes.GetException(comException);
            }
            catch
            {
                IsBusy = false;
                throw;
            }
            finally
            {
                Monitor.Exit(Locker);
            }    
        }
    }
}
