using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Commands
{
    [Description("")]
    [PluginType(PluginType.Command)]
    [DisplayName("CheckCamera")]
    [Icon("CameraSwitch")]
    public class CheckCameraAction: BaseCommand, IWorkflowCommand
    {
        public WorkFlowCommand CreateCommand()
        {
            var command = GetCommand();
            command.Properties.Add(new CustomProperty()
            {
                Name = "Message",
                PropertyType = CustomPropertyType.String
            });
            return command;
        }

        public bool Execute(WorkFlowCommand command,Context context)
        {
            if (!CheckCondition(command, context))
                return true;

            if (ServiceProvider.Instance.DeviceManager.ConnectedDevices.Count == 0 ||
                ServiceProvider.Instance.DeviceManager.SelectedCameraDevice == null || !ServiceProvider.Instance
                    .DeviceManager.SelectedCameraDevice.IsConnected)
            {
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.ShowMessage,command.Properties["Message"].ToString(context)));
                return false;
            }

            return true;
        }
    }
}
