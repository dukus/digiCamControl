using System.Windows;
using System.Windows.Controls;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core.Interface
{
    public interface IViewElementPlugin
    {
        string Name { get; set; }
        WorkFlowViewElement CreateElement(WorkFlowView view);
        FrameworkElement GetControl(WorkFlowViewElement viewElement, Context context);
    }
}