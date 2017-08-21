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
                    if (item.Name == name)
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
            var res = new Dictionary<string, object>();
            foreach (Variable variable in Items)
            {
                switch (variable.VariableType)
                {
                    case VariableTypeEnum.String:
                        res.Add(variable.Name, variable.Value);
                        break;
                    case VariableTypeEnum.Number:
                        {
                            double d = 0;
                            if (double.TryParse(variable.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                            {
                                res.Add(variable.Name, d);
                            }
                            else
                            {
                                res.Add(variable.Name, variable.Value);
                            }
                        }
                        break;
                    case VariableTypeEnum.Date:
                        {
                            DateTime dateTime;
                            if (DateTime.TryParse(variable.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal,
                                out dateTime))
                            {
                                res.Add(variable.Name, dateTime);
                            }
                            else
                            {
                                res.Add(variable.Name, variable.Value);
                            }
                        }
                        break;
                }
            }
            return res;
        }

    }
}
