using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Classes
{
    static class WpfCommands
    {
        /// <summary>
        /// Gets the command for selecting a device
        /// </summary>
        public static RelayCommand<ICameraDevice> SelectDeviceCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the show live view command. As command parameter ICameraDevice required
        /// </summary>
        /// <value>
        /// The show live view command.
        /// </value>
        public static RelayCommand<ICameraDevice> ShowLiveViewCommand
        {
            get;
            private set;
        }

        public static RelayCommand<ICameraDevice> DevicePropertyCommand
        {
            get;
            private set;
        }



        static WpfCommands()
        {
            SelectDeviceCommand = new RelayCommand<ICameraDevice>(SelectCamera);
            ShowLiveViewCommand = new RelayCommand<ICameraDevice>(device => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Show, device), device => (device != null && device.GetCapability(CapabilityEnum.LiveView)));
            DevicePropertyCommand = new RelayCommand<ICameraDevice>(x => ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.CameraPropertyWnd_Show, x));
        }

        public static void SelectCamera(ICameraDevice cameraDevice)
        {
            ServiceProvider.DeviceManager.SelectedCameraDevice = cameraDevice;
        }


        // ref http://msdn.microsoft.com/en-us/library/vstudio/ee230087%28v=vs.100%29.aspx
        public static void DisableWpfTabletSupport()
        {
            // Get a collection of the tablet devices for this window.  
            TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            if (devices.Count > 0)
            {
                // Get the Type of InputManager.
                Type inputManagerType = typeof(System.Windows.Input.InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.
                object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.
                    Type stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.
                    while (devices.Count > 0)
                    {
                        // Remove the first tablet device in the devices collection.
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                                null, stylusLogic, new object[] { (uint)0 });
                    }
                }

            }
        }
    }
}
