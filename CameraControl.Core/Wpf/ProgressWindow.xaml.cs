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
using System.Windows.Media;
using CameraControl.Devices;

#endregion

namespace CameraControl.Core.Wpf
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow 
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        public string Label
        {
            get { return (string) lbl_label.Content; }
            set { Dispatcher.Invoke(new Action(delegate { lbl_label.Content = value; })); }
        }

        public string Label2
        {
            get { return (string)lbl_label2.Content; }
            set { Dispatcher.Invoke(new Action(delegate { lbl_label2.Content = value; })); }
        }
        public double Progress
        {
            get { return progressBar.Value; }
            set
            {
                Dispatcher.Invoke(
                    new Action(
                        delegate { progressBar.Value = progressBar.Maximum < value ? value : progressBar.Maximum; }));
            }
        }

        public double MaxValue
        {
            get { return progressBar.Maximum; }
            set { Dispatcher.Invoke(new Action(delegate { progressBar.Maximum = value; })); }
        }

        public new void Hide()
        {
            try
            {
                Dispatcher.Invoke(new Action(() => base.Hide()));
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public ImageSource ImageSource
        {
            get { return image.Source; }
            set
            {
                Dispatcher.Invoke(new Action(delegate
                                                 {
                                                     image.BeginInit();
                                                     image.Source = value;
                                                     image.EndInit();
                                                 }));
            }
        }
    }
}