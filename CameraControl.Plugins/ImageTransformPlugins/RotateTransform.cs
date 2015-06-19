using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class RotateTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Rotate"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            throw new NotImplementedException();
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            throw new NotImplementedException();
        }
    }
}
