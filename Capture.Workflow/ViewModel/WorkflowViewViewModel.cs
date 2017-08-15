using System;
using System.Windows.Controls;
using CameraControl.Devices;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using GalaSoft.MvvmLight;

namespace Capture.Workflow.ViewModel
{
    public class WorkflowViewViewModel: ViewModelBase, IDisposable
    {
        private UserControl _contents;
        private WorkFlow _workflow;
        private string _title;
        private Context _context;

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

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public ICameraDevice CameraDevice
        {
            get { return WorkflowManager.Instance.Context.CameraDevice; }
            set
            {
                WorkflowManager.Instance.Context.CameraDevice = value;
                RaisePropertyChanged(() => CameraDevice);
            }
        }


        public WorkflowViewViewModel()
        {
            Workflow = WorkflowManager.Instance.Context.WorkFlow;
            CameraDevice = ServiceProvider.Instance.DeviceManager.SelectedCameraDevice;
            if (!IsInDesignMode)
            {
                WorkflowManager.Instance.Message += Instance_Message;
                ServiceProvider.Instance.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
                ShowView(Workflow.Views[0].Name);
            }
        }

        private void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
        {
            
        }

        private void Instance_Message(object sender, MessageEventArgs e)
        {
            switch (e.Name)
            {
                case Messages.ShowView:
                    ShowView((string)e.Param);
                    break;
            }
        }

        private void ShowView(string viewName)
        {
            WorkFlowView view = Workflow.GetView(viewName);
            if (view != null)
            {
                Title = view.Properties["ViewTitle"]?.Value;
                if (Contents?.DataContext != null)
                {
                    // dispose old view if was loaded
                    var obj = Contents.DataContext as IDisposable;
                    obj?.Dispose();
                }
                Contents = view.Instance.GetPreview(view);
            }
        }


        public void Dispose()
        {
            WorkflowManager.Instance.Message -= Instance_Message;
            ServiceProvider.Instance.DeviceManager.CameraConnected -= DeviceManager_CameraConnected;
        }
    }
}
