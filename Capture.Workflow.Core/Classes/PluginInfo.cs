using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes
{
    public class PluginInfo
    {
        public PluginType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Class { get; set; }
    }
}
