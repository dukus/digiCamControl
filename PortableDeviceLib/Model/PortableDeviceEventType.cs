using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableDeviceLib.Model
{
    /// <summary>
    /// Represent the base class for event type
    /// </summary>
    public class PortableDeviceEventType
    {
      public PortableDeviceObject DeviceObject { get; set; }
      public Guid EventGuid { get; set; }
      public uint ObjectHandle { get; set; }
      public bool IsInSdram { get; set; }
    }
}
