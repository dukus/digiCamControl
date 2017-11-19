using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes.Attributes
{
    public class IconAttribute: Attribute
    {
        public string Icon { get; set; }

        public IconAttribute()
        {
            
        }

        public IconAttribute(string icon)
        {
            Icon = icon;
        }
    }
}
