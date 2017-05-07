using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Devices.Classes;

namespace Capture.Workflow.Core.Classes
{
    public class WorkFlow
    {
        public ObservableCollection<WorkFlowView> Views { get; set; }
        public ObservableCollection<WorkFlowEvent> Events { get; set; }
        public VariableCollection Variables { get; set; }

        public WorkFlow()
        {
            Views = new ObservableCollection<WorkFlowView>();
            Variables = new VariableCollection();
            Events = new AsyncObservableCollection<WorkFlowEvent>();
        }


        public WorkFlowView GetView(string name)
        {
            foreach (var view in Views)
            {
                if (view.Name == name)
                    return view;
            }
            return null;
        }

    }
}
