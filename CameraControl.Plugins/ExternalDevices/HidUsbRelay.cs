using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Plugins.ExternalDevices
{
    class HidUsbRelay : IExternalDevice
    {
        public string Name { get; set; }
        private int Hd = 0;

        public HidUsbRelay()
        {
            Name = "HID USB Relay Release";
            DeviceType = SourceEnum.ExternaExternalShutterRelease;
            //Connect();
        }

        public bool Capture(CustomConfig config)
        {
            Connect();
            RelayDeviceWrapper.usb_relay_device_close_all_relay_channel(Hd);
            Thread.Sleep(500);
            RelayDeviceWrapper.usb_relay_device_open_all_relay_channel(Hd);
            RelayDeviceWrapper.usb_relay_device_close(Hd);
            Hd = 0;
            return true;
        }

        public bool Focus(CustomConfig config)
        {
            return true;
        }

        public bool CanExecute(CustomConfig config)
        {
            return true;
        }

        public UserControl GetConfig(CustomConfig config)
        {
            try
            {
                return new HidUsbRelayConfig(config);
            }
            catch (Exception exception)
            {
                Log.Error("", exception);
            }
            return null;
        }

        public SourceEnum DeviceType { get; set; }
        public bool OpenShutter(CustomConfig config)
        {
            Capture(config);
            return true;
        }

        public bool CloseShutter(CustomConfig config)
        {
            Connect();
            RelayDeviceWrapper.usb_relay_device_close_all_relay_channel(Hd);
            RelayDeviceWrapper.usb_relay_device_close(Hd);
            Hd = 0;
            return true;
        }

        public bool AssertFocus(CustomConfig config)
        {
            return true;
        }

        public bool DeassertFocus(CustomConfig config)
        {
            return true;
        }

        private void Connect()
        {
            if(Hd!=0)
                return;
            var dev = RelayDeviceWrapper.usb_relay_device_enumerate();
            if(dev==null)
                throw new Exception("No usb relay device found !");
            Hd = RelayDeviceWrapper.usb_relay_device_open_with_serial_number(dev.Value.serial_number,
                dev.Value.serial_number.Length);
        }

    }
}
