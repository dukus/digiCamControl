using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using GalaSoft.MvvmLight;

namespace Capture.Workflow.ViewModel
{
    public class NewViewSelectorViewModel:ViewModelBase
    {
        public List<PluginInfo> Items { get; set; }
        public PluginInfo SelectedItem { get; set; }
        public string Name { get; set; }

        public NewViewSelectorViewModel()
        {
            Items = WorkflowManager.Instance.GetPlugins(PluginType.View);
            if (Items.Count > 0)
                SelectedItem = Items[0];
        }
    }
}
