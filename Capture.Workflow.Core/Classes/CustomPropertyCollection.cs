using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Capture.Workflow.Core.Classes
{
    public class CustomPropertyCollection
    {
        [XmlElement("CustomProperty")]
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

        public void Add(CustomProperty property)
        {
            Items.Add(property);
        }

        public CustomPropertyCollection()
        {
            Items = new List<CustomProperty>();
        }

        public void CopyValuesFrom(CustomPropertyCollection propertyCollection)
        {
            foreach (var property in propertyCollection.Items)
            {
                this[property.Name].Value = property.Value;
            }
        }
    }
}
