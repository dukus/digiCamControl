#region License
/*
PortableDeviceObject.cs
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

namespace PortableDeviceLib.Model
{
    /// <summary>
    /// Represent an objet in a WPD
    /// </summary>
    public class PortableDeviceObject
    {

        /// <summary>
        /// Initialize a new instance of the <see cref="PortableDeviceObject"/> class
        /// </summary>
        /// <param name="id"></param>
        public PortableDeviceObject(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            this.ID = id;
        }

        /// <summary>
        /// Gets the ID of the object
        /// </summary>
        public string ID
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the name of the object
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the content type
        /// </summary>
        public string ContentType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the format
        /// </summary>
        public string Format
        {
            get;
            internal set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
                return this.ID;
            else
                return this.Name;
        }

    }
}
