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
    public class WorkFlowCommand : BaseItem
    {
        [XmlIgnore]
        [JsonIgnore]
        public IWorkflowCommand Instance { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public WorkFlow Parent { get; set; }

        public WorkFlowCommand()
        {
            Properties = new CustomPropertyCollection();
            Properties.PropertyChanged += Properties_PropertyChanged;
        }

        private void Properties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Name));
        }
    }
}
