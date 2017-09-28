using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Views
{
    public class BaseView: IViewPlugin
    {
        public string Name { get; set; }

        public virtual WorkFlowView CreateView()
        {
            throw new NotImplementedException();
        }

        public virtual List<string> GetPositions()
        {
            throw new NotImplementedException();
        }

        public virtual UserControl GetPreview(WorkFlowView view, Context context)
        {
            throw new NotImplementedException();
        }
    }
}
