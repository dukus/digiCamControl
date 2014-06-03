using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;

namespace CameraControl.Devices.Xml
{
    public class XmlCommandDescriptor : BaseFieldClass
    {
        [XmlAttribute]
        public uint Code { get; set; }

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

        [XmlAttribute]
        public string Description { get; set; }

        private string _hexCode;
        [XmlAttribute]
        public string HexCode
        {
            get
            {
                _hexCode = Code.ToString("X");
                return _hexCode;
            }
            set { _hexCode = value; }
        }

        [XmlIgnore]
        public string DisplayName
        {
            get
            {
                return HexCode + " - " + Name; ;
            }
        }

        public override string ToString()
        {
            return HexCode + Name;
        }
    }
}
