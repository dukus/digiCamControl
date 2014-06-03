using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Xml
{
    public class XmlPropertyValue : BaseFieldClass
    {
        [XmlAttribute]
        public long Value { get; set; }

        private string _name;

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("DisplayName");
            }
        }

        private string _hexCode;
        [XmlAttribute]
        public string HexValue
        {
            get
            {
                _hexCode = Value.ToString("X");
                return _hexCode;
            }
            set { _hexCode = value; }
        }

        [XmlIgnore]
        public string DisplayName
        {
            get
            {
                return Value+ " - "+ HexValue + " - " + Name; ;
            }
        }
        public override string ToString()
        {
            return HexValue + Name;
        }
    }
}
