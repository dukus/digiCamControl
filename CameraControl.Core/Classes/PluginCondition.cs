using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CameraControl.Core.Scripting;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class PluginCondition : BaseFieldClass
    {
        private AsyncObservableCollection<string> _variableList;
        public string Operator { get; set; }
        public string Variable { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }
        
        [XmlIgnore]
        public AsyncObservableCollection<string> OperatorList { get; set; }

        [XmlIgnore]
        public AsyncObservableCollection<string> VariableList
        {
            get
            {
                if (_variableList == null)
                    InitVariableList();
                return _variableList;
            }
        }
        
        [XmlIgnore]
        public AsyncObservableCollection<string> ConditionList { get; set; }
        
        [XmlIgnore]
        public AsyncObservableCollection<string> ValueList { get; set; }

        public PluginCondition()
        {
            OperatorList = new AsyncObservableCollection<string>() {"AND", "OR"};
            ConditionList = new AsyncObservableCollection<string>() { ">=","<=","!=","=","<",">" };
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
        }
    }
}
