using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core.Interface
{
    public interface IEventPlugin
    {
        string Name { get; }
        WorkFlowEvent CreateEvent();
        void RegisterEvent(WorkFlowEvent flowEvent);
        void UnRegisterEvent(WorkFlowEvent flowEvent);
    }
}
