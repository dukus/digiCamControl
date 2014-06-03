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
using System.Threading;
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

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for ActionExecuteWnd.xaml
    /// </summary>
    public partial class ActionExecuteWnd
    {
        public IMenuAction MenuAction { get; set; }

        public ActionExecuteWnd(IMenuAction action)
        {
            MenuAction = action;
            MenuAction.ProgressChanged += MenuAction_ProgressChanged;
            MenuAction.ActionDone += MenuAction_ActionDone;
            InitializeComponent();
            Title = "Execute action : " + MenuAction.Title;
            ServiceProvider.Settings.ApplyTheme(this);
        }

        private void MenuAction_ActionDone(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 progressBar1.IsIndeterminate = false;
                                                 button1.Content = "Start";
                                                 listBox1.Items.Add("Action done");
                                             }));
        }

        private void MenuAction_ProgressChanged(object sender, EventArgs e)
        {
            ActionEventArgs args = e as ActionEventArgs;
            if (args != null)
            {
                Dispatcher.Invoke(new Action(delegate
                                                 {
                                                     listBox1.Items.Add(args.Message);
                                                     listBox1.ScrollIntoView(listBox1.Items[listBox1.Items.Count - 1]);
                                                 }));
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!MenuAction.IsBusy)
            {
                progressBar1.IsIndeterminate = true;

                listBox1.Items.Clear();
                Thread thread = new Thread(() => MenuAction.Run(null));
                thread.Start();
                button1.Content = "Stop";
            }
            else
            {
                MenuAction.Stop();
            }
        }
    }
}