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
using System.Windows.Navigation;
using System.Windows.Shapes;

#endregion

namespace CameraControl.Controls
{
    /// <summary>
    /// Interaction logic for FolderBrowserComboBox.xaml
    /// </summary>
    public partial class FolderBrowserComboBox : UserControl, INotifyPropertyChanged
    {
        public event EventHandler ValueChanged;

        public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register(
            "SelectedPath", typeof (string), typeof (FolderBrowserComboBox), new PropertyMetadata(""));

        public string SelectedPath
        {
            get { return folderBrowser.SelectedImagePath; }
            set
            {
                folderBrowser.SelectedImagePath = value;
                NotifyPropertyChanged("SelectedPath");
            }
        }


        public FolderBrowserComboBox()
        {
            InitializeComponent();
        }


        private void Tree1_Initialized(object sender, EventArgs e)
        {
            var trv = sender as TreeView;
            var trvItem = new TreeViewItem() {Header = "Initialized item"};
            var trvItemSel = trv.Items[1] as TreeViewItem;
            trvItemSel.Items.Add(trvItem);
        }

        private void header_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!PopupTest.IsOpen)
            {
                PopupTest.Placement = System.Windows.Controls.Primitives.PlacementMode.RelativePoint;
                PopupTest.VerticalOffset = header.ActualHeight;
                PopupTest.StaysOpen = true;
                PopupTest.Height = folderBrowser.Height;
                PopupTest.Width = header.ActualWidth;
                PopupTest.IsOpen = true;
            }
            else
            {
                PopupTest.IsOpen = false;
            }
        }

        private void folderBrowser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //header.Text = folderBrowser.SelectedImagePath;
            if (ValueChanged != null)
                ValueChanged(this, new EventArgs());
            PopupTest.IsOpen = false;
        }

        private void header_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PopupTest.IsOpen = false;
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

        private void PopupTest_Opened(object sender, EventArgs e)
        {
        }
    }
}