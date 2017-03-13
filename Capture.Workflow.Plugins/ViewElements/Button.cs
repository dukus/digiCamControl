using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.ViewElements
{
    [Description("")]
    [PluginType(PluginType.ViewElement)]
    [DisplayName("Button")]
    public class Button:IViewElementPlugin
    {
        public string Name { get; set; }
        public WorkFlowViewElement CreateElement()
        {
            WorkFlowViewElement element = new WorkFlowViewElement();
            element.Properties.Items.Add(new CustomProperty()
            {
                Name = "Caption",
                PropertyType = CustomPropertyType.String
            });
            return element;
        }

    }
}
