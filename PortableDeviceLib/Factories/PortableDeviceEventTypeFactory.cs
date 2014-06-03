using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceLib.Model;

namespace PortableDeviceLib.Factories
{
    /// <summary>
    /// Represent a factory that can construct new <see cref="PortableDeviceEventType"/> object
    /// </summary>
    public class PortableDeviceEventTypeFactory
    {
        private static PortableDeviceEventTypeFactory instance;

        private Dictionary<Guid, Func<PortableDeviceEventType>> factoryMethods;

        private PortableDeviceEventTypeFactory()
        {
            this.factoryMethods = new Dictionary<Guid, Func<PortableDeviceEventType>>();
        }

        #region Properties

        /// <summary>
        /// Gets the unique instance of the <see cref="PortableDeviceEventTypeFactory"/>
        /// </summary>
        public static PortableDeviceEventTypeFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new PortableDeviceEventTypeFactory();
                return instance;
            }
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Create a new instance of the 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public PortableDeviceEventType CreateEventType(Guid guid)
        {
            if (this.factoryMethods.ContainsKey(guid))
                return this.factoryMethods[guid]();
            else
                return new PortableDeviceEventType();
        }

        #endregion

    }
}
