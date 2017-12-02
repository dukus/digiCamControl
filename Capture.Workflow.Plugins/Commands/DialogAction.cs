using System.ComponentModel;
using System.Windows;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using Microsoft.Win32;

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
                Name = "Type",
                Value = "Message",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = {"Message", "Warning", "YesNo", "SaveFile"}
            });
            //command.Properties.Add(new CustomProperty()
            //{
            //    Description = "Display a dialog with Yes/No buttons, No will abort the commands executions",
            //    Name = "YesNo",
            //    PropertyType = CustomPropertyType.Bool
            //});
            command.Properties.Add(new CustomProperty()
            {
                Name = "Variable",
                PropertyType = CustomPropertyType.Variable,
                Description = "Variable to store the SaveFile dialog result"
            });
            command.Properties.Add(new CustomProperty()
            {
                Name = "FileNameFilter",
                PropertyType = CustomPropertyType.String,
                Value = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*",
                Description = "File name filter for the the SaveFile dialog result"
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
            switch (command.Properties["Type"].ToString(context))
            {
                case "Message":
                    MessageBox.Show(command.Properties["Message"].ToString(context));
                    break;
                case "Warning":
                    MessageBox.Show( command.Properties["Message"].ToString(context), "Warning", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    break;
                case "YesNo":
                    return MessageBox.Show(command.Properties["Message"].ToString(context), "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                case "SaveFile":
                    var dialog =new SaveFileDialog();
                    dialog.Filter = command.Properties["FileNameFilter"].ToString(context);
                    dialog.FileName = context.WorkFlow.Variables[command.Properties["Variable"].Value].Value;
                    if (dialog.ShowDialog() == true)
                        context.WorkFlow.Variables[command.Properties["Variable"].Value].Value = dialog.FileName;
                    else
                        return false;
                    break;
            }
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

