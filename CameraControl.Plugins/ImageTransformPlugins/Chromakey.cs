using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class Chromakey : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Chromakey"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            return "";
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new ChromakeyView();
            control.DataContext = new ChromakeyViewModel(configData);
            return control;
        }
    }
}
