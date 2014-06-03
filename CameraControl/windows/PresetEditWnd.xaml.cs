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
using CameraControl.Core;
using CameraControl.Core.Classes;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for PresetEditWnd.xaml
    /// </summary>
    public partial class PresetEditWnd : INotifyPropertyChanged
    {
        private CameraPreset _selectedCameraPreset;

        public CameraPreset SelectedCameraPreset
        {
            get { return _selectedCameraPreset; }
            set
            {
                _selectedCameraPreset = value;
                NotifyPropertyChanged("SelectedCameraPreset");
            }
        }

        public PresetEditWnd()
        {
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void btn_del_preset_Click(object sender, RoutedEventArgs e)
        {
            if (lst_preset.SelectedItem != null)
                ServiceProvider.Settings.CameraPresets.Remove((CameraPreset) lst_preset.SelectedItem);
        }

        private void btn_del_prop_Click(object sender, RoutedEventArgs e)
        {
            if (lst_properties.SelectedItem != null)
                SelectedCameraPreset.Values.Remove((ValuePair) lst_properties.SelectedItem);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ServiceProvider.Settings.Save();
        }
    }
}