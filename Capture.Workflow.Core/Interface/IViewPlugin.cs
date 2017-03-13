using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core.Interface
{
    public interface IViewPlugin
    {
        string Name { get; set; }
        WorkFlowView CreateView();
    }
}