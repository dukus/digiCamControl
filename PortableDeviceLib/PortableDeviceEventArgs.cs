using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceLib.Model;

namespace PortableDeviceLib
{
    /// <summary>
    /// Event arguments for the <see cref="PortableDevice.DeviceEventSended"/> event
    /// </summary>
    public class PortableDeviceEventArgs : EventArgs
    {
        #region Constructors
        
        /// <summary>
        /// Initialize a new instance of the <see cref="PortableDeviceEventArgs"/> class
        /// </summary>
        public PortableDeviceEventArgs()
            : base()
        {

        }

        /// <summary>
        /// Initialize a new instance of the <see cref="PortableDeviceEventArgs"/> class
        /// </summary>
        /// <param name="eventType">The event type</param>
        public PortableDeviceEventArgs(PortableDeviceEventType eventType)
            : this()
        {
            this.EventType = eventType;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the event guid.
        /// </summary>
        public PortableDeviceEventType EventType
        {
            get;
            set;
        }

        #endregion
    }
}
