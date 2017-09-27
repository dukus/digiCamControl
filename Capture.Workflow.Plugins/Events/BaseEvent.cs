using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (string.IsNullOrWhiteSpace(_event.Properties["Condition"].Value))
                return true;

            var var = new Engine();

            foreach (var variable in context.WorkFlow.Variables.Items)
            {
                //e.Parameters[variable.Name] = new Exception(variable.Value);
                var.SetValue(variable.Name, variable.GetAsObject());
            }
            return var.Execute(_event.Properties["Condition"].Value).GetCompletionValue().AsBoolean();
        }
    }
}

