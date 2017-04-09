using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;

namespace Capture.Workflow.Core.Classes
{
    public class CommandCollection
    {
        [XmlElement("Command")]
        public AsyncObservableCollection<WorkFlowCommand> Items { get; set; }

        public string Name { get; set; }
        
        public WorkFlowCommand this[string name]
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


        public CommandCollection()
        {
            Items = new AsyncObservableCollection<WorkFlowCommand>();
        }

        public CommandCollection(string name)
        {
            Items = new AsyncObservableCollection<WorkFlowCommand>();
            Name = name;
        }
    }
}
