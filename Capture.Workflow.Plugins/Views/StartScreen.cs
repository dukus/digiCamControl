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
    [Description("Vindow to query parameter on start")]
    [PluginType(PluginType.View)]
    [DisplayName("StartScreen")]
    public class StartScreen : BaseView
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
            view.Events.Add(new CommandCollection("Load"));
            view.Events.Add(new CommandCollection("UnLoad"));
            return view;
        }

        public override List<string> GetPositions()
        {
            return new List<string> { "Center", "BottomLeft", "BottomRight","Background" };
        }

        public override UserControl GetPreview(WorkFlowView view, Context context)
        {
            WorkflowManager.Execute(view.GetEventCommands("Load"), context);
            var model = new StartScreenViewModel();
            foreach (var element in view.Elements)
            {
                switch (element.Properties["Position"].Value)
                {
                    case "Center":
                        model.CenterElements.Add(element.Instance.GetControl(element, context));
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
            var res = new StartScreenView();
            res.DataContext = model;
            return res;
        }
    }
}
