#region License
/*
FunctionalCategory.cs
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
    /// Represent a functional category and supported content type
    /// </summary>
    public class FunctionalCategory
    {

        private Dictionary<Guid, ContentType> contentTypes;

        /// <summary>
        /// Initialize a new instance of the <see cref="FunctionalCategory"/> class
        /// </summary>
        /// <param name="portableDeviceClass"></param>
        /// <param name="guid"></param>
        /// <param name="name"></param>
        internal FunctionalCategory(PortableDeviceClass portableDeviceClass, Guid guid, string name)
        {
            if (guid == Guid.Empty)
                throw new ArgumentException("guid cann't be Guid.Empty");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            this.contentTypes = new Dictionary<Guid, ContentType>();

            this.Guid = guid;
            this.Name = name;

            this.ExtractContentType(portableDeviceClass, guid);
        }

        /// <summary>
        /// Get the guid of this functional category
        /// You can use it with PortableDeviceCabilities
        /// </summary>
        public Guid Guid
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the name of this functional category
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Get all content types in this category
        /// </summary>
        public IEnumerable<ContentType> ContentTypes
        {
            get
            {
                return this.contentTypes.Values;
            }
        }

        /// <summary>
        /// <see cref="System.Object.ToString()"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Name))
                return this.Name;
            else
                return this.Guid.ToString();
        }

        private void ExtractContentType(PortableDeviceClass portableDeviceClass, Guid functionalCategory)
        {
            if (portableDeviceClass == null)
                throw new ArgumentNullException("portableDeviceClass");

            try
            {
                PortableDeviceApiLib.IPortableDeviceCapabilities capabilities;
                portableDeviceClass.Capabilities(out capabilities);

                if (capabilities == null)
                {
                    System.Diagnostics.Trace.WriteLine("Cannot extract capabilities from device");
                    throw new PortableDeviceException("Cannot extract capabilities from device");
                }


                PortableDeviceApiLib.IPortableDeviceValues pValues = (PortableDeviceApiLib.IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();


                //Functional objects variables
                IPortableDevicePropVariantCollection contentTypes;
                uint countObjects = 1;
                tag_inner_PROPVARIANT values = new tag_inner_PROPVARIANT();
                string contentTypeName;
                Guid currentContentTypeGuid;
                capabilities.GetSupportedContentTypes(ref functionalCategory, out contentTypes);

                contentTypes.GetCount(ref countObjects);
                for (uint i = 0; i < countObjects; i++)
                {
                    contentTypes.GetAt(i, ref values);

                    pValues.SetValue(ref PortableDevicePKeys.WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_CONTENT_TYPES, ref values);
                    pValues.GetStringValue(ref PortableDevicePKeys.WPD_COMMAND_CAPABILITIES_GET_SUPPORTED_CONTENT_TYPES, out contentTypeName);
                    currentContentTypeGuid = new Guid(contentTypeName);
                    this.contentTypes.Add(currentContentTypeGuid, new ContentType(
                        portableDeviceClass,
                        currentContentTypeGuid,
                        PortableDeviceHelpers.GetKeyNameFromGuid(currentContentTypeGuid)));
                }

            }
            catch (Exception ex)
            {
                throw new PortableDeviceException("Error on extract functional object", ex);
            }
        }

    }
}
