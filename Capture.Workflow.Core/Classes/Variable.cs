using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes
{
    public class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public bool Reinit { get; set; }
        public bool Global { get; set; }
    }
}
