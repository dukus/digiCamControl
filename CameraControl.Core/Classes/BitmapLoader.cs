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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Classes
{
    public class BitmapLoader : BaseFieldClass
    {
        private const int MaxThumbSize = 1920*2;
        private const int LargeThumbSize = 1600;
        private const int SmallThumbSize = 255;

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
                        bi.UriSource = new Uri(ServiceProvider.Branding.DefaultThumbImage);
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


        public void GenerateCache(FileItem fileItem)
        {
            bool deleteFile = false;
            if (fileItem == null)
                return;
            if (!File.Exists(fileItem.FileName))
                return;
            
            if ((File.Exists(fileItem.LargeThumb) && File.Exists(fileItem.SmallThumb)) && File.Exists(fileItem.InfoFile))
                return;

            string filename = fileItem.FileName;
            if (fileItem.IsRaw)
            {

                try
                {
                    string dcraw_exe = Path.Combine(Settings.ApplicationFolder, "dcraw.exe");
                    if (File.Exists(dcraw_exe))
                    {
                        PhotoUtils.RunAndWait(dcraw_exe, string.Format(" -e \"{0}\"", fileItem.FileName));
                        string thumb = Path.Combine(Path.GetDirectoryName(fileItem.FileName),
                            Path.GetFileNameWithoutExtension(fileItem.FileName) + ".thumb.jpg");
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
                using (MemoryStream fileStream = new MemoryStream(File.ReadAllBytes(filename)))
                {
                    BitmapDecoder bmpDec = BitmapDecoder.Create(fileStream,
                                                                BitmapCreateOptions.PreservePixelFormat,
                                                                BitmapCacheOption.OnLoad);

                    bmpDec.DownloadProgress += (o, args) => StaticHelper.Instance.LoadingProgress = args.Progress;

                    fileItem.FileInfo.Width = bmpDec.Frames[0].PixelWidth;
                    fileItem.FileInfo.Height = bmpDec.Frames[0].PixelHeight;

                    double dw = (double) LargeThumbSize/bmpDec.Frames[0].PixelWidth;
                    WriteableBitmap writeableBitmap =
                        BitmapFactory.ConvertToPbgra32Format(GetBitmapFrame(bmpDec.Frames[0],
                                                                            (int) (bmpDec.Frames[0].PixelWidth*dw),
                                                                            (int) (bmpDec.Frames[0].PixelHeight*dw),
                                                                            BitmapScalingMode.Linear));

                    LoadHistogram(fileItem, writeableBitmap);
                    Save2Jpg(writeableBitmap, fileItem.LargeThumb);

                    dw = (double) SmallThumbSize/writeableBitmap.PixelWidth;
                    writeableBitmap = writeableBitmap.Resize((int) (writeableBitmap.PixelWidth*dw),
                                                             (int) (writeableBitmap.PixelHeight*dw),
                                                             WriteableBitmapExtensions.Interpolation.Bilinear);

                    if (fileItem.FileInfo.ExifTags.ContainName("Exif.Image.Orientation") )
                    {
                        if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "bottom, right")
                            writeableBitmap = writeableBitmap.Rotate(180);

                        //if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "top, left")
                        //    writeableBitmap = writeableBitmap.Rotate(180);

                        if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "right, top")
                            writeableBitmap = writeableBitmap.Rotate(90);

                        if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "left, bottom")
                            writeableBitmap = writeableBitmap.Rotate(270);
                    }

                    Save2Jpg(writeableBitmap, fileItem.SmallThumb);
                    fileItem.Thumbnail = LoadSmallImage(fileItem);
                    fileItem.IsLoaded = true;
                    fileItem.SaveInfo();
                    if (deleteFile)
                        File.Delete(filename);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error generating cache " + fileItem.FileName, exception);
            }
        }


        public static BitmapFrame GetBitmapFrame(BitmapFrame photo, int width, int height, BitmapScalingMode mode)
        {
            TransformedBitmap target = new TransformedBitmap(
                photo,
                new ScaleTransform(
                    width/photo.Width*96/photo.DpiX,
                    height/photo.Height*96/photo.DpiY,
                    0, 0));
            BitmapFrame thumbnail = BitmapFrame.Create(target);
            BitmapFrame newPhoto = Resize(thumbnail, width, height, mode);

            return newPhoto;
        }

        private static BitmapFrame Rotate(BitmapFrame photo, double angle, BitmapScalingMode mode)
        {
            TransformedBitmap target = new TransformedBitmap(
                photo,
                new RotateTransform(angle, photo.PixelWidth/2, photo.PixelHeight/2));
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

        private unsafe void LoadHistogram(FileItem fileItem, WriteableBitmap bitmap)
        {
            fileItem.FileInfo.HistogramBlue = new int[256];
            fileItem.FileInfo.HistogramGreen = new int[256];
            fileItem.FileInfo.HistogramRed = new int[256];
            fileItem.FileInfo.HistogramLuminance = new int[256];
            using (BitmapContext bitmapContext = bitmap.GetBitmapContext())
            {
                for (var i = 0; i < bitmapContext.Width*bitmapContext.Height; i++)
                {
                    int num1 = bitmapContext.Pixels[i];
                    byte a = (byte) (num1 >> 24);
                    int num2 = (int) a;
                    if (num2 == 0)
                        num2 = 1;
                    int num3 = 65280/num2;
                    byte R = (byte) ((num1 >> 16 & (int) byte.MaxValue)*num3 >> 8);
                    byte G = (byte) ((num1 >> 8 & (int) byte.MaxValue)*num3 >> 8);
                    byte B = (byte) ((num1 & (int) byte.MaxValue)*num3 >> 8);

                    fileItem.FileInfo.HistogramBlue[B]++;
                    fileItem.FileInfo.HistogramGreen[G]++;
                    fileItem.FileInfo.HistogramRed[R]++;
                    int lum = (R + R + R + B + G + G + G + G) >> 3;
                    fileItem.FileInfo.HistogramLuminance[lum]++;
                }
            }
            fileItem.FileInfo.HistogramBlue = SmoothHistogram(fileItem.FileInfo.HistogramBlue);
            fileItem.FileInfo.HistogramGreen = SmoothHistogram(fileItem.FileInfo.HistogramGreen);
            fileItem.FileInfo.HistogramRed = SmoothHistogram(fileItem.FileInfo.HistogramRed);
            fileItem.FileInfo.HistogramLuminance = SmoothHistogram(fileItem.FileInfo.HistogramLuminance);
        }

        public unsafe void Highlight(BitmapFile file, bool under, bool over)
        {
            if (!under && !over)
                return;
            if (file == null || file.DisplayImage == null)
                return;
            WriteableBitmap bitmap = file.DisplayImage.Clone();
            int color1 = ConvertColor(Colors.Blue);
            int color2 = ConvertColor(Colors.Red);
            int treshold = 2;
            using (BitmapContext bitmapContext = bitmap.GetBitmapContext())
            {
                for (var i = 0; i < bitmapContext.Width*bitmapContext.Height; i++)
                {
                    int num1 = bitmapContext.Pixels[i];
                    byte a = (byte) (num1 >> 24);
                    int num2 = (int) a;
                    if (num2 == 0)
                        num2 = 1;
                    int num3 = 65280/num2;
                    //Color col = Color.FromArgb(a, (byte)((num1 >> 16 & (int)byte.MaxValue) * num3 >> 8),
                    //                           (byte)((num1 >> 8 & (int)byte.MaxValue) * num3 >> 8),
                    //                           (byte)((num1 & (int)byte.MaxValue) * num3 >> 8));
                    byte R = (byte) ((num1 >> 16 & byte.MaxValue)*num3 >> 8);
                    byte G = (byte) ((num1 >> 8 & byte.MaxValue)*num3 >> 8);
                    byte B = (byte) ((num1 & byte.MaxValue)*num3 >> 8);

                    if (under && R < treshold && G < treshold && B < treshold)
                        bitmapContext.Pixels[i] = color1;
                    if (over && R > 255 - treshold && G > 255 - treshold && B > 255 - treshold)
                        bitmapContext.Pixels[i] = color2;
                }
            }
            bitmap.Freeze();
            file.DisplayImage = bitmap;
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
                file.Metadata.Add(new DictionaryItem() {Name = item.Name, Value = item.Value});
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

        public ImageSource LoadImage(string filename, int width, int rotateAngle)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
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
        
        public WriteableBitmap LoadImage(FileItem fileItem, bool fullres, bool showfocuspoints)
        {
            if (fileItem == null)
                return null;
            if (!File.Exists(fileItem.LargeThumb) && !fullres)
                return null;

            if (File.Exists(fileItem.InfoFile))
                fileItem.LoadInfo();
            else
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
                            PhotoUtils.RunAndWait(dcraw_exe, string.Format(" -e \"{0}\"", fileItem.FileName));
                            string thumb = Path.Combine(Path.GetDirectoryName(fileItem.FileName),
                                Path.GetFileNameWithoutExtension(fileItem.FileName) + ".thumb.jpg");
                            if (File.Exists(thumb))
                            {
                                bmpDec = BitmapDecoder.Create(new Uri(thumb),BitmapCreateOptions.None,BitmapCacheOption.OnLoad);
                                File.Delete(thumb);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Error get dcraw thumb", exception);
                    }
                }

                if (bmpDec == null)
                    bmpDec = BitmapDecoder.Create(new Uri(fullres ? fileItem.FileName : fileItem.LargeThumb),
                        BitmapCreateOptions.None,
                        BitmapCacheOption.OnLoad);

                double dw = (double) MaxThumbSize/bmpDec.Frames[0].PixelWidth;
                WriteableBitmap bitmap;
                /*
                if (fullres)
                    bitmap =
                        BitmapFactory.ConvertToPbgra32Format(GetBitmapFrame(bmpDec.Frames[0],
                                                                            (int) (bmpDec.Frames[0].PixelWidth*dw),
                                                                            (int) (bmpDec.Frames[0].PixelHeight*dw),
                                                                            BitmapScalingMode.NearestNeighbor));
                else
                    bitmap = BitmapFactory.ConvertToPbgra32Format(bmpDec.Frames[0]);
                */
                bitmap = BitmapFactory.ConvertToPbgra32Format(bmpDec.Frames[0]);

                if (showfocuspoints)
                    DrawFocusPoints(fileItem, bitmap);

                if (fileItem.FileInfo !=null && fileItem.FileInfo.ExifTags.ContainName("Exif.Image.Orientation") )
                {
                    if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "bottom, right")
                        bitmap = bitmap.Rotate(180);

                    //if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "top, left")
                    //    bitmap = bitmap.Rotate(180);

                    if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "right, top")
                        bitmap = bitmap.Rotate(90);

                    if (fileItem.FileInfo.ExifTags["Exif.Image.Orientation"] == "left, bottom")
                        bitmap = bitmap.Rotate(270);
                }

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
            }
            return null;
        }

        public WriteableBitmap LoadSmallImage(FileItem fileItem)
        {
            if (!File.Exists(fileItem.SmallThumb))
                return null;
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(fileItem.SmallThumb);
                bi.EndInit();
                WriteableBitmap bitmap = BitmapFactory.ConvertToPbgra32Format(bi);
                bitmap.Freeze();
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
            Exiv2Helper exiv2Helper = new Exiv2Helper();
            try
            {
                exiv2Helper.Load(fileItem);
            }
            catch (Exception exception)
            {
                Log.Error("Error loading metadata ", exception);
            }
        }

        private void DrawFocusPoints(FileItem fileItem, WriteableBitmap bitmap)
        {
            if (fileItem.FileInfo == null || fileItem.FileInfo.Width == 0 || fileItem.FileInfo.Height == 0)
                return;
            bitmap.Lock();
            double dw = (double) bitmap.PixelWidth/fileItem.FileInfo.Width;
            double dh = (double) bitmap.PixelHeight/fileItem.FileInfo.Height;

            foreach (Rect focuspoint in fileItem.FileInfo.FocusPoints)
            {
                DrawRect(bitmap, (int) (focuspoint.X*dw), (int) (focuspoint.Y*dh),
                    (int) ((focuspoint.X + focuspoint.Width)*dw),
                    (int) ((focuspoint.Y + focuspoint.Height)*dh), Colors.Aqua, (bitmap.PixelWidth/1000) + 1);
            }
            bitmap.Unlock();
        }

        private void DrawRect(WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, int line)
        {
            for (int i = 0; i < line; i++)
            {
                //bmp.DrawRectangle(x1 + i, y1 + i, x2 - i, y2 - i, color);
                DrawFocusRect(bmp, x1 + i, y1 + i, x2 - i, y2 - i, color);
            }
        }

        private void DrawFocusRect(WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color)
        {
            int width = (x2 - x1)/4;
            int height = (y2 - y1)/4;

            DrawLineEx(bmp, x1, y1, width, 0, color);
            DrawLineEx(bmp, x1, y1, 0, height, color);

            DrawLineEx(bmp, x1, y2, width, 0, color);
            DrawLineEx(bmp, x1, y2, 0, -height, color);


            DrawLineEx(bmp, x2, y1, -width, 0, color);
            DrawLineEx(bmp, x2, y1, 0, height, color);

            DrawLineEx(bmp, x2, y2, -width, 0, color);
            DrawLineEx(bmp, x2, y2, 0, -height, color);


        }

        public void DrawLineEx(WriteableBitmap bmp, int x, int y, int width, int height, Color color)
        {
            bmp.DrawLineAa(x, y, x + width, y + height, color);
        }

        private PointCollection ConvertToPointCollection(int[] values)
        {
            PointCollection points = new PointCollection();
            if (values == null)
            {
                points.Freeze();
                return points;
            }

            //values = SmoothHistogram(values);

            int max = values.Max();


            // first point (lower-left corner)
            points.Add(new System.Windows.Point(0, max));
            // middle points
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new System.Windows.Point(i, max - values[i]));
            }
            // last point (lower-right corner)
            points.Add(new System.Windows.Point(values.Length - 1, max));
            points.Freeze();
            return points;
        }

        private int[] SmoothHistogram(int[] originalValues)
        {
            int[] smoothedValues = new int[originalValues.Length];

            double[] mask = new double[] {0.25, 0.5, 0.25};

            for (int bin = 1; bin < originalValues.Length - 1; bin++)
            {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++)
                {
                    smoothedValue += originalValues[bin - 1 + i]*mask[i];
                }
                smoothedValues[bin] = (int) smoothedValue;
            }

            return smoothedValues;
        }

        private static int ConvertColor(Color color)
        {
            int num = (int) color.A + 1;
            return (int) color.A << 24 | (int) (byte) ((int) color.R*num >> 8) << 16 |
                   (int) (byte) ((int) color.G*num >> 8) << 8 | (int) (byte) ((int) color.B*num >> 8);
        }
    }
}