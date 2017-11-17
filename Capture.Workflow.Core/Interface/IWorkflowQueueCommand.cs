using Capture.Workflow.Core.Database;

namespace Capture.Workflow.Core.Interface
{
    public interface IWorkflowQueueCommand
    {
        bool ExecuteQueue(DbQueue queue);
    }
}