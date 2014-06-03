#region License
/*
PortableDeviceCollection.cs
Copyright (C) 2009 Vincent Lainé
 
This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceApiLib;

namespace PortableDeviceLib
{

    /// <summary>
    /// Manage a collection of <see cref="PortableDeviceLib.PortableDevice"/> from the WPD API Interop
    /// </summary>
    public class PortableDeviceCollection
    {
        private static PortableDeviceCollection _instance;

        private PortableDeviceManagerClass deviceManager;
        private uint countDevices = 0;
        private Dictionary<string, PortableDevice> portableDevices;

        private string appName;
        private float majorVersion;
        private float minorVersion;

        /// <summary>
        /// Private default constructor
        /// </summary>
        private PortableDeviceCollection(string appName, float majorVersion, float minorVersion)
        {
            if (string.IsNullOrEmpty(appName))
                throw new ArgumentNullException("appName");

            this.deviceManager = new PortableDeviceManagerClass();

            this.portableDevices = new Dictionary<string, PortableDevice>();

            this.appName = appName;
            this.majorVersion = majorVersion;
            this.minorVersion = minorVersion;
            this.AutoConnectToPortableDevice = true;
        }

        #region Properties

        /// <summary>
        /// Get or set a value that indicate if we autoconnect to PortableDevice when creating a new instance
        /// </summary>
        public bool AutoConnectToPortableDevice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of PortableDevice connected
        /// </summary>
        public int Count
        {
            get
            {
                RefreshDevices();
                return Convert.ToInt32(countDevices);
            }
        }

        /// <summary>
        /// Get the device ID list
        /// </summary>
        public IEnumerable<string> DeviceIds
        {
            get
            {
                return InternalGetDeviceIds();
            }
        }

        /// <summary>
        /// Get the list of devices
        /// </summary>
        public IEnumerable<PortableDevice> Devices
        {
            get
            {
                if (DeviceIds == null)
                    yield break;

                foreach (string id in DeviceIds)
                    yield return GetPortableDeviceById(id);
            }
        }

        /// <summary>
        /// Gets or sets the instance of PortableDeviceCollection
        /// </summary>
        public static PortableDeviceCollection Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Get the portable device by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PortableDevice GetPortableDeviceById(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            if (!portableDevices.ContainsKey(id))
            {
                PortableDevice portableDevice = new PortableDevice(id);
                if( AutoConnectToPortableDevice )
                    portableDevice.ConnectToDevice(appName, majorVersion, minorVersion);
                portableDevices.Add(id, portableDevice);
            }

            return this.portableDevices[id];
        }


      /// <summary>
      /// remove a disconected device, this should be call after device disconnected
      ///  </summary>
      /// <param name="id">Device ide of the disconnected device</param>
        public void RemoveDevice(string id)
        {
          if(portableDevices.ContainsKey(id))
          {
            portableDevices.Remove(id);
          }
        }

      /// <summary>
        /// Create the unique instance of PortableDeviceCollection
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <returns></returns>
        public static PortableDeviceCollection CreateInstance(string appName, float majorVersion, float minorVersion)
        {
            if (string.IsNullOrEmpty(appName))
                throw new ArgumentNullException("appName");

            if (_instance == null)
                _instance = new PortableDeviceCollection(appName, majorVersion, minorVersion);

            return _instance;
        }

        public void RefreshDevices()
        {
          this.deviceManager.RefreshDeviceList();
          uint _countDevices = 1;
          this.deviceManager.GetDevices(null, ref _countDevices);
          string[] devicesIds = new string[_countDevices];
          this.deviceManager.GetDevices(devicesIds, ref _countDevices);
          this.countDevices = _countDevices;
          List<string> removedDevices =
            (from portableDevice in portableDevices
             where devicesIds.Contains(portableDevice.Key)
             select portableDevice.Key).ToList();
          foreach (string removedDevice in removedDevices)
          {
            RemoveDevice(removedDevice);
          }
        }

      private string[] InternalGetDeviceIds()
        {
            RefreshDevices();
            if (this.countDevices <= 0)
                return null;

            string[] deviceIds = new string[this.countDevices];
            this.deviceManager.GetDevices(deviceIds, ref this.countDevices);            

            return deviceIds;
        }

    }
}
