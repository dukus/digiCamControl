using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CameraControl.Devices.Custom
{
    public class DevicePropertyValue
    {
        [XmlAttribute]
        public long Value { get; set; }
        [XmlAttribute]
        public string Name { get; set; }

    }
}
