using System;
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
            {
                switch (param.Type)
                {
                    case PluginType.View:
                        ((WorkflowEditorViewModel)DataContext).NewViewCommand.Execute(param);
                        break;
                    case PluginType.Event:
                        ((WorkflowEditorViewModel)DataContext).NewEventCommand.Execute(param);
                        break;
                    case PluginType.Action:
                        break;
                    case PluginType.ViewElement:
                        ((WorkflowEditorViewModel)DataContext).NewViewElementCommand.Execute(param);
                        break;
                    case PluginType.Command:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void WorkflowEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((WorkflowEditorViewModel) DataContext).SaveCommand.Execute(null);
        }
    }
}
