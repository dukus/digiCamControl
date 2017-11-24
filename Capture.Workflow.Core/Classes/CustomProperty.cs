using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Capture.Workflow.Core.Annotations;
using SmartFormat;

namespace Capture.Workflow.Core.Classes
{
    [XmlType(TypeName = "Property")]
    public class CustomProperty: INotifyPropertyChanged
    {
        private string _value;

        [XmlAttribute]
        public string Name { get; set; }
        [XmlIgnore]
        public string Description { get; set; }

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
        public string Parameter { get; set; }
        [XmlIgnore]
        public CustomPropertyType PropertyType { get; set; }
        [XmlIgnore]
        public List<string> ValueList { get; set; }
        [XmlIgnore]
        public double RangeMin { get; set; }
        [XmlIgnore]
        public double RangeMax { get; set; }
        
        public CustomProperty()
        {
            ValueList = new List<string>();
            RangeMin = 0;
            RangeMax = 0;
        }

        public int ToInt(Context context)
        {
            int val = 0;
            var strVal = ToString(context);
            if (IsPercentage(context))
                strVal = strVal.Replace("%", "");
            int.TryParse(strVal, out val);
            return val;
        }

        public bool IsPercentage(Context context)
        {
            return ToString(context).Trim().EndsWith("%");
        }

        public string ToString(Context context)
        {
            if (context?.WorkFlow == null)
                return "";
            if (string.IsNullOrWhiteSpace(Value))
                return Value;
            Smart.Default.Settings.ConvertCharacterStringLiterals = false;
            return Smart.Format(Value, context.WorkFlow.Variables.GetAsDictionary()); 
        }

        public bool ToBool(Context context)
        {
            bool val;
            bool.TryParse(ToString(context), out val);
            return val;
        }

        public void InitVaribleList()
        {
            ValueList = WorkflowManager.Instance.Context.WorkFlow.Variables.Items.Select(x => x.Name).ToList();
            ValueList.Insert(0, "");
        }

        public void InitViewList()
        {
            ValueList = WorkflowManager.Instance.Context.WorkFlow.Views.Select(x => x.Name).ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Name + " = " + Value;
        }
    }
}
