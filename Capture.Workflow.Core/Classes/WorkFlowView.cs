using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core.Interface;
using Newtonsoft.Json;

namespace Capture.Workflow.Core.Classes
{
    public class WorkFlowView : BaseItem, INotifyPropertyChanged
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
            Properties.PropertyChanged += Properties_PropertyChanged;
        }

        private void Properties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Name));
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
