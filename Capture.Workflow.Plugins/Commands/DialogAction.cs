using System.ComponentModel;
using System.Windows;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("Dialog")]
    [Icon("CommentQuestionOutline")]
    public class DialogAction : BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Message",
                PropertyType = CustomPropertyType.String
            });
            command.Properties.Add(new CustomProperty()
            {
                Description = "Display a dialog with Yes/No buttons, No will abort the commands executions",
                Name = "YesNo",
                PropertyType = CustomPropertyType.Bool
            });
            command.Properties.Add(new CustomProperty()
            {
                Description = "If checked, after message show, will abort the command list executions",
                Name = "Error",
                PropertyType = CustomPropertyType.Bool
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (!CheckCondition(command, context))
                return true;
            if (!string.IsNullOrEmpty(command.Properties["Message"].ToString(context)))
                if (command.Properties["YesNo"].ToBool(context))
                {
                    return MessageBox.Show(command.Properties["Message"].ToString(context),"",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes;
                }
                else
                {
                    MessageBox.Show(command.Properties["Message"].ToString(context));
                }
            return !command.Properties["Error"].ToBool(context);
        }
    }
}

