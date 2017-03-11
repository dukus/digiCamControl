using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes.Attributes
{
    public class PluginTypeAttribute: Attribute
    {
        public PluginType PluginType { get; set; }

        public PluginTypeAttribute()
        {
            
        }

        public PluginTypeAttribute(PluginType type)
        {
            PluginType = type;
        }
    }
}
