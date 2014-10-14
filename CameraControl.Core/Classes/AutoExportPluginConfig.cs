using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class AutoExportPluginConfig
    {
        public string Type { get; set; }
        public ValuePairEnumerator ConfigData { get; set; }
        public bool IsEnabled { get; set; }
    }
}
