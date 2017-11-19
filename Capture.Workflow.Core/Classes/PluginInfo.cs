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
        private string _icon;

        [XmlAttribute]
        public PluginType Type { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlIgnore]
        public string Description { get; set; }
        [XmlAttribute]
        public string Class { get; set; }
        [XmlAttribute]
        public string Assembly { get; set; }

        [XmlIgnore]
        public string Icon
        {
            get
            {
                if (string.IsNullOrEmpty(_icon))
                    return "Play";
                return _icon;
            }
            set { _icon = value; }
        }

    }
}

