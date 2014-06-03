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
using System.Security.AccessControl;
using System.Text;
using System.Xml.Serialization;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

#endregion

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
                        new XmlSerializer(typeof (PluginInfo));
                    FileStream myFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    pluginInfo = (PluginInfo) mySerializer.Deserialize(myFileStream);
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