using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class AutoExportPluginConfig
    {
        [XmlAttribute]
        public string Type { get; set; }
        public ValuePairEnumerator ConfigData { get; set; }
        [XmlAttribute]
        public bool IsEnabled { get; set; }
        [XmlAttribute]
        public string Name { get; set; }

        public string DisplayName
        {
            get { return string.IsNullOrEmpty(Name) ? Type : Name; }
        }

        public AutoExportPluginConfig(IAutoExportPlugin plugin)
        {
            Type = plugin.Name;
            IsEnabled = true;
            ConfigData = new ValuePairEnumerator();
        }

        public AutoExportPluginConfig()
        {
            ConfigData = new ValuePairEnumerator();
        }
    }
}
