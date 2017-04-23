using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using GalaSoft.MvvmLight;

namespace Capture.Workflow.ViewModel
{
    public class WorkflowViewViewModel: ViewModelBase
    {
        private UserControl _contents;
        private WorkFlow _workflow;

        public UserControl Contents
        {
            get { return _contents; }
            set
            {
                _contents = value;
                RaisePropertyChanged(()=>Contents);
            }
        }

        public WorkFlow Workflow
        {
            get { return _workflow; }
            set
            {
                _workflow = value;
                RaisePropertyChanged(() => Workflow);
            }
        }


        public WorkflowViewViewModel()
        {
            Workflow = WorkflowManager.Instance.Context.WorkFlow;
            WorkflowManager.Instance.Context.CameraDevice = ServiceProvider.Instance.DeviceManager.SelectedCameraDevice;
            if (!IsInDesignMode)
            {
                WorkflowManager.Instance.Message += Instance_Message;
                Contents = Workflow.Views[0].Instance.GetPreview(Workflow.Views[0]);
            }
        }

        private void Instance_Message(object sender, MessageEventArgs e)
        {
            switch (e.Name)
            {
                case Messages.ShowView:
                    WorkFlowView view = Workflow.GetView((string) e.Param);
                    if (view != null)
                    {
                        if (Contents.DataContext != null)
                        {
                            // dispose old view if was loaded
                            var obj = Contents.DataContext as IDisposable;
                            if (obj != null)
                            {
                                obj.Dispose();
                            }
                        }
                        Contents = view.Instance.GetPreview(view);
                    }
                    break;


            }
        }
    }
}
