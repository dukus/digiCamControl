using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class NoTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "NoTransform"; } 
        }

        public bool Execute(string source, string dest, ValuePairEnumerator configData)
        {
            File.Copy(source, dest, true);
            return true;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            return null;
        }
    }
}
