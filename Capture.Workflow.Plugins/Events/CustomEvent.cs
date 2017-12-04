using System.ComponentModel;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Events
{
    [Description("")]
    [PluginType(PluginType.Event)]
    [DisplayName("CustomEvent")]
    public class CustomEvent:BaseEvent, IEventPlugin
    {

        private WorkFlowEvent _flowEvent;

        public WorkFlowEvent CreateEvent()
        {
            WorkFlowEvent workFlowEvent = GetEvent();
            workFlowEvent.Properties.Items.Add(new CustomProperty()
            {
                Name = "Event",
                PropertyType = CustomPropertyType.String
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
            var contex = e.Param as Context;
            contex = contex ?? WorkflowManager.Instance.Context;

            if (e.Name == _flowEvent.Properties["Event"].ToString(contex))
            {
                if (CheckCondition(_flowEvent, contex))
                {
                    var c = WorkflowManager.Instance.Context;
                    WorkflowManager.Execute(_flowEvent.CommandCollection, contex);
                }
            }
        }

        public void UnRegisterEvent(WorkFlowEvent flowEvent)
        {
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}
