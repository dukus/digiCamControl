using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Xml.Serialization;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Plugin
{
    public class PluginInfo : BaseFieldClass
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Id { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Folder { get; set; }
        public string AssemblyFileName { get; set; }
        public string LogoFile { get; set; }

        [XmlIgnore]
        public bool Enabled { get; set; }


        public static PluginInfo Load(string filename)
        {
            PluginInfo pluginInfo = new PluginInfo();
            try
            {
                if (File.Exists(filename))
                {
                    XmlSerializer mySerializer =
                      new XmlSerializer(typeof(PluginInfo));
                    FileStream myFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    pluginInfo = (PluginInfo)mySerializer.Deserialize(myFileStream);
                    myFileStream.Close();
                    if (!string.IsNullOrEmpty(pluginInfo.LogoFile))
                    {
                        pluginInfo.LogoFile = Path.Combine(Path.GetDirectoryName(filename), pluginInfo.LogoFile);
                        if (!File.Exists(pluginInfo.LogoFile))
                            Log.Debug("Logo file not found" + pluginInfo.LogoFile);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
            return pluginInfo;    
        }

        public PluginInfo()
        {
            
        }
    }
}
