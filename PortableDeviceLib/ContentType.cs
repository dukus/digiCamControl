#region License
/*
ContentType.cs
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
    /// 
    /// </summary>
    public class ContentType
    {

        private Dictionary<Guid, string> formats;

        /// <summary>
        /// Initialize a new instance of the <see cref="ContentType"/> class
        /// </summary>
        /// <param name="portableDeviceClass"></param>
        /// <param name="guid"></param>
        /// <param name="name"></param>
        internal ContentType(PortableDeviceClass portableDeviceClass, Guid guid, string name)
        {
            if (guid == Guid.Empty)
                throw new Exception("guid cann't be Guid.Empty");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            formats = new Dictionary<Guid, string>();
            this.Guid = guid;
            this.Name = name;

            this.ExtractSupportedFormat(portableDeviceClass, guid);
        }

        /// <summary>
        /// Gets or sets the guid of the content type
        /// </summary>
        public Guid Guid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the name of the content type
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the supported format
        /// </summary>
        public IEnumerable<string> SupportedFormats
        {
            get
            {
                return formats.Values;
            }
        }

        private void ExtractSupportedFormat(PortableDeviceClass portableDeviceClass, Guid contentType)
        {
            if (portableDeviceClass == null)
                throw new PortableDeviceException("");

            PortableDeviceApiLib.IPortableDeviceCapabilities capabilities;
            portableDeviceClass.Capabilities(out capabilities);

            if (capabilities == null)
            {
                System.Diagnostics.Trace.WriteLine("Cannot extract capabilities from device");
                throw new PortableDeviceException("Cannot extract capabilities from device");
            }


            PortableDeviceApiLib.IPortableDeviceValues pValues = (PortableDeviceApiLib.IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();


            //Functional objects variables
            IPortableDevicePropVariantCollection formats;
            uint countObjects = 1;
            tag_inner_PROPVARIANT values = new tag_inner_PROPVARIANT();
            string formatName;
            Guid currentFormat;
            capabilities.GetSupportedFormats(ref contentType, out formats);

            formats.GetCount(ref countObjects);
            for (uint i = 0; i < countObjects; i++)
            {
                formats.GetAt(i, ref values);

                pValues.SetValue(ref PortableDevicePKeys.WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_FORMATS, ref values);
                pValues.GetStringValue(ref PortableDevicePKeys.WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_FORMATS, out formatName);
                currentFormat = new Guid(formatName);
                this.formats.Add(currentFormat, PortableDeviceHelpers.GetKeyNameFromGuid(currentFormat));
            }

        }
    }
}
