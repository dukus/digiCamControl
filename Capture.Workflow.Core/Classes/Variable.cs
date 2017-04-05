using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Capture.Workflow.Core.Classes
{
    public class Variable
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        [XmlAttribute]
        public string DefaultValue { get; set; }
        [XmlAttribute]
        public bool Reinit { get; set; }
        [XmlAttribute]
        public bool Editable { get; set; }

        public Variable()
        {
            Editable = true;
            Value = "";
            DefaultValue = "";
        }
    }
}
