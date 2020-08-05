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
using System.Runtime.ExceptionServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using ImageMagick;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

#endregion

namespace CameraControl.Core.Classes
{
    public class BitmapLoader : BaseFieldClass
    {
        public delegate void MetaDataUpdatedEventHandler(object sender, FileItem item);

        public event MetaDataUpdatedEventHandler MetaDataUpdated;

        private const int MaxThumbSize = 1920 * 2;
        public const int LargeThumbSize = 1600;
        public const int SmallThumbSize = 400;

        private static BitmapLoader _instance;

        public static BitmapLoader Instance
        {
            get { return _instance ?? (_instance = new BitmapLoader()); }
            set { _instance = value; }
        }

        private BitmapImage _defaultThumbnail;

        public BitmapImage DefaultThumbnail
        {
            get
            {
                if (_defaultThumbnail == null)
                {
                    if (!string.IsNullOrEmpty(ServiceProvider.Branding.DefaultThumbImage) &&
                        File.Exists(ServiceProvider.Branding.DefaultThumbImage))
                    {
                        BitmapImage bi = new BitmapImage();
                        // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                        bi.BeginInit();
                        bi.UriSource = new Uri(PhotoUtils.GetFullPath(ServiceProvider.Branding.DefaultThumbImage));
                        bi.EndInit();
                        _defaultThumbnail = bi;
                    }
                    else
                    {
                        _defaultThumbnail = new BitmapImage(new Uri("pack://application:,,,/Images/logo_big.png"));
                    }
                }
                return _defaultThumbnail;
            }
            set { _defaultThumbnail = value; }
        }


        private BitmapImage _noImageThumbnail;

        public BitmapImage NoImageThumbnail
        {
            get
            {
                if (_noImageThumbnail == null)
                {
                    if (!string.IsNullOrEmpty(ServiceProvider.Branding.DefaultMissingThumbImage) &&
                        File.Exists(ServiceProvider.Branding.DefaultMissingThumbImage))
                    {
                        BitmapImage bi = new BitmapImage();
                        // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                        bi.BeginInit();
                        bi.UriSource = new Uri(ServiceProvider.Branding.DefaultMissingThumbImage);
                        bi.EndInit();
                        _noImageThumbnail = bi;
                    }
                    else
                    {
                        _noImageThumbnail = new BitmapImage(new Uri("pack://application:,,,/Images/NoImage.png"));
                    }
                }
                return _noImageThumbnail;
            }
            set { _noImageThumbnail = value; }
        }

        [HandleProcessCorruptedStateExceptions]
        public void GenerateCache(FileItem fileItem)
        {
            bool deleteFile = false;
            if (fileItem == null)
                return;
            if (!File.Exists(fileItem.FileName))
                return;

            if ((File.Exists(fileItem.LargeThumb) && File.Exists(fileItem.SmallThumb)) && File.Exists(fileItem.InfoFile))
                return;

            if (fileItem.Loading)
                return;

            fileItem.Loading = true;

            PhotoUtils.WaitForFile(fileItem.FileName);
            string filename = fileItem.FileName;
            if (fileItem.IsMovie)
            {
                try
                {
                    string ffmpeg_exe = Path.Combine(Settings.ApplicationFolder, "ffmpeg.exe");
                    if (File.Exists(ffmpeg_exe))
                    {
                        string thumb = Path.Combine(Path.GetDirectoryName(fileItem.FileName),
                            Path.GetFileNameWithoutExtension(fileItem.FileName) + ".thumb.jpg");
                        PhotoUtils.RunAndWait(ffmpeg_exe, String.Format("-i \"{0}\" -ss 00:00:01.000 -f image2 -vframes 1 \"{1}\"", fileItem.FileName, thumb));
                        if (File.Exists(thumb))
                        {
                            deleteFile = true;
                            filename = thumb;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Error get video thumb", exception);
                }
            }
            if (fileItem.IsRaw)
            {
                try
                {
                    string dcraw_exe = Path.Combine(Settings.ApplicationFolder, "dcraw.exe");
                    if (File.Exists(dcraw_exe))
                    {
                        string thumb = Path.Combine(Path.GetTempPath(),
                            Path.GetFileNameWithoutExtension(fileItem.FileName) + ".thumb.jpg");
                        PhotoUtils.RunAndWait(dcraw_exe,
                            string.Format(" -e -O \"{0}\" \"{1}\"", thumb, fileItem.FileName));
                        if (File.Exists(thumb))
                        {
                            deleteFile = true;
                            filename = thumb;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Error get dcraw thumb", exception);
                }
            }

            GetMetadata(fileItem);
            try
            {
                using (MagickImage image = new MagickImage(filename))
                {
                    fileItem.FileInfo.SetSize(image.Width, image.Height);

                    double dw = (double)LargeThumbSize / image.Width;
                    image.FilterType = FilterType.Box;
                    image.Thumbnail((int)(image.Width * dw), (int)(image.Height * dw));

                    if (!ServiceProvider.Settings.DisableHardwareAccelerationNew)
                        image.UnsharpMask(1, 1, 0.5, 0.1);

                    PhotoUtils.CreateFolder(fileItem.LargeThumb);
                    image.Write(fileItem.LargeThumb);
                    fileItem.IsLoaded = true;
                    fileItem.Loading = false;

                    dw = (double)SmallThumbSize / image.Width;
                    image.Thumbnail((int)(image.Width * dw), (int)(image.Height * dw));

                    if (!ServiceProvider.Settings.DisableHardwareAccelerationNew)
                        image.UnsharpMask(1, 1, 0.5, 0.1);

                    PhotoUtils.CreateFolder(fileItem.SmallThumb);
                    image.Write(fileItem.SmallThumb);

                    fileItem.Thumbnail = LoadImage(fileItem.SmallThumb);
                }
                fileItem.SaveInfo();
                SetImageInfo(fileItem);
                if (deleteFile)
                    File.Delete(filename);
                OnMetaDataUpdated(fileItem);
            }
            catch (Exception exception)
            {
                Log.Error("Error generating cache " + fileItem.FileName, exception);
            }
            fileItem.Loading = false;
        }



        public static BitmapFrame GetBitmapFrame(BitmapFrame photo, int width, int height, BitmapScalingMode mode)
        {
            TransformedBitmap target = new TransformedBitmap(
                photo,
                new ScaleTransform(
                    width / photo.Width * 96 / photo.DpiX,
                    height / photo.Height * 96 / photo.DpiY,
                    0, 0));
            BitmapFrame thumbnail = BitmapFrame.Create(target);
            BitmapFrame newPhoto = Resize(thumbnail, width, height, mode);

            return newPhoto;
        }

        private static BitmapFrame Rotate(BitmapFrame photo, double angle, BitmapScalingMode mode)
        {
            TransformedBitmap target = new TransformedBitmap(
                photo,
                new RotateTransform(angle, photo.PixelWidth / 2, photo.PixelHeight / 2));
            BitmapFrame thumbnail = BitmapFrame.Create(target);
            BitmapFrame newPhoto = Resize(thumbnail, photo.PixelWidth, photo.PixelHeight, mode);
            return newPhoto;
        }

        private static BitmapFrame Resize(BitmapFrame photo, int width, int height, BitmapScalingMode scalingMode)
        {
            DrawingGroup group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, scalingMode);
            group.Children.Add(new ImageDrawing(photo, new Rect(0, 0, width, height)));
            DrawingVisual targetVisual = new DrawingVisual();
            DrawingContext targetContext = targetVisual.RenderOpen();
            targetContext.DrawDrawing(group);
            RenderTargetBitmap target = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            targetContext.Close();
            target.Render(targetVisual);
            BitmapFrame targetFrame = BitmapFrame.Create(target);
            return targetFrame;
        }


        public static BitmapSource SaveImageSource(FrameworkElement obj, int width, int height)
        {
            // Save current canvas transform
            obj.LayoutTransform = null;
            obj.Width = width;
            obj.Height = height;
            obj.UpdateLayout();
            obj.UpdateLayout();
            // fix margin offset as well
            Thickness margin = obj.Margin;
            obj.Margin = new Thickness(0, 0,
                 margin.Right - margin.Left, margin.Bottom - margin.Top);

            // Get the size of canvas
            Size size = new Size(width, height);

            // force control to Update
            obj.Measure(size);
            obj.Arrange(new Rect(size));
            RenderTargetBitmap bmp = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);

            bmp.Render(obj);

            //// return values as they were before
            //obj.LayoutTransform = transform;
            //obj.Margin = margin;
            obj = null;
            return bmp;
        }


        public static void Save2Jpg(BitmapSource source, string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.QualityLevel = 90;
                encoder.Save(stream);
                stream.Close();
            }
        }

        public void LoadHistogram(FileItem item)
        {
            try
            {
                var fileInfo = item.FileInfo;
                if (fileInfo != null)
                {
                    if (fileInfo.IsLoading)
                        return;
                    fileInfo.IsLoading = true;
                }
                if (fileInfo == null || fileInfo.ExifTags == null || fileInfo.ExifTags.Items.Count == 0)
                {
                    GetMetadata(item);
                    fileInfo = item.FileInfo;
                    fileInfo.IsLoading = true;
                }
                if (!File.Exists(item.SmallThumb))
                {
                    fileInfo.IsLoading = false;
                    return;
                }

                using (MagickImage image = new MagickImage(item.SmallThumb))
                {
                    var Blue = new int[256];
                    var Green = new int[256];
                    var Red = new int[256];
                    var Luminance = new int[256];
                    Dictionary<IMagickColor<byte>, int> h = image.Histogram();
                    foreach (var i in h)
                    {
                        byte R = i.Key.R;
                        byte G = i.Key.G;
                        byte B = i.Key.B;
                        Blue[B] += i.Value;
                        Green[G] += i.Value;
                        Red[R] += i.Value;
                        int lum = (R + R + R + B + G + G + G + G) >> 3;
                        Luminance[lum] += i.Value;
                    }
                    fileInfo.HistogramBlue = Blue;
                    fileInfo.HistogramGreen = Green;
                    fileInfo.HistogramRed = Red;
                    fileInfo.HistogramLuminance = Luminance;
                    fileInfo.IsLoading = false;
                    item.FileInfo = fileInfo;
                }
                item.SaveInfo();
                if (ServiceProvider.Settings.SelectedBitmap.FileItem == item)
                {
                    SetData(ServiceProvider.Settings.SelectedBitmap,
                                 ServiceProvider.Settings.SelectedBitmap.FileItem);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to load histogram", ex);
            }

            //item.FileInfo.HistogramBlue = SmoothHistogram(item.FileInfo.HistogramBlue);
            //item.FileInfo.HistogramGreen = SmoothHistogram(item.FileInfo.HistogramGreen);
            //item.FileInfo.HistogramRed = SmoothHistogram(item.FileInfo.HistogramRed);
            //item.FileInfo.HistogramLuminance = SmoothHistogram(item.FileInfo.HistogramLuminance);
        }

        public static void Highlight(BitmapFile file, bool under, bool over)
        {
            if (!under && !over)
                return;
            if (file == null || file.DisplayImage == null)
                return;
            var bitmap = Highlight(file.DisplayImage.Clone(), under, over);
            bitmap.Freeze();
            file.DisplayImage = bitmap;
        }

        public static unsafe WriteableBitmap Highlight(WriteableBitmap bitmap, bool under, bool over)
        {
            if (!under && !over)
                return bitmap;
            int color1 = ConvertColor(Colors.Blue);
            int color2 = ConvertColor(Colors.Red);
            int treshold = 2;
            using (BitmapContext bitmapContext = bitmap.GetBitmapContext())
            {
                for (var i = 0; i < bitmapContext.Width * bitmapContext.Height; i++)
                {
                    int num1 = bitmapContext.Pixels[i];
                    byte a = (byte)(num1 >> 24);
                    int num2 = (int)a;
                    if (num2 == 0)
                        num2 = 1;
                    int num3 = 65280 / num2;
                    //Color col = Color.FromArgb(a, (byte)((num1 >> 16 & (int)byte.MaxValue) * num3 >> 8),
                    //                           (byte)((num1 >> 8 & (int)byte.MaxValue) * num3 >> 8),
                    //                           (byte)((num1 & (int)byte.MaxValue) * num3 >> 8));
                    byte R = (byte)((num1 >> 16 & byte.MaxValue) * num3 >> 8);
                    byte G = (byte)((num1 >> 8 & byte.MaxValue) * num3 >> 8);
                    byte B = (byte)((num1 & byte.MaxValue) * num3 >> 8);

                    if (under && R < treshold && G < treshold && B < treshold)
                        bitmapContext.Pixels[i] = color1;
                    if (over && R > 255 - treshold && G > 255 - treshold && B > 255 - treshold)
                        bitmapContext.Pixels[i] = color2;
                }
            }
            return bitmap;
        }

        public void SetData(BitmapFile file, FileItem fileItem)
        {
            if (fileItem == null || fileItem.FileInfo == null)
                return;
            fileItem.LoadInfo();
            file.FileName = Path.GetFileNameWithoutExtension(fileItem.FileName);
            file.Comment = "";
            if (fileItem.FileInfo.ExifTags.ContainName("Iptc.Application2.Caption"))
                file.Comment = fileItem.FileInfo.ExifTags["Iptc.Application2.Caption"];

            file.Metadata.Clear();
            foreach (ValuePair item in fileItem.FileInfo.ExifTags.Items)
            {
                file.Metadata.Add(new DictionaryItem() { Name = item.Name, Value = item.Value });
            }
            file.BlueColorHistogramPoints = ConvertToPointCollection(fileItem.FileInfo.HistogramBlue);
            file.RedColorHistogramPoints = ConvertToPointCollection(fileItem.FileInfo.HistogramRed);
            file.GreenColorHistogramPoints = ConvertToPointCollection(fileItem.FileInfo.HistogramGreen);
            file.LuminanceHistogramPoints = ConvertToPointCollection(fileItem.FileInfo.HistogramLuminance);

            // file.InfoLabel = Path.GetFileName(file.FileItem.FileName);
            file.InfoLabel = "";
            //file.InfoLabel += String.Format(" | {0}x{1}", fileItem.FileInfo.Width, fileItem.FileInfo.Height);
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.ExposureTime"))
            {
                file.InfoLabel += "E " + fileItem.FileInfo.ExifTags["Exif.Photo.ExposureTime"];
            }
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.FNumber"))
            {
                file.InfoLabel += " | " + fileItem.FileInfo.ExifTags["Exif.Photo.FNumber"];
            }
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.ISOSpeedRatings"))
            {
                file.InfoLabel += " | ISO " + fileItem.FileInfo.ExifTags["Exif.Photo.ISOSpeedRatings"];
            }
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.FocalLength"))
            {
                file.InfoLabel += " | " + fileItem.FileInfo.ExifTags["Exif.Photo.FocalLength"];
            }
            SetImageInfo(fileItem);
        }

        public void SetImageInfo(FileItem fileItem)
        {
            if (fileItem == null || fileItem.FileInfo == null)
                return;
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.ExposureTime"))
            {
                fileItem.E = fileItem.FileInfo.ExifTags["Exif.Photo.ExposureTime"];
            }
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.FNumber"))
            {
                fileItem.F = fileItem.FileInfo.ExifTags["Exif.Photo.FNumber"].Replace("F", "");
            }
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.ISOSpeedRatings"))
            {
                fileItem.Iso = fileItem.FileInfo.ExifTags["Exif.Photo.ISOSpeedRatings"];
            }
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.ExposureBiasValue"))
                fileItem.ExposureBias = fileItem.FileInfo.ExifTags["Exif.Photo.ExposureBiasValue"];
            if (fileItem.FileInfo.ExifTags.ContainName("Exif.Photo.FocalLength"))
            {
                fileItem.FocalLength = fileItem.FileInfo.ExifTags["Exif.Photo.FocalLength"];
            }
        }

        public BitmapSource LoadImage(string filename)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(filename);
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        public BitmapSource LoadImage(string filename, int width, int rotateAngle)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            if (width > 0)
                bi.DecodePixelWidth = width;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(filename);
            bi.EndInit();

            var bitmap = BitmapFactory.ConvertToPbgra32Format(bi);
            if (rotateAngle != 0)
                bitmap = bitmap.Rotate(rotateAngle);
            bitmap.Freeze();
            return bitmap;
        }

        public WriteableBitmap LoadImage(FileItem fileItem, bool fullres)
        {
            return LoadImage(fileItem, fullres, ServiceProvider.Settings.ShowFocusPoints);
        }

        [HandleProcessCorruptedStateExceptions]
        public WriteableBitmap LoadImage(FileItem fileItem, bool fullres, bool showfocuspoints)
        {
            if (fileItem == null)
                return null;
            if (!File.Exists(fileItem.LargeThumb) && !fullres)
                return null;
            if (File.Exists(fileItem.InfoFile))
                fileItem.LoadInfo();
            if (fileItem.FileInfo == null)
                fileItem.FileInfo = new FileInfo();

            try
            {
                BitmapDecoder bmpDec = null;
                if (fullres && fileItem.IsRaw)
                {
                    try
                    {
                        string dcraw_exe = Path.Combine(Settings.ApplicationFolder, "dcraw.exe");
                        if (File.Exists(dcraw_exe))
                        {
                            string thumb = Path.Combine(Path.GetTempPath(),
                                Path.GetFileNameWithoutExtension(fileItem.FileName) + ".thumb.jpg");
                            PhotoUtils.RunAndWait(dcraw_exe,
                                string.Format(" -e -O \"{0}\" \"{1}\"", thumb, fileItem.FileName));
                            if (File.Exists(thumb))
                            {
                                bmpDec = BitmapDecoder.Create(new Uri(thumb), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                                File.Delete(thumb);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Error get dcraw thumb", exception);
                    }
                }

                PhotoUtils.WaitForFile(fullres ? fileItem.FileName : fileItem.LargeThumb);
                if (bmpDec == null)
                    bmpDec = BitmapDecoder.Create(new Uri(fullres ? fileItem.FileName : fileItem.LargeThumb),
                        BitmapCreateOptions.None,
                        BitmapCacheOption.OnLoad);
                // if no future processing required
                if (!showfocuspoints && ServiceProvider.Settings.RotateIndex == 0 && !ServiceProvider.Settings.FlipPreview)
                {
                    fileItem.Loading = false;
                    var b = new WriteableBitmap(bmpDec.Frames[0]);
                    b.Freeze();
                    return b;
                }

                var bitmap = BitmapFactory.ConvertToPbgra32Format(bmpDec.Frames[0]);

                if (showfocuspoints && !fileItem.Transformed)
                    DrawFocusPoints(fileItem, bitmap);


                if (ServiceProvider.Settings.RotateIndex != 0)
                {
                    switch (ServiceProvider.Settings.RotateIndex)
                    {
                        case 1:
                            bitmap = bitmap.Rotate(90);
                            break;
                        case 2:
                            bitmap = bitmap.Rotate(180);
                            break;
                        case 3:
                            bitmap = bitmap.Rotate(270);
                            break;
                    }
                }

                if (ServiceProvider.Settings.FlipPreview)
                {
                    bitmap = bitmap.Flip(WriteableBitmapExtensions.FlipMode.Vertical);
                }

                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception exception)
            {
                Log.Error("Error loading image", exception);
                if (exception.GetType() == typeof(OutOfMemoryException) && fullres)
                {
                    return LoadImage(fileItem, false);
                }
            }
            return null;
        }

        /// <summary>
        /// Will load the small thumbnail attached to file item
        /// If the cach not yet generated will retun null
        /// </summary>
        /// <param name="fileItem"></param>
        /// <param name="width">The desired with, if 0 the full size will be loaded</param>
        /// <returns></returns>
        public WriteableBitmap LoadSmallImage(FileItem fileItem, int width = 0)
        {
            if (!File.Exists(fileItem.SmallThumb))
                return null;
            PhotoUtils.WaitForFile(fileItem.SmallThumb);
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (width > 0)
                    bi.DecodePixelWidth = width;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(fileItem.SmallThumb);
                bi.EndInit();
                WriteableBitmap bitmap = BitmapFactory.ConvertToPbgra32Format(bi);
                //bitmap.Freeze();
                return bitmap;
            }
            catch (Exception exception)
            {
                Log.Error("Error loading image", exception);
            }
            return null;
        }

        public void GetMetadata(FileItem fileItem)
        {
            try
            {
                PhotoUtils.WaitForFile(fileItem.FileName);
                Exiv2Helper exiv2Helper = new Exiv2Helper();
                exiv2Helper.Load(fileItem);

                if (fileItem.FileInfo.ExifTags.ContainName("Exif.Image.Orientation"))
                {
                    if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "bottom, right")
                        fileItem.AutoRotation = 2;

                    //if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "top, left")
                    //    writeableBitmap = writeableBitmap.Rotate(180);

                    if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "right, top")
                        fileItem.AutoRotation = 1;

                    if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "left, bottom")
                        fileItem.AutoRotation = 3;
                }

            }
            catch (Exception exception)
            {
                Log.Error("Error loading metadata ", exception);
            }
        }

        public void DrawFocusPoints(FileItem fileItem, WriteableBitmap bitmap)
        {
            if (fileItem.FileInfo == null || fileItem.FileInfo.Width == 0 || fileItem.FileInfo.Height == 0)
                return;
            bitmap.Lock();
            double dw = (double)bitmap.PixelWidth / fileItem.FileInfo.Width;
            double dh = (double)bitmap.PixelHeight / fileItem.FileInfo.Height;

            foreach (Rect focuspoint in fileItem.FileInfo.FocusPoints)
            {
                DrawRect(bitmap, (int)(focuspoint.X * dw), (int)(focuspoint.Y * dh),
                    (int)((focuspoint.X + focuspoint.Width) * dw),
                    (int)((focuspoint.Y + focuspoint.Height) * dh), Colors.Aqua, (bitmap.PixelWidth / 1000) + 2);
            }
            bitmap.Unlock();
        }

        private void DrawRect(WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, int line)
        {
            DrawFocusRect(bmp, x1, y1, x2, y2, color, line);
        }

        private void DrawFocusRect(WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, int tick)
        {
            int width = (x2 - x1) / 4;
            int height = (y2 - y1) / 4;
            DrawLineEx(bmp, x1 - (tick / 2), y1, width, 0, color, tick);
            DrawLineEx(bmp, x1, y1, 0, height, color, tick);

            DrawLineEx(bmp, x1, y2, width, 0, color, tick);
            DrawLineEx(bmp, x1 + (tick / 2), y2, 0, -height, color, tick);


            DrawLineEx(bmp, x2, y1, -width, 0, color, tick);
            DrawLineEx(bmp, x2 - (tick / 2), y1, 0, height, color, tick);

            DrawLineEx(bmp, x2 + (tick / 2), y2, -width, 0, color, tick);
            DrawLineEx(bmp, x2, y2, 0, -height, color, tick);


        }

        public void DrawLineEx(WriteableBitmap bmp, int x, int y, int width, int height, Color color, int tick)
        {
            bmp.DrawLineAa(x, y, x + width, y + height, color, tick);
        }

        public static PointCollection ConvertToPointCollection(int[] values)
        {
            PointCollection points = new PointCollection();
            if (values == null)
            {
                points.Freeze();
                return points;
            }

            values = SmoothHistogram(values);

            int max = values.Max();


            // first point (lower-left corner)
            points.Add(new Point(0, max));
            // middle points
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new Point(i, max - values[i]));
            }
            // last point (lower-right corner)
            points.Add(new Point(values.Length - 1, max));
            points.Freeze();
            return points;
        }

        private static int[] SmoothHistogram(int[] originalValues)
        {
            int[] smoothedValues = new int[originalValues.Length];

            double[] mask = new double[] { 0.25, 0.5, 0.25 };

            for (int bin = 1; bin < originalValues.Length - 1; bin++)
            {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++)
                {
                    smoothedValue += originalValues[bin - 1 + i] * mask[i];
                }
                smoothedValues[bin] = (int)smoothedValue;
            }

            return smoothedValues;
        }

        private static int ConvertColor(Color color)
        {
            int num = (int)color.A + 1;
            return (int)color.A << 24 | (int)(byte)((int)color.R * num >> 8) << 16 |
                   (int)(byte)((int)color.G * num >> 8) << 8 | (int)(byte)((int)color.B * num >> 8);
        }

        protected virtual void OnMetaDataUpdated(FileItem item)
        {
            var handler = MetaDataUpdated;
            if (handler != null) handler(this, item);
        }

        static bool HasJpegHeader(Stream stream)
        {
            try
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    UInt16 soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                    UInt16 jfif = br.ReadUInt16(); // JFIF marker (FFE0)
                    return soi == 0xd8ff && (jfif == 0xe0ff || jfif == 0xe1ff || jfif == 0xeeff);
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public static bool IsCompleteJPG(Stream stream)
        {
            try
            {
                if (HasJpegHeader(stream))
                {
                    int maxBytestoTest = 20480; // on sony slt-a37, end of file marker is at 18788 bytes (18KB from end of file, usting 20K to be safe)
                    long numBytes = stream.Length;
                    long offset = 2;
                    byte[] last_bytes = new byte[2];
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        do
                        {
                            reader.BaseStream.Seek(-1 * offset, SeekOrigin.End);
                            reader.Read(last_bytes, 0, 2);

                            if (last_bytes[0] == 0xFF && last_bytes[1] == 0xD9)
                                return true;
                            else
                                offset += 1;
                        } while (offset < maxBytestoTest);
                    }
                }
            }
            catch (Exception)
            {
                //log.Error(e);
                return false;
            }
            return false;

        }

    }
}