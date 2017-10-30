using System.Collections.Generic;
using System.ComponentModel;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("WorkflowAction")]
    public class WorkflowAction :BaseCommand, IWorkflowCommand
    {
     
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Action",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = new List<string>
                {
                    "ShowView",
                    "FinishSession",
                    "CancelSession",
                    "PreviousView",
                    "UpdateThumb",
                    "NextPhoto",
                    "PrevPhoto",
                    "DeletePhoto",
                }
            });

            command.Properties.Add(new CustomProperty()
            {
                Name = "ViewName",
                PropertyType = CustomPropertyType.View
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (!CheckCondition(command, context))
                return true;

            switch (command.Properties["Action"].Value)
            {
                case "ShowView":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.ShowView,
                        command.Properties["ViewName"].ToString(context)));
                    break;
                case "FinishSession":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.SessionFinished, context));
                    break;
                case "CancelSession":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.SessionCanceled, context));
                    break;
                case "PreviousView":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.PreviousView, context));
                    break;
                case "UpdateThumb":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.ThumbCreate, context));
                    break;
                case "NextPhoto":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.NextPhoto, context));
                    break;
                case "PrevPhoto":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.PrevPhoto, context));
                    break;
                case "DeletePhoto":
                    WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.DeletePhoto, context));
                    break;
            }
            return true;
        }
    }
}
