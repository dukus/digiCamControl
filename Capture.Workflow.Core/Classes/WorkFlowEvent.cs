using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Capture.Workflow.Core.Interface;
using Newtonsoft.Json;

namespace Capture.Workflow.Core.Classes
{
    public class WorkFlowEvent: BaseItem
    {
        [XmlIgnore]
        [JsonIgnore]
        public IEventPlugin Instance { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public WorkFlow Parent { get; set; }

        public CommandCollection CommandCollection { get; set; }

        public WorkFlowEvent()
        {
            CommandCollection = new CommandCollection();
            Properties = new CustomPropertyCollection();
        }
    }
}
