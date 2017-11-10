using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Capture.Workflow.Core.Annotations;

namespace Capture.Workflow.Core.Classes
{
    public class Variable:INotifyPropertyChanged
    {
        private string _value;
        private string _name;
        private Variable _attachedVariable;

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [XmlAttribute]
        public string Value
        {
            get
            {
                if (AttachedVariable != null)
                    return AttachedVariable.Value;
                return _value;
            }
            set
            {
                _value = value;
                if (AttachedVariable != null)
                    AttachedVariable.Value = _value;
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
        public bool ItemVariable { get; set; }
        [XmlAttribute]
        public VariableTypeEnum VariableType { get; set; }

        [XmlIgnore]
        public Variable AttachedVariable
        {
            get { return _attachedVariable; }
            set
            {
                _attachedVariable = value;
                OnPropertyChanged(nameof(AttachedVariable));
                OnPropertyChanged(nameof(Value));
            }
        }


        public Variable()
        {
            Editable = true;
            Value = "";
            DefaultValue = "";
            VariableType = VariableTypeEnum.String;
            Reinit = false;
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
                case VariableTypeEnum.Boolean:
                {
                    return Value != null && Value.ToLower().Trim() == "true";
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
