using System;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using MaterialDesignColors;

namespace Capture.Workflow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            var Swatches = new SwatchesProvider().Swatches;
            foreach (var swatch in Swatches)
            {
                Console.Write("\""+swatch.Name+"\",");
            }
        }

        private void MetroWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.KeyPressed, e));
        }
    }
}
