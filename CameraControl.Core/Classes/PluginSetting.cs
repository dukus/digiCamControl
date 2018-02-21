using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace CameraControl.Core.Classes
{
    public class PluginSetting
    {
        public string Name { get; set; }
        public List<ValuePair> Values { get; set; }
        public AutoExportPluginConfig AutoExportPluginConfig { get; set; }

        public PluginSetting()
        {
            Values = new List<ValuePair>();
            AutoExportPluginConfig = new AutoExportPluginConfig(){Name = "Plugin"};
        }

        /// <summary>
        /// Return value for name parameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns> string</returns>
        [XmlIgnore]
        [JsonIgnore]
        public object this[string name]
        {
            get
            {
                foreach (var value in Values)
                {
                    if (value.Name == name)
                        return value.Value;
                }
                return "";
            }
            set
            {
                foreach (var values in Values)
                {
                    if (values.Name == name)
                    {
                        values.Value = value.ToString();
                        return;
                    }
                }
                Values.Add(new ValuePair() {Name = name, Value = value.ToString()});
            }
        }


        public bool GetBool(string name)
        {
            return (from value in Values where value.Name == name select value.Value == "True").FirstOrDefault();
        }

        public int GetInt(string name)
        {
            int i = 0;
            foreach (var value in Values)
            {
                if (value.Name == name)
                {
                    if (int.TryParse(value.Value, out i))
                        return i;
                }
            }
            return 0;
        }
    }
}
