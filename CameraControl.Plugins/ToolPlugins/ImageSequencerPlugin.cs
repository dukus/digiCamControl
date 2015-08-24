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

        public string Id
        {
            get { return "{6E020FD0-BEF0-4B22-8247-8271D07B4900}"; }
        }

        public string Title { get; set; }

        public ImageSequencerPlugin()
        {
            Title = TranslationStrings.LabelImageSequencer;
        }
    }
}
