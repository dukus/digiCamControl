using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    class ResizeTransform : IImageTransformPlugin
    {
        public string Name { get; set; }
        public bool Execute(string source, string dest, ValuePairEnumerator configData)
        {
            return true;
        }
    }
}
