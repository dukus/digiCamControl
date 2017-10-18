using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes.Attributes
{
    public class GroupAttribute: Attribute
    {
        public string Group { get; set; }
        public GroupAttribute()
        {
            
        }

        public GroupAttribute(string group)
        {
            Group = group;
        }


    }
}
