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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Vision.Motion;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Color = System.Windows.Media.Color;
using Path = System.IO.Path;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Timer = System.Timers.Timer;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for LiveViewWnd.xaml
    /// </summary>
    public partial class LiveViewWnd : MetroWindow, IWindow, INotifyPropertyChanged
    {

        private ICameraDevice _selectedPortableDevice;
        
        private DateTime _focusMoveTime = DateTime.Now;

        public LiveViewData LiveViewData { get; set; }

        private CameraProperty _cameraProperty;

        public CameraProperty CameraProperty
        {
            get { return _cameraProperty; }
            set
            {
                _cameraProperty = value;
                NotifyPropertyChanged("CameraProperty");
            }
        }

        public ICameraDevice SelectedPortableDevice
        {
            get { return this._selectedPortableDevice; }
            set
            {
                if (this._selectedPortableDevice != value)
                {
                    this._selectedPortableDevice = value;
                    NotifyPropertyChanged("SelectedPortableDevice");
                }
            }
        }

        public LiveViewWnd()
        {
            try
            {
                SelectedPortableDevice = ServiceProvider.DeviceManager.SelectedCameraDevice;
            }
            catch (Exception ex)
            {
                Log.Error("Live view init error ", ex);
            }
            Init();
        }

        public LiveViewWnd(ICameraDevice device)
        {
            try
            {
                SelectedPortableDevice = device;
            }
            catch (Exception ex)
            {
                Log.Error("Live view init error ",ex);
            }
            Init();
        }

        public void Init()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Log.Error("Live view init error ", ex);
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //SelectedPortableDevice.StoptLiveView();
            ServiceProvider.Settings.ApplyTheme(this);
        }


        private void Window_Closed(object sender, EventArgs e)
        {
        }



        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left && 
                _selectedPortableDevice.LiveViewImageZoomRatio.Value == "All")
            {
                try
                {
                    ((LiveViewViewModel) DataContext).SetFocusPos(e.MouseDevice.GetPosition(_image), _image.ActualWidth,
                        _image.ActualHeight);

                }
                catch (Exception exception)
                {
                    Log.Error("Focus Error", exception);
                    StaticHelper.Instance.SystemMessage = "Focus error: " + exception.Message;
                }
            }
        }


        #region Implementation of IWindow

        public void ExecuteCommand(string cmd, object param)
        {

            Dispatcher.Invoke(new Action(delegate
            {
                try
                {
                    if (DataContext != null)
                        ((LiveViewViewModel) (DataContext)).WindowsManager_Event(cmd, param);
                }
                catch (Exception)
                {
                    
                    
                }
            }));
            switch (cmd)
            {
                case WindowsCmdConsts.LiveViewWnd_Show:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        try
                        {
                            ICameraDevice cameraparam = param as ICameraDevice;
                            var properties = cameraparam.LoadProperties();
                            if (properties.SaveLiveViewWindow && properties.WindowRect.Width > 0 && properties.WindowRect.Height>0)
                            {
                                this.Left = properties.WindowRect.Left;
                                this.Top = properties.WindowRect.Top;
                                this.Width = properties.WindowRect.Width;
                                this.Height = properties.WindowRect.Height;
                            }
                            else
                            {
                                this.WindowState = ((Window)ServiceProvider.PluginManager.SelectedWindow).WindowState;                                
                            }

                            if (cameraparam == SelectedPortableDevice && IsVisible)
                            {
                                Activate();
                                Focus();
                                return;
                            }


                            DataContext = new LiveViewViewModel(cameraparam);
                            SelectedPortableDevice = cameraparam;

                            Show();
                            Activate();
                            Focus();

                        }
                        catch (Exception exception)
                        {
                            Log.Error("Error initialize live view window ", exception);
                        }
                    }
                        ));
                    break;
                case WindowsCmdConsts.LiveViewWnd_Hide:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        try
                        {
                            ICameraDevice cameraparam = ((LiveViewViewModel)DataContext).CameraDevice;
                            var properties = cameraparam.LoadProperties();
                            if (properties.SaveLiveViewWindow)
                            {
                                properties.WindowRect = new Rect(this.Left, this.Top, this.Width, this.Height);
                            }
                            ((LiveViewViewModel) DataContext).UnInit();
                        }
                        catch (Exception exception)
                        {
                            Log.Error("Unable to stop live view", exception);
                        }
                        Hide();
                        //ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.FocusStackingWnd_Hide);
                    }));
                    break;
                case WindowsCmdConsts.LiveViewWnd_Message:
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        if (this.IsLoaded)
                            this.ShowMessageAsync("", (string) param);
                        else
                        {
                            MessageBox.Show((string) param);
                        }
                    }));
                }
                    break;
                case CmdConsts.All_Close:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        if (DataContext != null)
                        {
                            ICameraDevice cameraparam = ((LiveViewViewModel)DataContext).CameraDevice;
                            var properties = cameraparam.LoadProperties();
                            if (properties.SaveLiveViewWindow)
                            {
                                properties.WindowRect = new Rect(this.Left, this.Top, this.Width, this.Height);
                            }
                            ((LiveViewViewModel) DataContext).UnInit();
                            Hide();
                            Close();
                        }
                    }));
                    break;
                case CmdConsts.All_Minimize:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        WindowState = WindowState.Minimized;
                    }));
                    break;
                case WindowsCmdConsts.LiveViewWnd_Maximize:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        WindowState = WindowState.Maximized;
                    }));
                    break;
            }
        }

        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsVisible)
            {
                e.Cancel = true;
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Hide, SelectedPortableDevice);
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion


        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((DateTime.Now - _focusMoveTime).TotalMilliseconds < 200)
                return;
            _focusMoveTime = DateTime.Now;
            TriggerClass.KeyDown(e);
        }

        private void canvas_image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left )
            {
                try
                {
                    ((LiveViewViewModel)DataContext).SetFocusPos(e.MouseDevice.GetPosition(_previeImage), _previeImage.ActualWidth,
                        _previeImage.ActualHeight);

                }
                catch (Exception exception)
                {
                    Log.Error("Focus Error", exception);
                    StaticHelper.Instance.SystemMessage = "Focus error: " + exception.Message;
                }
            }

        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.Run(HelpSections.LiveView);
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //CameraProperty.LiveviewSettings.CanvasHeight = slide_vert.ActualHeight;
            //CameraProperty.LiveviewSettings.CanvasWidt = slide_horiz.ActualWidth;
        }

        private void _image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ((LiveViewViewModel)DataContext).CameraDevice.LiveViewImageZoomRatio.NextValue();
            }
            else
            {
                ((LiveViewViewModel)DataContext).CameraDevice.LiveViewImageZoomRatio.PrevValue();
            }
        }

        private void MetroWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ((LiveViewViewModel)DataContext).SetOverlay(files[0]);
                ((LiveViewViewModel) DataContext).OverlayActivated = true;
                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
            }
        }
    }
}