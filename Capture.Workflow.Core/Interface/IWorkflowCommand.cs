using System.Collections.Generic;
using System.Windows.Controls;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core.Interface
{
    public interface IWorkflowCommand
    {
        string Name { get; set; }
        WorkFlowCommand CreateCommand();
        bool  Execute(WorkFlowCommand command, Context context);
    }
}