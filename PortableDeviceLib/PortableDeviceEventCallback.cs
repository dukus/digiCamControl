using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceApiLib;
using PortableDeviceLib.Factories;
using PortableDeviceLib.Model;

namespace PortableDeviceLib
{
    /// <summary>
    /// Manage event from device
    /// </summary>
    internal class PortableDeviceEventCallback : IPortableDeviceEventCallback
    {
        private PortableDevice device;

        /// <summary>
        /// Initialize an new instance of the <see cref="PortableDeviceEventCallback"/> class
        /// </summary>
        /// <param name="portableDevice"></param>
        internal PortableDeviceEventCallback(PortableDevice portableDevice)
        {
            if (portableDevice == null)
                throw new ArgumentNullException("portableDevice");

            this.device = portableDevice;
        }

        /// <summary>
        /// Callback for event from device
        /// </summary>
        /// <param name="pEventParameters"></param>
        public void OnEvent(IPortableDeviceValues pEventParameters)
        {
          string pnpDeviceId;
          pEventParameters.GetStringValue(ref PortableDevicePKeys.WPD_EVENT_PARAMETER_PNP_DEVICE_ID, out pnpDeviceId);
          if (this.device.DeviceId != pnpDeviceId)
            return;

          Guid eventGuid;
          pEventParameters.GetGuidValue(ref PortableDevicePKeys.WPD_EVENT_PARAMETER_EVENT_ID, out eventGuid);

          PortableDeviceEventType deviceEventType = new PortableDeviceEventType() {EventGuid = eventGuid};

          if (eventGuid == PortableDeviceGuids.WPD_EVENT_OBJECT_ADDED)
          {
            string objectId;
            pEventParameters.GetStringValue(ref PortableDevicePKeys.WPD_OBJECT_ID, out objectId);
            string objectName;
            pEventParameters.GetStringValue(ref PortableDevicePKeys.WPD_OBJECT_NAME, out objectName);
            PortableDeviceObject deviceObject = new PortableDeviceObject(objectId) {Name = objectName};
            deviceEventType.DeviceObject = deviceObject;
          }

          // the original api isn't finise, i use a siple workaroud, but this need to be fixed using event factory
          //this.device.RaiseEvent(PortableDeviceEventTypeFactory.Instance.CreateEventType(eventGuid));
          this.device.RaiseEvent(deviceEventType);
        }
    }
}
