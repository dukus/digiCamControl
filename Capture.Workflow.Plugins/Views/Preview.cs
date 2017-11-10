using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Plugins.Views.View;
using Capture.Workflow.Plugins.Views.ViewModel;

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
                Name = "(Name)",
                PropertyType = CustomPropertyType.String
            });
            view.Properties.Items.Add(new CustomProperty()
            {
                Name = "ViewTitle",
                PropertyType = CustomPropertyType.String
            });
            view.Properties.Items.Add(new CustomProperty()
            {
                Name = "FileListVisible",
                PropertyType = CustomPropertyType.Bool,
                Value = "True"
            });
            view.Events.Add(new CommandCollection("Load"));
            view.Events.Add(new CommandCollection("UnLoad"));
            return view;
        }

        public override List<string> GetPositions()
        {
            return new List<string> { "Left", "Right", "BottomLeft", "BottomRight", "Background" };
        }
        public override UserControl GetPreview(WorkFlowView view, Context context)
        {
            var model = new PreviewViewModel();
            foreach (var element in view.Elements)
            {
                switch (element.Properties["Position"].Value)
                {
                    case "Left":
                        model.LeftElements.Add(element.Instance.GetControl(element,context));
                        break;
                    case "Right":
                        model.RightElements.Add(element.Instance.GetControl(element, context));
                        break;
                    case "BottomLeft":
                        model.BottomLeftElements.Add(element.Instance.GetControl(element, context));
                        break;
                    case "BottomRight":
                        model.BottomRightElements.Add(element.Instance.GetControl(element, context));
                        break;
                    case "Background":
                        model.BackGroundElements.Add(element.Instance.GetControl(element, context));
                        break;
                }
            }
            model.View = view;
            var res = new PreviewView();
            model.FileListVisible = view.Properties["FileListVisible"].ToBool(context);
            model.Preview = true;
            res.DataContext = model;
            WorkflowManager.ExecuteAsync(view.GetEventCommands("Load"), WorkflowManager.Instance.Context);
            return res;
        }
    }
}
