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

        public string Id
        {
            get { return "{C66CBC77-71A3-491A-B844-56AF460DBEF5}"; }
        }

        public string Title { get; set; }

        public GenThumbPlugin()
        {
            Title = TranslationStrings.LabelGenerateThumbs;
            
        }
    }
}
