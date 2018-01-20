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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Web.UI.WebControls.WebParts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Accord.Imaging.Filters;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.ViewModel;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for AstroLiveViewWnd.xaml
    /// </summary>
    public partial class AstroLiveViewWnd : IWindow
    {
        public ICameraDevice CameraDevice { get; set; }
        public LiveViewData LiveViewData { get; set; }

        public int Brightness { get; set; }
        public bool Freeze { get; set; }
        public Point CentralPoint { get; set; }
        //public WriteableBitmap DisplayBitmap { get; set; }
        //public int ZoomFactor { get; set; }

        public AstroLiveViewWnd()
        {
            Brightness = 0;
            Freeze = false;
            InitializeComponent();
        }

        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {
            switch (cmd)
            {
                case WindowsCmdConsts.AstroLiveViewWnd_Show:
                    CameraDevice = param as ICameraDevice;
                    if (CameraDevice == null)
                        return;
                    Dispatcher.Invoke(new Action(delegate
                                                     {
                                                         DataContext = new AstroLiveViewViewModel(CameraDevice,this);
                                                         Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                                                         Show();
                                                         Activate();
                                                         Focus();
                                                     }));
                    break;
                case WindowsCmdConsts.AstroLiveViewWnd_Hide:
                    try
                    {
                        ((AstroLiveViewViewModel)DataContext).UnInit();
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Unable to stop live view", exception);
                    }
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

        #endregion

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.AstroLiveViewWnd_Hide);
            }
        }

        private void live_view_image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left &&((AstroLiveViewViewModel)DataContext).Bitmap!=null && ((AstroLiveViewViewModel)DataContext).ZoomFactor==1)
            {
                Point point = e.MouseDevice.GetPosition(live_view_image);
                double dw = ((AstroLiveViewViewModel)DataContext).Bitmap.PixelWidth / live_view_image.ActualWidth;
                double hw = ((AstroLiveViewViewModel)DataContext).Bitmap.PixelHeight / live_view_image.ActualHeight;
                ((AstroLiveViewViewModel)DataContext).CentralPoint = new Point(point.X * dw, point.Y * hw);
                if (((AstroLiveViewViewModel) DataContext).IsFullImage)
                {
                    ((LiveViewViewModel)DataContext).SetFocusPos(point, live_view_image.ActualWidth, live_view_image.ActualHeight);
                }
            }
        }

        private void img_preview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left && ((AstroLiveViewViewModel)DataContext).Bitmap != null)
            {
                Point point = e.MouseDevice.GetPosition(img_preview);
                if (((AstroLiveViewViewModel) DataContext).IsFullImage)
                {
                    double dw = ((AstroLiveViewViewModel) DataContext).Preview.PixelWidth/img_preview.ActualWidth;
                    double hw = ((AstroLiveViewViewModel) DataContext).Preview.PixelHeight/img_preview.ActualHeight;
                    ((AstroLiveViewViewModel) DataContext).CentralPoint = new Point(point.X*dw, point.Y*hw);
                }
                try
                {
                    ((LiveViewViewModel)DataContext).SetFocusPos(point, img_preview.ActualWidth,img_preview.ActualHeight);
                }
                catch (Exception exception)
                {
                    Log.Error("Focus Error", exception);
                    StaticHelper.Instance.SystemMessage = "Focus error: " + exception.Message;
                }

            }
        }
    }
}