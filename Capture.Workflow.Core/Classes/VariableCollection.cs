using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;

namespace Capture.Workflow.Core.Classes
{
    public class VariableCollection
    {
        [XmlElement("Variable")]
        public AsyncObservableCollection<Variable> Items { get; set; }

        public Variable this[string name]
        {
            get
            {
                foreach (var item in Items)
                {
                    if (item.Name.ToLower() == name.ToLower())
                        return item;
                }
                return null;
            }
        }

        public VariableCollection()
        {
            Items = new AsyncObservableCollection<Variable>();
        }

        public Dictionary<string, object> GetAsDictionary()
        {
            return Items.ToDictionary(variable => variable.Name, variable => variable.GetAsObject());
        }

        public VariableCollection GetItemVariables()
        {
            var res = new VariableCollection();
            foreach (var item in Items)
            {
                if (item.ItemVariable)
                {
                    var val = new Variable()
                    {
                        Name = item.Name,
                        Value = item.Value,
                        VariableType = item.VariableType,
                    };
                    res.Items.Add(val);
                    item.AttachedVariable = val;
                }
            }
            return res;
        }

        public bool SetValue(string name, string value)
        {
            var s = this[name];
            if (s == null) return false;
            s.Value = value;
            return true;
        }
    }
}
