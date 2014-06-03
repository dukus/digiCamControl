using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace CameraControl.Core.Scripting
{
    public class MyCompletionData : ICompletionData
    {
        public string DisplayText { get; set; }

        public MyCompletionData(string text, string description, string displaytext = null)
        {
            Text = text;
            Description = description;
            DisplayText = displaytext;
            if (string.IsNullOrEmpty(DisplayText))
                DisplayText = Text;
            if (string.IsNullOrEmpty(Text))
                Text = displaytext;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.DisplayText; }
        }

        public object Description { get; private set; }

        public double Priority
        {
            get { return 0; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
