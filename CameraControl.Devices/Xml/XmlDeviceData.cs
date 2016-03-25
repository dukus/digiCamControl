#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

#endregion

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
        
        public bool PropertyExist(uint code)
        {
            return AvaiableProperties.Any(xmlPropertyDescriptor => xmlPropertyDescriptor.Code == code);
        }
    }
}