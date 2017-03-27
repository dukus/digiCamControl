using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;

namespace Capture.Workflow.Plugins.Views
{
    [Description("Preview window for captured photos")]
    [PluginType(PluginType.View)]
    [DisplayName("Preview")]
    public class Preview: BaseView
    {
        public override WorkFlowView CreateView()
        {
            WorkFlowView view = new WorkFlowView();
            view.Properties.Items.Add(new CustomProperty()
            {
                Name = "ViewTitle",
                PropertyType = CustomPropertyType.String
            });
            return view;
        }

        public override List<string> GetPositions()
        {
            return new List<string> { "Left", "BottomLeft", "BottomRight" };
        }
    }
}
