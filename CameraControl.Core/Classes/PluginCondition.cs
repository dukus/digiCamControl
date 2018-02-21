using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CameraControl.Core.Scripting;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

namespace CameraControl.Core.Classes
{
    public class PluginCondition : BaseFieldClass
    {
        private AsyncObservableCollection<string> _variableList;
        private string _variable;
        private AsyncObservableCollection<string> _valueList;
        private string _value;
        public string Operator { get; set; }

        public string Variable
        {
            get { return _variable; }
            set
            {
                _variable = value;
                NotifyPropertyChanged("ValueList");
            }
        }

        public string Condition { get; set; }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public AsyncObservableCollection<string> OperatorList { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public AsyncObservableCollection<string> VariableList
        {
            get
            {
                //if (_variableList == null)
                InitVariableList();
                return _variableList;
            }
        }
        
        [XmlIgnore]
        [JsonIgnore]
        public AsyncObservableCollection<string> ConditionList { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public AsyncObservableCollection<string> ValueList
        {
            get
            {
                InitValueList();
                return _valueList;
            }
            set
            {
                _valueList = value;
                NotifyPropertyChanged("ValueList");
            }
        }

        public PluginCondition()
        {
            OperatorList = new AsyncObservableCollection<string>() {"AND", "OR"};
            ConditionList = new AsyncObservableCollection<string>() { ">=","<=","!=","=","<",">","%"};
            ValueList = new AsyncObservableCollection<string>();
            Operator = "AND";
            Condition = "=";
        }

        private void InitVariableList()
        {
            _variableList = new AsyncObservableCollection<string>();
            _variableList.Add("camera");
            var processor = new CommandLineProcessor();
            var resp = processor.Pharse(new[] { "list", "camera" });
            var list = resp as IEnumerable<string>;
            if (list != null)
            {
                foreach (string s in list)
                {
                    _variableList.Add(s.Split('=')[0]);
                }
            }
            resp = processor.Pharse(new[] { "list", "property" });
            list = resp as IEnumerable<string>;
            if (list != null)
            {
                foreach (string s in list)
                {
                    _variableList.Add(s.Split('=')[0]);
                }
            }
            resp = processor.Pharse(new[] { "list", "session" });
            list = resp as IEnumerable<string>;
            if (list != null)
            {
                foreach (string s in list)
                {
                    _variableList.Add(s.Split('=')[0]);
                }
            }            
        }

        private void InitValueList()
        {
            _valueList = new AsyncObservableCollection<string>();
            try
            {
                if (string.IsNullOrEmpty(Variable))
                    return;
                var processor = new CommandLineProcessor();
                if (Variable == "camera")
                {
                    _valueList = new AsyncObservableCollection<string>(processor.Pharse(new[] { "list", "cameras" }) as IEnumerable<string>);
                    return;
                }
                _valueList = new AsyncObservableCollection<string>(processor.Pharse(new[] { "list", Variable }) as IEnumerable<string>);
            }
            catch (Exception)
            {
            }
        }

        public bool Evaluate(ICameraDevice device)
        {
            bool cond = true;

            if (string.IsNullOrWhiteSpace(Value) || string.IsNullOrWhiteSpace(Variable) ||
                string.IsNullOrWhiteSpace(Condition))
                return true;
            
            string op = Condition;

            var processor = new CommandLineProcessor(){TargetDevice = device};
            var resp = processor.Pharse(new[] { "get",Variable });

            string left = resp.ToString();
            string right = Value;

            if (string.IsNullOrEmpty(left))
                return true;

            float leftNum = 0;
            float rightNum = 0;

            bool numeric = float.TryParse(left, out leftNum);
            numeric = numeric && float.TryParse(right, out rightNum);

            // try to process our test

            if (op == ">=")
            {
                if (numeric) cond = leftNum >= rightNum;
                else cond = String.Compare(left, right, StringComparison.Ordinal) >= 0;
            }
            else if (op == "<=")
            {
                if (numeric) cond = leftNum <= rightNum;
                else cond = String.Compare(left, right, StringComparison.Ordinal) <= 0;
            }
            else if (op == "!=")
            {
                if (numeric) cond = leftNum != rightNum;
                else cond = String.Compare(left, right, StringComparison.Ordinal) != 0;
            }
            else if (op == "=")
            {
                if (numeric) cond = leftNum == rightNum;
                else cond = String.Compare(left, right, StringComparison.Ordinal) == 0;
            }
            else if (op == "<")
            {
                if (numeric) cond = leftNum < rightNum;
                else cond = String.Compare(left, right, StringComparison.Ordinal) < 0;
            }
            else if (op == ">")
            {
                if (numeric) cond = leftNum > rightNum;
                else cond = String.Compare(left, right, StringComparison.Ordinal) > 0;
            }
            else if (op == "%")
            {
                if (numeric) cond = (int) leftNum%(int) rightNum == 0;
                else cond = false;
            }
            else
            {
                Log.Error("Wrong operator :" + op);
                return true;
            }
            return cond;
        }
    }
}
