using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Capture.Workflow.Core.Classes
{
    public class PluginInfo
    {
        [XmlAttribute]
        public PluginType Type { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlAttribute]
        public string Class { get; set; }
        [XmlAttribute]
        public string Assembly { get; set; }
    }
}

