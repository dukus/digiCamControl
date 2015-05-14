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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;

#endregion

namespace CameraControl.Panels
{
    /// <summary>
    /// Interaction logic for SelectionControl.xaml
    /// </summary>
    public partial class SelectionControl : UserControl
    {
        public SelectionControl()
        {
            SelectAllCommand =
                new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectAll(); });
            SelectNoneCommand =
                new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectNone(); });
            SelectLiked = new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectLiked(); });
            SelectUnLiked =
                new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectUnLiked(); });
            SelectInvertCommand =
                new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectInver(); });
            SelectSeries =
                new RelayCommand<object>(delegate
                {
                    try
                    {
                        ServiceProvider.Settings.DefaultSession.SelectSameSeries(
                            ServiceProvider.Settings.SelectedBitmap.FileItem.Series);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("SelectSeries", ex);
                    }
                });
            InitializeComponent();
        }

        public RelayCommand<object> SelectAllCommand { get; private set; }

        public RelayCommand<object> SelectLiked { get; private set; }

        public RelayCommand<object> SelectUnLiked { get; private set; }


        public RelayCommand<object> SelectNoneCommand { get; private set; }

        public RelayCommand<object> SelectInvertCommand { get; private set; }

        public RelayCommand<object> SelectSeries { get; private set; }
    }
}