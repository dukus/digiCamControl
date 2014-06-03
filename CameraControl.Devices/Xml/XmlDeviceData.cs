using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CameraControl.Devices.Xml
{
    public class XmlDeviceData
    {
        public List<XmlCommandDescriptor> AvaiableCommands { get; set; }
        public List<XmlEventDescriptor> AvaiableEvents { get; set; }
        public List<XmlPropertyDescriptor> AvaiableProperties { get; set; }

        public string Model { get; set; }
        public string Manufacturer { get; set; }


        public XmlDeviceData()
        {
            AvaiableCommands = new List<XmlCommandDescriptor>();
            AvaiableEvents = new List<XmlEventDescriptor>();
            AvaiableProperties = new List<XmlPropertyDescriptor>();
        }

        public static XmlDeviceData Load(string filename)
        {
            XmlDeviceData photoSession = new XmlDeviceData();
            if (File.Exists(filename))
            {
                XmlSerializer mySerializer =
                    new XmlSerializer(typeof (XmlDeviceData));
                FileStream myFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                photoSession = (XmlDeviceData) mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
            }
            return photoSession;
        }

        public string GetCommandName(uint code)
        {
            foreach (XmlCommandDescriptor xmlCommandDescriptor in AvaiableCommands)
            {
                if (xmlCommandDescriptor.Code == code)
                    return xmlCommandDescriptor.Name;
            }
            return "";
        }

        public string GetEventName(uint code)
        {
            foreach (XmlEventDescriptor xmlEventDescriptor in AvaiableEvents)
            {
                if (xmlEventDescriptor.Code == code)
                    return xmlEventDescriptor.Name;
            }
            return "";
        }

        public string GetPropName(uint code)
        {
            foreach (XmlPropertyDescriptor xmlPropertyDescriptor in AvaiableProperties)
            {
                if (xmlPropertyDescriptor.Code == code)
                    return xmlPropertyDescriptor.Name;
            }
            return "";
        }


    }
}
