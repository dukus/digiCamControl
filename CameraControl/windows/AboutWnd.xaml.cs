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

using System.IO;
using System.Windows;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for AboutWnd.xaml
    /// </summary>
    public partial class AboutWnd
    {
        public AboutWnd()
        {
            InitializeComponent();
            Title = "About " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var file = Path.Combine(Settings.ApplicationFolder, "about.txt");
            if (File.Exists(file))
            {
                textBlock2.Visibility = Visibility.Hidden;
                textBlock1.Text = File.ReadAllText(file);
                btn_donate.Visibility = Visibility.Collapsed;
                button1.Visibility = Visibility.Collapsed;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            PhotoUtils.Run("http://www.digicamcontrol.com/");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            PhotoUtils.Run("http://www.gnu.org/licenses/gpl-3.0.txt");
        }

        private void btn_donate_Click(object sender, RoutedEventArgs e)
        {
            PhotoUtils.Donate();
        }
    }
}