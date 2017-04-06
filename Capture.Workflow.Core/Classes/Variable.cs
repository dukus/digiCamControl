using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Capture.Workflow.Core.Annotations;

namespace Capture.Workflow.Core.Classes
{
    public class Variable:INotifyPropertyChanged
    {
        private string _value;

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        [XmlAttribute]
        public string DefaultValue { get; set; }
        [XmlAttribute]
        public bool Reinit { get; set; }
        [XmlAttribute]
        public bool Editable { get; set; }

        public Variable()
        {
            Editable = true;
            Value = "";
            DefaultValue = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
