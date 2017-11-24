using System.ComponentModel;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Events
{
    [Description("Executed when the specified variable value was changed ")]
    [PluginType(PluginType.Event)]
    [DisplayName("VariableChanged")]
    public class VariableChangedEvent: BaseEvent, IEventPlugin
    {
        private WorkFlowEvent _flowEvent;

        public WorkFlowEvent CreateEvent()
        {
            WorkFlowEvent workFlowEvent = GetEvent();
            workFlowEvent.Properties.Items.Add(new CustomProperty()
            {
                Name = "Variable",
                PropertyType = CustomPropertyType.Variable,
            });
            return workFlowEvent;
        }

        public void RegisterEvent(WorkFlowEvent flowEvent)
        {
            _flowEvent = flowEvent;
            WorkflowManager.Instance.Message += Instance_Message;
        }

        private void Instance_Message(object sender, MessageEventArgs e)
        {
            if (e.Name == Messages.VariableChanged)
            {
                var var = e.Param as Variable;
                if (var != null && var.Name == _flowEvent.Properties["Variable"].ToString(e.Context))
                {
                    if (!CheckCondition(_flowEvent, e.Context))
                        return;
                    WorkflowManager.Execute(_flowEvent.CommandCollection, e.Context);
                }
            }
        }

        public void UnRegisterEvent(WorkFlowEvent flowEvent)
        {
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}
