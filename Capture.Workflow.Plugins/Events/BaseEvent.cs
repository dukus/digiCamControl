using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices;
using Capture.Workflow.Core.Classes;
using Jint;

namespace Capture.Workflow.Plugins.Events
{
    public class BaseEvent
    {
        public string Name { get; }

        public WorkFlowEvent GetEvent()
        {
            WorkFlowEvent workFlowEvent = new WorkFlowEvent();
            workFlowEvent.Properties.Items.Add(new CustomProperty()
            {
                Name = "Condition",
                PropertyType = CustomPropertyType.Code
            });
            return workFlowEvent;
        }

        public bool CheckCondition(WorkFlowEvent _event, Context context)
        {
            if (string.IsNullOrWhiteSpace(_event.Properties["Condition"].ToString(context)))
                return true;

            var var = new Engine();

            foreach (var variable in context.WorkFlow.Variables.Items)
            {
                var.SetValue(variable.Name, variable.GetAsObject());
            }
            try
            {
                return var.Execute(_event.Properties["Condition"].ToString(context)).GetCompletionValue().AsBoolean();
            }
            catch (Exception e)
            {
                Log.Error("Evaluation error " + _event.Name, e);
            }
            return false;
        }
    }
}

