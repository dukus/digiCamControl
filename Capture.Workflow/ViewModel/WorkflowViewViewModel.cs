using System;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Devices;
using Capture.Workflow.Classes;
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;

namespace Capture.Workflow.ViewModel
{
    public class WorkflowViewViewModel: ViewModelBase, IDisposable
    {

        private string _currentView;
        private string _preView;
        private UserControl _contents;
        private WorkFlow _workflow;
        private string _title;
        private Context _context;
        private bool _titleBar;
        private WindowState _windowState;
        private WindowStyle _windowStyle;
        private bool _fullScreen;
        private string _displayName;
        private bool _isBusy;

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

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                RaisePropertyChanged(() => DisplayName);
            }
        }


        public Context Context => WorkflowManager.Instance.Context;

        public ICameraDevice CameraDevice
        {
            get { return WorkflowManager.Instance.Context.CameraDevice; }
            set
            {
                WorkflowManager.Instance.Context.CameraDevice = value;
                RaisePropertyChanged(() => CameraDevice);
            }
        }

        public CameraDeviceManager DeviceManager => ServiceProvider.Instance.DeviceManager;

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

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }


        public bool ShowTitleBar => !FullScreen;
        public QueueManager QueueManager => QueueManager.Instance;


        public WorkflowViewViewModel()
        {
            Workflow = WorkflowManager.Instance.Context.WorkFlow;
            var context = WorkflowManager.Instance.Context;
            if (!IsInDesignMode)
            {
                TitleBar = !Workflow.Properties["HideTileBar"].ToBool(context);
                WindowStyle = Workflow.Properties["FullScreen"].ToBool(context) ? WindowStyle.None : WindowStyle.SingleBorderWindow;
                FullScreen = Workflow.Properties["FullScreen"].ToBool(context);
                WorkflowManager.Instance.PreviewSize = Workflow.Properties["PreviewSize"].ToInt(context);
                WindowState = WindowState.Maximized;
                DisplayName = Workflow.Name + " - " + Workflow.Version;
                new PaletteHelper().SetLightDark(Workflow.Properties["BaseColorScheme"].Value == "Dark");
                new PaletteHelper().ReplacePrimaryColor(Workflow.Properties["ColorScheme"].Value);
                CameraDevice = ServiceProvider.Instance.DeviceManager.SelectedCameraDevice;
                WorkflowManager.Instance.Message += Instance_Message;
                ServiceProvider.Instance.DeviceManager.CameraConnected += DeviceManager_CameraConnected;

                GoogleAnalytics.Instance.TrackEvent("Workflow", "Start", Workflow.Name);

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
            else
            {
                TitleBar = true;
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
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => ShowView((string) e.Param)));
                    break;
                case Messages.PreviousView:
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => ShowView(_preView)));
                    break;
                case Messages.ShowHelp:
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => ShowHelp((Context)e.Param)));
                    break;
                case Messages.IsBusy:
                    IsBusy = true;
                    break;
                case Messages.IsNotBusy:
                    IsBusy = false;
                    break;
            }
        }

        private void ShowHelp(Context context)
        {
            if (string.IsNullOrEmpty(context.WorkFlow.Properties["HelpFile"].ToString(context)))
                return;
            try
            {
                var stream = Workflow.GetFileStream(context.WorkFlow.Properties["HelpFile"].ToString(context));
                if (stream != null)
                {
                    var view = new HelpView(stream);
                    view.ShowDialog();
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error("Show help error", e);
            }
        }

        private void ShowView(string viewName)
        {
            if(string.IsNullOrWhiteSpace(viewName))
                return;
            Workflow.Variables.SetValue("CurrentView", viewName);
            GoogleAnalytics.Instance.TrackScreenView(viewName);
            _preView = _currentView;
            _currentView = viewName;
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
                Contents = view.Instance.GetPreview(view, WorkflowManager.Instance.Context);
            }
        }


        public void Dispose()
        {
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
                    Log.Debug("Unable to clean item", e);
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
