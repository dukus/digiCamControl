using System;
using System.Xml.Serialization;
using CameraControl.Devices;
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
            Properties.PropertyChanged += Properties_PropertyChanged;
        }

        private void Properties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Name));
        }
    }
}
