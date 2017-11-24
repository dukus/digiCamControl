using System;
using CameraControl.Devices;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Scripting;


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
                Name = "(Name)",
                PropertyType = CustomPropertyType.String
            });
            workFlowEvent.Properties.Items.Add(new CustomProperty()
            {
                Name = "Condition",
                PropertyType = CustomPropertyType.Code
            });
            return workFlowEvent;
        }

        public bool CheckCondition(WorkFlowEvent _event, Context context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_event.Properties["Condition"].ToString(context)))
                    return true;

                return ScriptEngine.Instance.Evaluate(_event.Properties["Condition"].ToString(context), context);
            }
            catch (Exception e)
            {
                Log.Error("Event CheckCondition error " + _event.Name, e);
            }
            return false;
        }
    }
}

