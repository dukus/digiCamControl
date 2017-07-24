using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.ViewModel;

namespace Capture.Workflow.View
{
    /// <summary>
    /// Interaction logic for WorkflowEditorView.xaml
    /// </summary>
    public partial class WorkflowEditorView  
    {
        public WorkflowEditorView()
        {
            InitializeComponent();
        }

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            var param = eventArgs.Parameter as PluginInfo;
            if (param != null)
                ((WorkflowEditorViewModel) DataContext).NewViewCommand.Execute(param);
        }

    }
}
