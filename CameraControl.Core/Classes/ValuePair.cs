using System.Xml.Serialization;

namespace CameraControl.Core.Classes
{
    public class ValuePair
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        [XmlAttribute]
        public bool IsDisabled { get; set; }
    }
}
