using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;

namespace Capture.Workflow.Core.Classes
{
    public class VariableCollection
    {
        [XmlElement("Variable")]
        public AsyncObservableCollection<Variable> Items { get; set; }

        public Variable this[string name]
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

        public VariableCollection()
        {
            Items = new AsyncObservableCollection<Variable>();
        }

    }
}
