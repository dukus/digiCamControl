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
    public class WorkFlowViewElement: BaseItem
    {
        [XmlIgnore]
        [JsonIgnore]
        public IViewElementPlugin Instance { get; set; }

        public WorkFlowViewElement()
        {
            Properties = new CustomPropertyCollection();
        }
    }
}
