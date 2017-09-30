using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.VariableChanged, this));
            }
        }

        [XmlAttribute]
        public string DefaultValue { get; set; }
        [XmlAttribute]
        public bool Reinit { get; set; }
        [XmlAttribute]
        public bool Editable { get; set; }
        [XmlAttribute]
        public VariableTypeEnum VariableType { get; set; }

        public Variable()
        {
            Editable = true;
            Value = "";
            DefaultValue = "";
            VariableType = VariableTypeEnum.String;
            Reinit = true;
        }

        public object GetAsObject()
        {
            switch (VariableType)
            {
                case VariableTypeEnum.String:
                    return Value;
                case VariableTypeEnum.Number:
                {
                    double d = 0;
                    if (double.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    {
                        return d;
                    }
                    break;
                }
                case VariableTypeEnum.Date:
                {
                    DateTime dateTime;
                    if (DateTime.TryParse(Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal,
                        out dateTime))
                    {
                        return dateTime;
                    }
                    break;
                }
            }
            return Value;
            
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Name +" = "+Value;
        }
    }
}
