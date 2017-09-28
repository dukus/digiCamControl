using System.Collections.Generic;
using System.Windows.Controls;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core.Interface
{
    public interface IViewPlugin
    {
        string Name { get; set; }
        WorkFlowView CreateView();
        List<string> GetPositions();
        UserControl GetPreview(WorkFlowView view, Context context);
    }
}