#region License
/*
PortableDeviceException.cs
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
using System.Runtime.Serialization;

namespace PortableDeviceLib
{
    public class PortableDeviceException : Exception
    {

        public PortableDeviceException() :
            base()
        { }

        public PortableDeviceException(string message) :
            base(message)
        { }

        public PortableDeviceException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public PortableDeviceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }


    }
}
