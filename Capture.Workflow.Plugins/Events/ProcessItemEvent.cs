using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public class ProcessItemEvent : BaseEvent, IEventPlugin
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
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.IsBusy, null));
                var contex = e.Param as Context;
                if (contex != null)
                {
                    if (!CheckCondition(_flowEvent, contex))
                        return;
                    foreach (FileItem item in WorkflowManager.Instance.FileItems)
                    {
                        if (!CheckCondition(_flowEvent, contex))
                            continue;
                        var itemContex = new Context();
                        itemContex.CameraDevice = contex.CameraDevice;
                        itemContex.WorkFlow = contex.WorkFlow;
                        itemContex.FileItem = item;
                        itemContex.Target = ContextTargetEnum.FileItem;
                        itemContex.FileItem = item;
                        //Load item specific variable values
                        foreach (var variable in item.Variables.Items)
                        {
                            var varItem = itemContex.WorkFlow.Variables[variable.Name];
                            if (varItem != null)
                            {
                                varItem.AttachedVariable = variable;
                            }
                        }
                        WorkflowManager.Instance.Context = itemContex;
                        WorkflowManager.Execute(_flowEvent.CommandCollection, itemContex);
                    }
                }
                WorkflowManager.Instance.OnMessage(new MessageEventArgs(Messages.IsNotBusy, null));
            }
        }

        public void UnRegisterEvent(WorkFlowEvent flowEvent)
        {
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}
