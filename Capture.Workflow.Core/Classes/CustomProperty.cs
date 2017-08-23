using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Capture.Workflow.Core.Classes
{
    [XmlType(TypeName = "Property")]
    public class CustomProperty
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlIgnore]
        public string Description { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        [XmlAttribute]
        public string Parameter { get; set; }
        [XmlIgnore]
        public CustomPropertyType PropertyType { get; set; }
        [XmlIgnore]
        public List<string> ValueList { get; set; }
        [XmlIgnore]
        public double RangeMin { get; set; }
        [XmlIgnore]
        public double RangeMax { get; set; }

        public CustomProperty()
        {
            ValueList = new List<string>();
            RangeMin = 0;
            RangeMax = 0;
        }

        public int ToInt()
        {
            int val = 0;
            int.TryParse(Value,  out val);
            return val;
        }

        public bool ToBool()
        {
            bool val;
            bool.TryParse(Value, out val);
            return val;
        }

        public void InitVaribleList()
        {
            ValueList = WorkflowManager.Instance.Context.WorkFlow.Variables.Items.Select(x => x.Name).ToList();
        }

        public void InitViewList()
        {
            ValueList = WorkflowManager.Instance.Context.WorkFlow.Views.Select(x => x.Name).ToList();
        }
    }
}
