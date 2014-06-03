using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AForge.Imaging.Filters;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for AstroLiveViewWnd.xaml
    /// </summary>
    public partial class AstroLiveViewWnd : IWindow
    {
        private Timer _liveviewtimer = new Timer(1000/14);
        public ICameraDevice CameraDevice { get; set; }
        public LiveViewData LiveViewData { get; set; }
        private bool _oper_in_progress = false;

        public int Brightness { get; set; }
        public bool Freeze { get; set; }
        public Point CentralPoint { get; set; }
        public WriteableBitmap DisplayBitmap { get; set; }
        public int ZoomFactor { get; set; }

        public AstroLiveViewWnd()
        {
            Brightness = 0;
            Freeze = false;
            ZoomFactor = 2;
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
            _liveviewtimer.Elapsed += new ElapsedEventHandler(_liveviewtimer_Elapsed);
            _liveviewtimer.AutoReset = true;
        }

        void _liveviewtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_oper_in_progress)
                return;
            _oper_in_progress = true;
            try
            {
                if (!Freeze)
                    LiveViewData = LiveViewManager.GetLiveViewImage(CameraDevice);
                Dispatcher.Invoke(new Action(DisplayLiveView));
            }
            catch (Exception exception)
            {
                Log.Debug("Error get live view data *Astro ", exception);
            }
            _oper_in_progress = false;
        }

        void DisplayLiveView()
        {
            if (LiveViewData == null || LiveViewData.ImageData == null)
                return;
            MemoryStream stream = new MemoryStream(LiveViewData.ImageData,
                                                   LiveViewData.ImageDataPosition,
                                                   LiveViewData.ImageData.Length -
                                                   LiveViewData.ImageDataPosition);
            using (var bmp = new Bitmap(stream))
            {
                Bitmap res = bmp;
                if(Brightness!=0)
                {
                    BrightnessCorrection filter = new BrightnessCorrection(Brightness);
                    res = filter.Apply(res);
                }

                DisplayBitmap = BitmapFactory.ConvertToPbgra32Format(BitmapSourceConvert.ToBitmapSource(res));
                DrawGrid(DisplayBitmap);
                DisplayBitmap.Freeze();
                live_view_image.Source = DisplayBitmap;
            }
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
                        Show();
                        Activate();
                        Topmost = true;
                        //Topmost = false;
                        Focus();
                    }));
                    break;
                case WindowsCmdConsts.AstroLiveViewWnd_Hide:
                    StopLiveView();
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

        private void btn_start_lv_Click(object sender, RoutedEventArgs e)
        {
            StartLiveView();
        }

        private void StartLiveView()
        {
            if (!IsVisible)
                return;
            string resp = CameraDevice.GetProhibitionCondition(OperationEnum.LiveView);
            if (string.IsNullOrEmpty(resp))
            {
                Thread thread = new Thread(StartLiveViewThread);
                thread.Start();
                thread.Join();
            }
            else
            {
                Log.Error("Error starting live view " + resp);
                MessageBox.Show(TranslationStrings.LabelLiveViewError + "\n" + TranslationManager.GetTranslation(resp));
                return;
            }
        }

        private void StartLiveViewThread()
        {
            try
            {
                bool retry = false;
                int retryNum = 0;
                Log.Debug("LiveView: Liveview started");
                do
                {
                    try
                    {
                        LiveViewManager.StartLiveView(CameraDevice);
                    }
                    catch (DeviceException deviceException)
                    {
                        if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                            deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                        {
                            Thread.Sleep(200);
                            if (!IsVisible)
                                break;
                            Log.Debug("Retry live view :" + deviceException.ErrorCode.ToString("X"));
                            retry = true;
                            retryNum++;
                        }
                        else
                        {
                            throw;
                        }
                    }

                } while (retry && retryNum < 35);
                if (IsVisible)
                {
                    _oper_in_progress = false;
                    _liveviewtimer.Start();
                    Log.Debug("LiveView: Liveview start done");
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to start liveview !", exception);
                StaticHelper.Instance.SystemMessage = "Unable to start liveview ! " + exception.Message;
                //MessageBox.Show("Unable to start liveview !");
                //ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.LiveViewWnd_Hide);
            }
        }

        private void StopLiveView()
        {
            Thread thread = new Thread(StopLiveViewThread);
            thread.Start();
        }

        private void StopLiveViewThread()
        {
            try
            {
                bool retry = false;
                int retryNum = 0;
                Log.Debug("LiveView: Liveview stopping");
                do
                {
                    try
                    {
                        LiveViewManager.StopLiveView(CameraDevice);
                    }
                    catch (DeviceException deviceException)
                    {
                        
                        if (deviceException.ErrorCode == ErrorCodes.ERROR_BUSY ||
                            deviceException.ErrorCode == ErrorCodes.MTP_Device_Busy)
                        {
                            Thread.Sleep(500);
                            Log.Debug("Retry live view stop:" + deviceException.ErrorCode.ToString("X"));
                            retry = true;
                            retryNum++;
                        }
                        else
                        {
                            throw;
                        }
                    }

                } while (retry && retryNum < 35);
            }
            catch (Exception exception)
            {
                Log.Error("Unable to stop liveview !", exception);
                StaticHelper.Instance.SystemMessage = "Unable to stop liveview ! " + exception.Message;
            }
        }

        private void btn_stop_lv_Click(object sender, RoutedEventArgs e)
        {
            StopLiveView();
        }

        private void live_view_image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left && DisplayBitmap!=null)
            {
                Point point = e.MouseDevice.GetPosition(live_view_image);
                double dw = DisplayBitmap.PixelWidth/live_view_image.ActualWidth;
                double hw = DisplayBitmap.PixelHeight/live_view_image.ActualHeight;
                CentralPoint = new Point(point.X*dw, point.Y*hw);
            }
        }

        private void DrawGrid(WriteableBitmap bitmap)
        {
            if(CentralPoint.X==0&& CentralPoint.Y==0)
            {
                CentralPoint = new Point(bitmap.PixelWidth/2, bitmap.PixelHeight/2);
            }
            WriteableBitmap tempbitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX,
                                                             bitmap.DpiY, PixelFormats.Pbgra32, bitmap.Palette);

            tempbitmap.DrawLine(0, (int) CentralPoint.Y - 5, bitmap.PixelWidth, (int) CentralPoint.Y - 5, Colors.White);
            tempbitmap.DrawLine(0, (int) CentralPoint.Y + 5, bitmap.PixelWidth, (int) CentralPoint.Y + 5, Colors.White);

            tempbitmap.DrawLine((int) CentralPoint.X - 5, 0, (int) CentralPoint.X - 5, bitmap.PixelHeight, Colors.White);
            tempbitmap.DrawLine((int) CentralPoint.X + 5, 0, (int) CentralPoint.X + 5, bitmap.PixelHeight, Colors.White);

            bitmap.Blit(new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), tempbitmap,
                        new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            
            DrawPreviewImage(bitmap);
        }

        private void DrawPreviewImage(WriteableBitmap bitmap)
        {
            if (live_view_image.ActualWidth == 0 || panel_preview.ActualWidth == 0)
                return;
            double dw = panel_preview.ActualWidth / live_view_image.ActualWidth;
            double dh = panel_preview.ActualHeight / live_view_image.ActualHeight;
            double d = bitmap.PixelWidth*dw/ZoomFactor;
            double h = bitmap.PixelHeight*dh/ZoomFactor;
            WriteableBitmap tempbitmap = bitmap.Crop((int) (CentralPoint.X - (d/2)), (int) (CentralPoint.Y - (h/2)), (int) d, (int) h);
            tempbitmap.Freeze();
            img_preview.Source = tempbitmap;
        }

        private void btn_stay_on_top_Click(object sender, RoutedEventArgs e)
        {
            Topmost = (btn_stay_on_top.IsChecked == true);
        }

    }
}
