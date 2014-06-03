using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for TagSelectorWnd.xaml
    /// </summary>
    public partial class TagSelectorWnd : IWindow, INotifyPropertyChanged
    {
        private AsyncObservableCollection<TagItem> _tags1;
        public AsyncObservableCollection<TagItem> Tags1
        {
            get { return _tags1; }
            set
            {
                _tags1 = value;
                NotifyPropertyChanged("Tags1");
            }
        }

        private AsyncObservableCollection<TagItem> _tags2;
        public AsyncObservableCollection<TagItem> Tags2
        {
            get { return _tags2; }
            set
            {
                _tags2 = value;
                NotifyPropertyChanged("Tags2");
            }
        }

        private AsyncObservableCollection<TagItem> _tags3;
        public AsyncObservableCollection<TagItem> Tags3
        {
            get { return _tags3; }
            set
            {
                _tags3 = value;
                NotifyPropertyChanged("Tags3");
            }
        }

        private AsyncObservableCollection<TagItem> _tags4;
        public AsyncObservableCollection<TagItem> Tags4
        {
            get { return _tags4; }
            set
            {
                _tags4 = value;
                NotifyPropertyChanged("Tags4");
            }
        }

        public TagSelectorWnd()
        {
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
            //if (Tags1.Count == 0)
            //  Tags1.Add(new TagItem() { DisplayValue = "(empty)" });
        }


        private void LoadData()
        {
            Tags1 =
              new AsyncObservableCollection<TagItem>(ServiceProvider.Settings.DefaultSession.Tags.Where(x => x.Tag1Checked));
            Tags2 =
              new AsyncObservableCollection<TagItem>(ServiceProvider.Settings.DefaultSession.Tags.Where(x => x.Tag2Checked));
            Tags3 =
              new AsyncObservableCollection<TagItem>(ServiceProvider.Settings.DefaultSession.Tags.Where(x => x.Tag3Checked));
            Tags4 =
              new AsyncObservableCollection<TagItem>(ServiceProvider.Settings.DefaultSession.Tags.Where(x => x.Tag4Checked));
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.TagSelectorWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        LoadData();
                        Show();
                        Activate();
                        Topmost = true;
                        Focus();
                        ServiceProvider.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
                    }));
                    break;
                case WindowsCmdConsts.TagSelectorWnd_Hide:
                    ServiceProvider.DeviceManager.PhotoCaptured -= DeviceManager_PhotoCaptured;
                    Hide();
                    break;
                case CmdConsts.All_Close:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        Hide();
                        Close();
                    }));
                    break;
            }
        }

        void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 Activate();
                                                 Focus();
                                                 txt_barcode.Focus();
                                                 txt_barcode.SelectAll();
                                             }));
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.TagSelectorWnd_Hide);
            }
        }

        #region Implementation of INotifyPropertyChanged

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
