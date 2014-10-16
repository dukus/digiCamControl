using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    class ResizeTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Resize"; }
        }

        public bool Execute(string source, string dest, ValuePairEnumerator configData)
        {
            return true;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            return null;
        }
    }
}
