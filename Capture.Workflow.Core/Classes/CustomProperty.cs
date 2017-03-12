using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes
{
    public class CustomProperty
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Parameter { get; set; }
        public CustomPropertyType PropertyType { get; set; }
        public List<string> ValueList { get; set; }
        public double RangeMin { get; set; }
        public double RangeMax { get; set; }

        public CustomProperty()
        {
            ValueList = new List<string>();
        }
    }
}
