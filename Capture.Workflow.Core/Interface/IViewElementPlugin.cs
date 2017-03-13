using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core.Interface
{
    public interface IViewElementPlugin
    {
        string Name { get; set; }
        WorkFlowViewElement CreateElement();
    }
}