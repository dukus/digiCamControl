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
        }

        private void MetroWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.KeyPressed, e));
        }
    }
}
