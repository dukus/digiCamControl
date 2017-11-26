using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Capture.Workflow.View
{
    /// <summary>
    /// Interaction logic for HelpView.xaml
    /// </summary>
    public partial class HelpView : Window
    {
        public HelpView()
        {
            
        }

        public HelpView(Stream fileStream )
        {
            InitializeComponent();

            var textRange = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
            textRange.Load(fileStream, DataFormats.Rtf);
        }
    }
}
