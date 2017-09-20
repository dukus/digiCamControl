using System;
using System.IO;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Capture.Workflow.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        private AsyncObservableCollection<WorkFlow> _workFlows;
        public RelayCommand EditCommand { get; set; }
        public RelayCommand<WorkFlow> RunCommand { get; set; }

        public AsyncObservableCollection<WorkFlow> WorkFlows
        {
            get { return _workFlows; }
            set
            {
                _workFlows = value;
                RaisePropertyChanged(()=>WorkFlows);
            }
        }


        public MainWindowViewModel()
        {
            EditCommand = new RelayCommand(Edit);
            RunCommand=new RelayCommand<WorkFlow>(Run);
            if (!IsInDesignMode)
            {
                ServiceProvider.Instance.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
                ServiceProvider.Instance.DeviceManager.CameraDisconnected += DeviceManager_CameraDisconnected;
                ServiceProvider.Instance.DeviceManager.ConnectToCamera();
                LoadWorkFlows();
            }
        }

        private void Run(WorkFlow obj)
        {
            WorkflowManager.Instance.Context.WorkFlow = obj;
            WorkflowViewView wnd = new WorkflowViewView();
            wnd.ShowDialog();
        }

        private void LoadWorkFlows()
        {
            WorkFlows = new AsyncObservableCollection<WorkFlow>();
            var files = Directory.GetFiles(Settings.Instance.WorkflowFolder, "*.cwpkg");
            foreach (var file in files)
            {
                try
                {
                   WorkFlows.Add(WorkflowManager.Instance.LoadFromPackage(file)); 
                }
                catch (Exception e)
                {
                    Log.Debug("Unable to load package" + file, e);
                }
            }
        }

        private void DeviceManager_CameraDisconnected(CameraControl.Devices.ICameraDevice cameraDevice)
        {
         
        }

        private void DeviceManager_CameraConnected(CameraControl.Devices.ICameraDevice cameraDevice)
        {
         
        }

        private void Edit()
        {
            WorkflowEditorView wnd = new WorkflowEditorView();
            wnd.ShowDialog();
        }
    }
}
