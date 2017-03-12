using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture.Workflow.Core.Classes
{
    public class CustomPropertyCollection
    {
        public List<CustomProperty> Items { get; set; }

        public CustomProperty this[string name]
        {
            get
            {
                foreach (var item in Items)
                {
                    if (item.Name == name)
                        return item;
                }
                return null;
            }
        }
        
        public CustomPropertyCollection()
        {
            Items = new List<CustomProperty>();
        }
    }
}
