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

        public string Execute(FileItem item, string dest, ValuePairEnumerator configData)
        {
            if (item.FileName != dest)
                File.Copy(item.FileName, dest, true);
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            return null;
        }
    }
}
