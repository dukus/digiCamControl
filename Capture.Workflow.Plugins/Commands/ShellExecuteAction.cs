using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;


namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("ShellExecute")]
    [Icon("Equal")]
    public class ShellExecuteAction: BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Executable",
                PropertyType = CustomPropertyType.String,
            });
            command.Properties.Add(new CustomProperty
            {
                Name = "Parameter",
                PropertyType = CustomPropertyType.String,
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command, Context context)
        {
            if (!CheckCondition(command, context))
                return true;

            Utils.Run(command.Properties["Executable"].ToString(context),
                command.Properties["Parameter"].ToString(context));

            return true;
        }
    }
}
