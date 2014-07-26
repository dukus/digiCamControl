using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;

namespace CameraControl.Plugins.ToolPlugins
{
    public class ImageSequencerPlugin : IToolPlugin
    {
        public bool Execute()
        {
            GenThumbWindow windowthumb = new GenThumbWindow();
            windowthumb.DataContext = new GenThumbViewModel(windowthumb);
            windowthumb.ShowDialog();
            ImageSequencerWindow window = new ImageSequencerWindow();
            window.DataContext = new ImageSequencerViewModel(window);
            window.Show();
            return true;
        }

        public string Title { get; set; }

        public ImageSequencerPlugin()
        {
            Title = TranslationStrings.LabelImageSequencer;
        }
    }
}
