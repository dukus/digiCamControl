using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;

namespace CameraControl.Plugins.ToolPlugins
{
    class GenThumbPlugin : IToolPlugin
    {
        public bool Execute()
        {
            GenThumbWindow window = new GenThumbWindow();
            window.DataContext = new GenThumbViewModel(window);
            window.ShowDialog();
            return true;
        }

        public string Title { get; set; }

        public GenThumbPlugin()
        {
            Title = TranslationStrings.LabelGenerateThumbs;
            
        }
    }
}
