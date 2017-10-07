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

namespace Capture.Workflow.Plugins.Events
{
    [Description("")]
    [PluginType(PluginType.Event)]
    [DisplayName("LiveViewEvent")]
    public class LiveViewEvent: BaseEvent, IEventPlugin
    {

        private WorkFlowEvent _flowEvent;

        public WorkFlowEvent CreateEvent()
        {
            WorkFlowEvent workFlowEvent = GetEvent();
            return workFlowEvent;
        }

        public void RegisterEvent(WorkFlowEvent flowEvent)
        {
            _flowEvent = flowEvent;
            WorkflowManager.Instance.Message += Instance_Message;
        }


        private void Instance_Message(object sender, MessageEventArgs e)
        {

            if (e.Name == Messages.LiveViewChanged)
            {
                if (CheckCondition(_flowEvent, e.Context))
                    WorkflowManager.Execute(_flowEvent.CommandCollection, e.Context);
            }
        }

        public void UnRegisterEvent(WorkFlowEvent flowEvent)
        {
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}
