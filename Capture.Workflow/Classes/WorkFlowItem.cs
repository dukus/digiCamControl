using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using GalaSoft.MvvmLight;

namespace Capture.Workflow.Classes
{
    public class WorkFlowItem: ViewModelBase
    {
        private WorkFlow _workflow;

        public WorkFlow Workflow
        {
            get { return _workflow; }
            set
            {
                _workflow = value;
                RaisePropertyChanged(()=>Workflow);
            }
        }

        public bool IsEditable { get; set; }
        public bool IsRevertable { get; set; }
        public bool IsPackage { get; set; }
        public string File { get; set; }
    }
}
