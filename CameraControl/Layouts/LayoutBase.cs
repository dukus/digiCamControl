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
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Classes.Queue;
using CameraControl.Core.Controls.ZoomAndPan;
using CameraControl.Core.Wpf;
using CameraControl.Devices;
using CameraControl.ViewModel;
using Microsoft.VisualBasic.FileIO;

#endregion

namespace CameraControl.Layouts
{
    public class LayoutBase : UserControl
    {
        /// <summary>


        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        protected bool prevZoomRectSet = false;

        public ListBox ImageLIst { get; set; }
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private FileItem _selectedItem = null;
        public ZoomAndPanControl ZoomAndPanControl { get; set; }
        public UIElement content { get; set; }
        public MediaElement MediaElement { get; set; }

        public LayoutViewModel LayoutViewModel { get; set; }

        public LayoutBase()
        {
            LayoutViewModel = new LayoutViewModel();
            _worker.DoWork += worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }

        public void UnInit()
        {
            if (ZoomAndPanControl != null)
            {
                ZoomAndPanControl.ContentScaleChanged -= ZoomAndPanControl_ContentScaleChanged;
                ZoomAndPanControl.ContentOffsetXChanged -= ZoomAndPanControl_ContentScaleChanged;
                ZoomAndPanControl.ContentOffsetYChanged -= ZoomAndPanControl_ContentScaleChanged;
            }
            _worker.DoWork -= worker_DoWork;
            _worker.RunWorkerCompleted -= _worker_RunWorkerCompleted;
            ServiceProvider.Settings.PropertyChanged -= Settings_PropertyChanged;
            ServiceProvider.WindowsManager.Event -= Trigger_Event;
            ImageLIst.SelectionChanged -= ImageLIst_SelectionChanged;
        }


        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_selectedItem != ServiceProvider.Settings.SelectedBitmap.FileItem)
            {
                ServiceProvider.Settings.SelectedBitmap.FileItem = _selectedItem;
                _worker.RunWorkerAsync(_selectedItem);
            }
            else
            {
                if (LayoutViewModel.ZoomIndex>0)
                {
                    LoadFullRes();
                }
            }
        }

        private void DeleteItem()
        {
            List<FileItem> filestodelete = new List<FileItem>();
            try
            {
                filestodelete.AddRange(
                    ServiceProvider.Settings.DefaultSession.Files.Where(fileItem => fileItem.IsChecked));

                if (ServiceProvider.Settings.SelectedBitmap != null &&
                    ServiceProvider.Settings.SelectedBitmap.FileItem != null &&
                    filestodelete.Count == 0)
                    filestodelete.Add(ServiceProvider.Settings.SelectedBitmap.FileItem);

                if (filestodelete.Count == 0)
                    return;
                int selectedindex = ImageLIst.Items.IndexOf(filestodelete[0]);

                bool delete = false;
                if (filestodelete.Count > 1)
                {
                    delete = MessageBox.Show("Multile files are selected !! Do you really want to delete selected files ?", "Delete files",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                }
                else
                {
                    delete = MessageBox.Show("Do you really want to delete selected file ?", "Delete file",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes;

                }
                if (delete)
                {
                    foreach (FileItem fileItem in filestodelete)
                    {
                        if ((ServiceProvider.Settings.SelectedBitmap != null &&
                             ServiceProvider.Settings.SelectedBitmap.FileItem != null &&
                             fileItem.FileName == ServiceProvider.Settings.SelectedBitmap.FileItem.FileName))
                        {
                            ServiceProvider.Settings.SelectedBitmap.DisplayImage = null;
                        }
                        if (File.Exists(fileItem.FileName))
                            FileSystem.DeleteFile(fileItem.FileName, UIOption.OnlyErrorDialogs,
                                                  RecycleOption.SendToRecycleBin);
                        fileItem.RemoveThumbs();
                        ServiceProvider.Settings.DefaultSession.Files.Remove(fileItem);
                    }
                    if (selectedindex < ImageLIst.Items.Count)
                    {
                        ImageLIst.SelectedIndex = selectedindex + 1;
                        ImageLIst.SelectedIndex = selectedindex - 1;
                        FileItem item = ImageLIst.SelectedItem as FileItem;

                        if (item != null)
                            ImageLIst.ScrollIntoView(item);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error to delete file", exception);
            }
        }

        public void InitServices()
        {
            ServiceProvider.Settings.PropertyChanged += Settings_PropertyChanged;
            ServiceProvider.WindowsManager.Event += Trigger_Event;
            ImageLIst.SelectionChanged += ImageLIst_SelectionChanged;
            if (ServiceProvider.Settings.SelectedBitmap != null &&
                ServiceProvider.Settings.SelectedBitmap.FileItem != null)
            {
                ImageLIst.SelectedItem = ServiceProvider.Settings.SelectedBitmap.FileItem;
                ImageLIst.ScrollIntoView(ImageLIst.SelectedItem);
            }
            else
            {
                if (ServiceProvider.Settings.DefaultSession.Files.Count > 0)
                    ImageLIst.SelectedIndex = 0;
            }
            if (ZoomAndPanControl != null)
            {
                ZoomAndPanControl.ContentScaleChanged += ZoomAndPanControl_ContentScaleChanged;
                ZoomAndPanControl.ContentOffsetXChanged += ZoomAndPanControl_ContentScaleChanged;
                ZoomAndPanControl.ContentOffsetYChanged += ZoomAndPanControl_ContentScaleChanged;
            }
        }

        private void ZoomAndPanControl_ContentScaleChanged(object sender, EventArgs e)
        {
            GeneratePreview();
            var i = Math.Round(ZoomAndPanControl.FitScale(), 4);
            var freeZoom = Math.Round(ZoomAndPanControl.ContentScale, 4) >
                                       Math.Round(ZoomAndPanControl.FitScale(), 4);
            if (!freeZoom)
            {
                LayoutViewModel.RefresZoomIndex(0);
            }
        }

        private void GeneratePreview()
        {
            try
            {
                var bitmap = BitmapLoader.Instance.LoadSmallImage(ServiceProvider.Settings.SelectedBitmap.FileItem, 270);


                if (bitmap != null)
                {
                    if (ZoomAndPanControl == null)
                    {
                        bitmap.Freeze();
                        ServiceProvider.Settings.SelectedBitmap.Preview = bitmap;
                    }
                    else
                    {
                        if (ServiceProvider.Settings.SelectedBitmap.FileItem.IsMovie)
                        {
                            ServiceProvider.Settings.SelectedBitmap.Preview = bitmap;
                            return;
                        }
                        int dw = (int)(ZoomAndPanControl.ContentViewportWidthRation * bitmap.PixelWidth);
                        int dh = (int)(ZoomAndPanControl.ContentViewportHeightRation * bitmap.PixelHeight);
                        int fw = (int)(ZoomAndPanControl.ContentZoomFocusXRation * bitmap.PixelWidth);
                        int fh = (int)(ZoomAndPanControl.ContentZoomFocusYRation * bitmap.PixelHeight);

                        bitmap.FillRectangle2(0, 0, bitmap.PixelWidth, bitmap.PixelHeight,
                            Color.FromArgb(128, 128, 128, 128));
                        bitmap.FillRectangleDeBlend(fw - (dw / 2), fh - (dh / 2), fw + (dw / 2), fh + (dh / 2),
                            Color.FromArgb(128, 128, 128, 128));
                        bitmap.DrawRectangle(fw - (dw / 2), fh - (dh / 2), fw + (dw / 2), fh + (dh / 2), Colors.White);
                        bitmap.Freeze();
                        ServiceProvider.Settings.SelectedBitmap.Preview = bitmap;
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        private void ImageLIst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _selectedItem = e.AddedItems[0] as FileItem;
                if (_worker.IsBusy)
                    return;
                FileItem item = e.AddedItems[0] as FileItem;
                if (item != null)
                {
                    ServiceProvider.Settings.SelectedBitmap.SetFileItem(item);
                    _worker.RunWorkerAsync(false);
                }
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var item = ServiceProvider.Settings.SelectedBitmap.FileItem;

            if (ServiceProvider.Settings.SelectedBitmap.FileItem == null)
                return;
            //bool fullres = e.Argument is bool && (bool) e.Argument ||LayoutViewModel.ZoomFit
            //bool fullres = !LayoutViewModel.ZoomFit;
            bool fullres = e.Argument is bool && (bool)e.Argument;

            if ((!File.Exists(item.LargeThumb) && item.IsJpg && File.Exists(item.FileName)))
            {
                try
                {
                    PhotoUtils.WaitForFile(item.FileName);
                    ServiceProvider.Settings.SelectedBitmap.DisplayImage = (WriteableBitmap)BitmapLoader.Instance.LoadImage(item.FileName, BitmapLoader.LargeThumbSize,
                        0);
                }
                catch (Exception ex)
                {
                    Log.Error("Unable to load fast preview", ex);
                }
            }

            ServiceProvider.Settings.ImageLoading = fullres ||
                                                    !ServiceProvider.Settings.SelectedBitmap.FileItem.IsLoaded;
            if (ServiceProvider.Settings.SelectedBitmap.FileItem.Loading)
            {
                while (ServiceProvider.Settings.SelectedBitmap.FileItem.Loading)
                {
                    Thread.Sleep(10);
                }
            }
            else
            {
                BitmapLoader.Instance.GenerateCache(ServiceProvider.Settings.SelectedBitmap.FileItem);
            }
            if (!ServiceProvider.Settings.SelectedBitmap.FileItem.HaveHistogramReady())
                ServiceProvider.QueueManager.Add(new QueueItemFileItem
                {
                    FileItem = ServiceProvider.Settings.SelectedBitmap.FileItem,
                    Generate = QueueType.Histogram
                });

            ServiceProvider.Settings.SelectedBitmap.DisplayImage =
                BitmapLoader.Instance.LoadImage(ServiceProvider.Settings.SelectedBitmap.FileItem, fullres);
            ServiceProvider.Settings.SelectedBitmap.Notify();
            BitmapLoader.Instance.SetData(ServiceProvider.Settings.SelectedBitmap,
                              ServiceProvider.Settings.SelectedBitmap.FileItem);
            BitmapLoader.Highlight(ServiceProvider.Settings.SelectedBitmap,
                                            ServiceProvider.Settings.HighlightUnderExp,
                                            ServiceProvider.Settings.HighlightOverExp);
            ServiceProvider.Settings.SelectedBitmap.FullResLoaded = fullres;
            ServiceProvider.Settings.ImageLoading = false;
            GC.Collect();
            Dispatcher.BeginInvoke(new Action(OnImageLoaded));
        }

        public virtual void OnImageLoaded()
        {
            if (LayoutViewModel.ZoomIndex==0 && ZoomAndPanControl != null)
            {
                ZoomAndPanControl.ScaleToFit();
            }
            else
            {
                if (ZoomAndPanControl != null)
                    ZoomToFocus();
            }
            GeneratePreview();
        }

        public void LoadFullRes()
        {
            if (_worker.IsBusy)
                return;
            if (ServiceProvider.Settings.SelectedBitmap.FullResLoaded)
                return;
            _worker.RunWorkerAsync(true);
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DefaultSession")
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(new Action(delegate
                                                 {
                                                     ImageLIst.SelectedIndex = 0;
                                                     if (ImageLIst.Items.Count == 0)
                                                     {
                                                         ServiceProvider.Settings.SelectedBitmap.DisplayImage = null;
                                                         ServiceProvider.Settings.SelectedBitmap.Preview = null;
                                                     }
                                                 }));
            }
            if (e.PropertyName == "HighlightOverExp")
            {
                if (!_worker.IsBusy)
                {
                    _worker.RunWorkerAsync(false);
                }
            }
            if (e.PropertyName == "HighlightUnderExp")
            {
                if (!_worker.IsBusy)
                {
                    _worker.RunWorkerAsync(false);
                }
            }
            if (e.PropertyName == "ShowFocusPoints")
            {
                if (!_worker.IsBusy)
                {
                    _worker.RunWorkerAsync(false);
                }
            }
            if (e.PropertyName == "FlipPreview")
            {
                if (!_worker.IsBusy)
                {
                    _worker.RunWorkerAsync(false);
                }
            }
        }

        private void TriggerEvent(string cmd, object o)
        {
            try
            {
                switch (cmd)
                {
                    case WindowsCmdConsts.Next_Image:
                        if (ImageLIst.SelectedIndex <
                            ImageLIst.Items.Count - 1)
                        {
                            FileItem item =
                                ImageLIst.SelectedItem as FileItem;
                            if (item != null)
                            {
                                int ind = ImageLIst.Items.IndexOf(item);
                                ImageLIst.SelectedIndex = ind + 1;
                            }
                            item = ImageLIst.SelectedItem as FileItem;
                            if (item != null)
                                ImageLIst.ScrollIntoView(item);
                        }
                        break;
                    case WindowsCmdConsts.Prev_Image:
                        if (ImageLIst.SelectedIndex > 0)
                        {
                            FileItem item =
                                ImageLIst.SelectedItem as FileItem;
                            if (item != null)
                            {
                                int ind = ImageLIst.Items.IndexOf(item);
                                ImageLIst.SelectedIndex = ind - 1;
                            }
                            item = ImageLIst.SelectedItem as FileItem;
                            if (item != null)
                                ImageLIst.ScrollIntoView(item);
                        }
                        break;
                    case WindowsCmdConsts.Like_Image:
                        if (ImageLIst.SelectedItem != null)
                        {
                            FileItem item = null;
                            if (o != null)
                            {
                                item = ServiceProvider.Settings.DefaultSession.GetByName(o as string);
                            }
                            else
                            {
                                item = ImageLIst.SelectedItem as FileItem;
                            }
                            if (item != null)
                            {
                                item.IsLiked = !item.IsLiked;
                            }
                        }
                        break;
                    case WindowsCmdConsts.Unlike_Image:
                        if (ImageLIst.SelectedItem != null)
                        {
                            FileItem item = null;
                            if (o != null)
                            {
                                item =
                                    ServiceProvider.Settings.DefaultSession
                                        .GetByName(o as string);
                            }
                            else
                            {
                                item = ImageLIst.SelectedItem as FileItem;
                            }
                            if (item != null)
                            {
                                item.IsUnLiked = !item.IsUnLiked;
                            }
                        }
                        break;
                    case WindowsCmdConsts.Del_Image:
                        {
                            DeleteItem();
                        }
                        break;
                    case WindowsCmdConsts.SelectAll_Image:
                        {
                            ServiceProvider.Settings.DefaultSession.SelectAll();
                        }
                        break;
                    case WindowsCmdConsts.Select_Image:
                        FileItem fileItem = o as FileItem;
                        if (fileItem != null)
                        {
                            ImageLIst.SelectedValue = fileItem;
                            ImageLIst.ScrollIntoView(fileItem);
                        }
                        break;
                    case WindowsCmdConsts.Refresh_Image:
                        RefreshImage();
                        break;
                    case WindowsCmdConsts.Zoom_Image_Fit:
                        ZoomAndPanControl.ScaleToFit();
                        break;
                    case WindowsCmdConsts.Zoom_Image_60:
                        ZoomToFocus();
                        LoadFullRes();
                        ZoomAndPanControl.ZoomTo(0.6);
                        break;
                    case WindowsCmdConsts.Zoom_Image_100:
                        ZoomToFocus();
                        LoadFullRes();
                        ZoomAndPanControl.ZoomTo(1.0);
                        break;
                    case WindowsCmdConsts.Zoom_Image_200:
                        ZoomToFocus();
                        LoadFullRes();
                        ZoomAndPanControl.ZoomTo(2.0);
                        break;
                    case WindowsCmdConsts.RotateLeft:
                        {
                            FileItem item =
                                ImageLIst.SelectedItem as FileItem;
                            if (item != null)
                            {
                                item.Rotation--;

                            }
                        }
                        break;
                    case WindowsCmdConsts.RotateRight:
                        {
                            FileItem item =
                                ImageLIst.SelectedItem as FileItem;
                            if (item != null)
                            {
                                item.Rotation++;
                            }
                        }
                        break;
                    case WindowsCmdConsts.ViewExternal:
                        OpenInExternalViewer();
                        break;
                    case WindowsCmdConsts.ViewExplorer:
                        OpenInExplorer();
                        break;
                    case WindowsCmdConsts.RefreshDisplay:
                        if (LayoutViewModel.ZoomIndex == 0)
                            ZoomAndPanControl.ScaleToFit();
                        break;
                }
                if (cmd.StartsWith(WindowsCmdConsts.ZoomPoint))
                {
                    if (ZoomAndPanControl != null && cmd.Contains("_"))
                    {
                        var vals = cmd.Split('_');
                        if (vals.Count() > 2)
                        {
                            double x;
                            double y;
                            double.TryParse(vals[1], out x);
                            double.TryParse(vals[2], out y);
                            if (cmd.EndsWith("!"))
                                ZoomAndPanControl.SnapToRation(x, y);
                            else
                            {
                                ZoomAndPanControl.AnimatedSnapToRation(x, y);
                            }

                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to process event ", exception);
            }

        }

        private void OpenInExplorer()
        {
            if (ServiceProvider.Settings.SelectedBitmap == null ||
                ServiceProvider.Settings.SelectedBitmap.FileItem == null)
                return;
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "explorer";
                processStartInfo.UseShellExecute = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                processStartInfo.Arguments =
                    string.Format("/e,/select,\"{0}\"", ServiceProvider.Settings.SelectedBitmap.FileItem.FileName);
                Process.Start(processStartInfo);
            }
            catch (Exception exception)
            {
                Log.Error("Error to show file in explorer", exception);
            }
        }

        private void OpenInExternalViewer()
        {
            if (ServiceProvider.Settings.SelectedBitmap == null ||
                ServiceProvider.Settings.SelectedBitmap.FileItem == null)
                return;
            if (!string.IsNullOrWhiteSpace(ServiceProvider.Settings.ExternalViewer) &&
                File.Exists(ServiceProvider.Settings.ExternalViewer))
            {
                PhotoUtils.Run(ServiceProvider.Settings.ExternalViewer,
                    "\"" + ServiceProvider.Settings.SelectedBitmap.FileItem.FileName + "\"",
                    ProcessWindowStyle.Maximized);
            }
            else
            {
                PhotoUtils.Run("\"" + ServiceProvider.Settings.SelectedBitmap.FileItem.FileName + "\"", "",
                    ProcessWindowStyle.Maximized);
            }
        }

        private void Trigger_Event(string cmd, object o)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => TriggerEvent(cmd, o)));
            }
            catch (Exception)
            {


            }
        }

        private void RefreshImage()
        {
            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync(false);
            }
        }

        private void ZoomToFocus()
        {
            if (!LayoutViewModel.ZoomToFocus)
                return;
            if (
                ServiceProvider.Settings.SelectedBitmap.FileItem
                    .FileInfo != null &&
                ServiceProvider.Settings.SelectedBitmap.FileItem
                    .FileInfo.FocusPoints.Count > 0)
            {
                ZoomAndPanControl.SnapTo(new Point(ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.FocusPoints[0].X + ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.FocusPoints[0].Width / 2,
                    ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.FocusPoints[0].Y + ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.FocusPoints[0].Height / 2));
            }
        }

        protected void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            Point curContentMousePoint = e.GetPosition(content);

            if (e.Delta > 0)
            {
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                // don't allow zoomout les that original image 
                if (ZoomAndPanControl.ContentScale - 0.1 > ZoomAndPanControl.FitScale())
                {
                    ZoomAndPanControl.ZoomOut(curContentMousePoint);
                }
                else
                {
                    ZoomAndPanControl.ScaleToFit();
                }
                ZoomOut(curContentMousePoint);
            }
            if (ZoomAndPanControl.ContentScale > ZoomAndPanControl.FitScale())
            {
                LoadFullRes();
            }

        }



        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            ZoomAndPanControl.ZoomAboutPoint(ZoomAndPanControl.ContentScale - 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            ZoomAndPanControl.ZoomAboutPoint(ZoomAndPanControl.ContentScale + 0.1, contentZoomCenter);
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaElement.Play();
        }

        public void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MediaElement.Pause();
        }

        public void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MediaElement.Stop();
        }
    }
}