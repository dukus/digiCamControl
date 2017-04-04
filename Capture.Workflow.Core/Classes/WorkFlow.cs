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
        public ObservableCollection<Variable> Variables { get; set; }

        public WorkFlow()
        {
            Views = new ObservableCollection<WorkFlowView>();
            Variables = new AsyncObservableCollection<Variable>
            {
                new Variable() {Name = "SessionFolder"},
                new Variable() {Name = "FileNameTemplate"}
            };
        }

    }
}
