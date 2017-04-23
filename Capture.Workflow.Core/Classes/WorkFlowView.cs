using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core.Interface;
using Newtonsoft.Json;

namespace Capture.Workflow.Core.Classes
{
    public class WorkFlowView : BaseItem
    {
        [XmlIgnore]
        [JsonIgnore]
        public IViewPlugin Instance { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public WorkFlow Parent { get; set; }

        public AsyncObservableCollection<WorkFlowViewElement> Elements { get; set; }
        public List<CommandCollection> Events { get; set; }

        public WorkFlowView()
        {
            Properties = new CustomPropertyCollection();
            Elements = new AsyncObservableCollection<WorkFlowViewElement>();
            Events = new List<CommandCollection>();
        }

        public CommandCollection GetEventCommands(string name)
        {
            foreach (var collection in Events)
            {
                if (collection.Name == name)
                    return collection;
            }
            return null;
        }
    }
}
