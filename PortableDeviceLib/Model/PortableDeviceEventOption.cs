using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableDeviceLib.Model
{
    /// <summary>
    /// Represent an option
    /// </summary>
    public class PortableDeviceEventOption
    {
        #region Properties

        /// <summary>
        /// Gets or sets the guid
        /// </summary>
        public Guid Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value type
        /// </summary>
        public TypeCode ValueType
        {
            get;
            set;
        }

        #endregion
    }
}
