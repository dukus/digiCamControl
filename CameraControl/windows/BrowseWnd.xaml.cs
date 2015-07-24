#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for BrowseWnd.xaml
    /// </summary>
    public partial class BrowseWnd : INotifyPropertyChanged, IWindow
    {
        private PhotoSession _selectedPhotoSession;

        public PhotoSession SelectedPhotoSession
        {
            get { return _selectedPhotoSession; }
            set
            {
                _selectedPhotoSession = value;
                NotifyPropertyChanged("SelectedPhotoSession");
            }
        }

        public BrowseWnd()
        {
            InitializeComponent();
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

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.BrowseWnd_Show:
                    ServiceProvider.Settings.PropertyChanged += Settings_PropertyChanged;
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                                                         SelectedPhotoSession = ServiceProvider.Settings.DefaultSession;
                                                         Show();
                                                         Activate();
                                                         Focus();
                                                     }));
                    break;
                case WindowsCmdConsts.BrowseWnd_Hide:
                    {
                        ServiceProvider.Settings.PropertyChanged -= Settings_PropertyChanged;
                        Dispatcher.Invoke(new Action(Hide));
                    }
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

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DefaultSession")
            {
                SelectedPhotoSession = ServiceProvider.Settings.DefaultSession;
            }
        }

        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BrowseWnd_Hide);
            }
        }

        private void lst_profiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lst_profiles.SelectedItem != null)
            {
                ServiceProvider.Settings.DefaultSession = SelectedPhotoSession;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.BrowseWnd_Hide);
            }
        }

        private void lst_files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
    }
}