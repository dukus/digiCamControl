using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CameraControl.Devices.Custom
{
    public class DeviceDescription
    {
        [XmlAttribute]
        public string Model { get; set; }
        [XmlAttribute]
        public string BaseDevice { get; set; }
        [XmlAttribute]
        public string Manufacturer { get; set; }
        
        public string Description { get; set; }

        public List<DeviceProperty> Properties { get; set; }

        public DeviceDescription()
        {
            Properties = new List<DeviceProperty>();
        }

        public void Save(string file)
        {
                XmlSerializer serializer = new XmlSerializer(typeof(DeviceDescription));
                Stream writer = new FileStream(file, FileMode.Create);
                serializer.Serialize(writer, this);
                writer.Close();
        }

        public static DeviceDescription Load(string file)
        {
            try
            {
                DeviceDescription settings = new DeviceDescription();
                XmlSerializer mySerializer = new XmlSerializer(typeof(DeviceDescription));
                FileStream myFileStream = new FileStream(file, FileMode.Open);
                settings = (DeviceDescription)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();

                return settings;
            }
            catch (Exception ex)
            {
                Log.Error("Error load device description "+file,ex);
                return null;

            }
        }
       
    }

}
