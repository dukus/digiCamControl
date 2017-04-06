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

        public WorkFlowView()
        {
            Properties = new CustomPropertyCollection();
            Elements = new AsyncObservableCollection<WorkFlowViewElement>();
        }
    }
}
