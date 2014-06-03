using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableDeviceLib.Model
{
    /// <summary>
    /// Represent a folder
    /// </summary>
    public class PortableDeviceFolderObject : PortableDeviceContainerObject
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="PortableDeviceFolderObject"/>
        /// </summary>
        /// <param name="id"></param>
        public PortableDeviceFolderObject(string id)
            : base(id)
        {

        }

        #endregion
    }
}
