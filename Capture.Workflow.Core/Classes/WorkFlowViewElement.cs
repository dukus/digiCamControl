using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Capture.Workflow.Core.Annotations;
using Capture.Workflow.Core.Interface;
using Newtonsoft.Json;

namespace Capture.Workflow.Core.Classes
{
    public class WorkFlowViewElement: BaseItem,INotifyPropertyChanged
    {
        [XmlIgnore]
        [JsonIgnore]
        public IViewElementPlugin Instance { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public WorkFlowView Parent { get; set; }

        public List<CommandCollection> Events { get; set; }

        public WorkFlowViewElement()
        {
            Properties = new CustomPropertyCollection();
            Events = new List<CommandCollection>();
            Properties.PropertyChanged += Properties_PropertyChanged;
        }

        private void Properties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
