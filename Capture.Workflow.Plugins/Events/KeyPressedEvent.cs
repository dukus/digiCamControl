using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Capture.Workflow.Plugins.Events
{
    [Description("")]
    [PluginType(PluginType.Event)]
    [DisplayName("KeyPressed")]
    public class KeyPressedEvent:BaseEvent, IEventPlugin
    {
        private WorkFlowEvent _flowEvent;

        public WorkFlowEvent CreateEvent()
        {
            WorkFlowEvent workFlowEvent = GetEvent();
            workFlowEvent.Properties.Items.Add(new CustomProperty()
            {
                Name = "Key",
                PropertyType = CustomPropertyType.ValueList,
                ValueList = Enum.GetNames(typeof(Keys)).ToList()
            });
            workFlowEvent.Properties.Items.Add(new CustomProperty()
            {
                Name = "Alt",
                PropertyType = CustomPropertyType.Bool
            });
            workFlowEvent.Properties.Items.Add(new CustomProperty()
            {
                Name = "Ctrl",
                PropertyType = CustomPropertyType.Bool
            });
            workFlowEvent.Properties.Items.Add(new CustomProperty()
            {
                Name = "Shift",
                PropertyType = CustomPropertyType.Bool
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
            KeyEventArgs args = e.Param as KeyEventArgs;
            if (args != null && args.Key == (Key) Enum.Parse(typeof(Key), _flowEvent.Properties["Key"].ToString(WorkflowManager.Instance.Context)))
            {
                if (CheckCondition(_flowEvent, WorkflowManager.Instance.Context))
                    WorkflowManager.ExecuteAsync(_flowEvent.CommandCollection, WorkflowManager.Instance.Context);
            }
        }

        public void UnRegisterEvent(WorkFlowEvent flowEvent)
        {
            WorkflowManager.Instance.Message -= Instance_Message;
        }
    }
}
