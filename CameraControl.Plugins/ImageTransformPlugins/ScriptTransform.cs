using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class ScriptTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Script"; }
        }

        public string Execute(FileItem item, string infile, string dest, ValuePairEnumerator configData)
        {
            throw new NotImplementedException();
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new ScriptTransformView { DataContext = new ScriptTransformViewModel(configData) };
            return control;
        }
    }
}
