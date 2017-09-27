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
    [Description("Executed when session finished for every captured item ")]
    [PluginType(PluginType.Event)]
    [DisplayName("ProcessItem")]
    public class ProcessItemEvent: BaseEvent, IEventPlugin
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
            if (e.Name == Messages.SessionFinished)
            {
                var contex = e.Param as Context;
                if (contex != null)
                {
                    foreach (FileItem item in WorkflowManager.Instance.FileItems)
                    {
                        if (!CheckCondition(_flowEvent, contex))
                            continue;
                        var itemContex = new Context();
                        itemContex.CameraDevice = contex.CameraDevice;
                        itemContex.WorkFlow = contex.WorkFlow;
                        itemContex.FileItem = item;
                        itemContex.Target = ContextTargetEnum.FileItem;
                        WorkflowManager.Execute(_flowEvent.CommandCollection, itemContex);
                    }
                }
            }
        }

        public void UnRegisterEvent(WorkFlowEvent flowEvent)
        {
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}
