using System.ComponentModel;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Events
{
    [Description("")]
    [PluginType(PluginType.Event)]
    [DisplayName("CameraConnected")]
    public class CameraConnectedEvent:BaseEvent, IEventPlugin
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
            var contex = WorkflowManager.Instance.Context;
            if (CheckCondition(_flowEvent, contex))
                WorkflowManager.ExecuteAsync(_flowEvent.CommandCollection, contex);

            ServiceProvider.Instance.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
        }

        private void DeviceManager_CameraConnected(CameraControl.Devices.ICameraDevice cameraDevice)
        {

            var contex =  WorkflowManager.Instance.Context;

            if (CheckCondition(_flowEvent, contex))
                WorkflowManager.ExecuteAsync(_flowEvent.CommandCollection, contex);
        }

        public void UnRegisterEvent(WorkFlowEvent flowEvent)
        {
            ServiceProvider.Instance.DeviceManager.CameraConnected -= DeviceManager_CameraConnected;
        }

    }
}
