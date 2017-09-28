using System;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Devices;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Dragablz;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;

namespace Capture.Workflow.ViewModel
{
    public class WorkflowViewViewModel: ViewModelBase, IDisposable
    {
        private UserControl _contents;
        private WorkFlow _workflow;
        private string _title;
        private Context _context;
        private bool _titleBar;
        private WindowState _windowState;
        private WindowStyle _windowStyle;
        private bool _fullScreen;

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

        public bool TitleBar
        {
            get { return _titleBar; }
            set
            {
                _titleBar = value;
                RaisePropertyChanged(() => TitleBar);
            }
        }

        public WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                _windowState = value;
                RaisePropertyChanged(() => WindowState);
            }
        }

        public WindowStyle WindowStyle
        {
            get { return _windowStyle; }
            set
            {
                _windowStyle = value;
                RaisePropertyChanged(() => WindowStyle);
            }
        }

        public bool FullScreen
        {
            get { return _fullScreen; }
            set
            {
                _fullScreen = value;
                RaisePropertyChanged(() => FullScreen);
                RaisePropertyChanged(() => ShowTitleBar);
            }
        }

        public bool ShowTitleBar => !FullScreen;



        public WorkflowViewViewModel()
        {
            Workflow = WorkflowManager.Instance.Context.WorkFlow;

            if (!IsInDesignMode)
            {
                TitleBar = !Workflow.Properties["HideTileBar"].ToBool();
                WindowStyle = Workflow.Properties["FullScreen"].ToBool() ? WindowStyle.None : WindowStyle.SingleBorderWindow;
                FullScreen = Workflow.Properties["FullScreen"].ToBool();
                WindowState = WindowState.Maximized;

                new PaletteHelper().SetLightDark(Workflow.Properties["BaseColorScheme"].Value == "Dark");
                new PaletteHelper().ReplacePrimaryColor(Workflow.Properties["ColorScheme"].Value);
                CameraDevice = ServiceProvider.Instance.DeviceManager.SelectedCameraDevice;
                WorkflowManager.Instance.Message += Instance_Message;
                ServiceProvider.Instance.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
                foreach (WorkFlowEvent workflowEvent in Workflow.Events)
                {
                    try
                    {
                        workflowEvent.Instance.RegisterEvent(workflowEvent);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Unable to register event " + workflowEvent?.Name, e);
                    }
                }
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
                    App.Current.Dispatcher.BeginInvoke(new Action(() => ShowView((string) e.Param)));
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
            WorkflowManager.Instance.OnMessage(
                new MessageEventArgs(Messages.SessionFinished, WorkflowManager.Instance.Context)); 

            WorkflowManager.Instance.Message -= Instance_Message;
            ServiceProvider.Instance.DeviceManager.CameraConnected -= DeviceManager_CameraConnected;
            foreach (WorkFlowEvent workflowEvent in Workflow.Events)
            {
                try
                {
                    workflowEvent.Instance.UnRegisterEvent(workflowEvent);
                }
                catch (Exception e)
                {
                    Log.Error("Unable to unregister event " + workflowEvent?.Name, e);
                }
            }
            foreach (FileItem item in WorkflowManager.Instance.FileItems)
            {
                try
                {
                    item.Clear();
                }
                catch (Exception e)
                {
                    Log.Debug("Unable to clean item");
                }
            }
            WorkflowManager.Instance.FileItems.Clear();

            if (Contents?.DataContext != null)
            {
                // dispose old view if was loaded
                var obj = Contents.DataContext as IDisposable;
                obj?.Dispose();
            }
            new PaletteHelper().SetLightDark(false);
            new PaletteHelper().ReplacePrimaryColor("blue");
        }
    }
}
