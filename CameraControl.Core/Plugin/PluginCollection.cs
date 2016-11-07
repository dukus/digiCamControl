using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CameraControl.Core.Classes;
using CameraControl.Devices;

namespace CameraControl.Core.Plugin
{
    public class PluginCollection
    {
        public List<PluginInfo> Items { get; set; }

        public PluginCollection()
        {
            Items=new List<PluginInfo>();
        }

        public static PluginCollection Load(string fileName)
        {
            PluginCollection res = new PluginCollection();
            try
            {
                if (File.Exists(fileName))
                {
                    FileStream myFileStream = new FileStream(fileName, FileMode.Open);
                    Load(myFileStream);
                    myFileStream.Close();
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error loading plugin list ", exception);
            }
            return res;
        }

        public static PluginCollection Load(Stream stream)
        {
            PluginCollection res = new PluginCollection();
            try
            {
                    XmlSerializer mySerializer =
                        new XmlSerializer(typeof(PluginCollection));
                    res = (PluginCollection)mySerializer.Deserialize(stream);
           }
            catch (Exception exception)
            {
                Log.Error("Error loading plugin list ", exception);
            }
            return res;
        }

        public void Save(string fileName)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(PluginCollection));
                // Create a FileStream to write with.

                Stream writer = new FileStream(fileName, FileMode.Create);
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to save plugin list ", exception);
            }
        }
    }
}
