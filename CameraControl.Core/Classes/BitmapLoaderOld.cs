using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Imaging;
using CameraControl.Devices;
using Color = System.Windows.Media.Color;

namespace CameraControl.Core.Classes
{
    public class BitmapLoaderOld
    {
        private bool _isworking = false;
        private BitmapFile _nextfile;
        private BitmapFile _currentfile;


        private static BitmapLoaderOld _instance;
        public static BitmapLoaderOld Instance
        {
            get { return _instance ?? (_instance = new BitmapLoaderOld()); }
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
                        _defaultThumbnail = new BitmapImage(new Uri("pack://application:,,,/Images/logo.png"));
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

        public void GetBitmap(BitmapFile bitmapFile)
        {
            if (_isworking)
            {
                _nextfile = bitmapFile;
                return;
            }
            _nextfile = null;
            _isworking = true;
            _currentfile = bitmapFile;
            _currentfile.RawCodecNeeded = false;
            if (!File.Exists(_currentfile.FileItem.FileName))
            {
                Log.Error("File not found " + _currentfile.FileItem.FileName);
                StaticHelper.Instance.SystemMessage = "File not found " + _currentfile.FileItem.FileName;
            }
            else
            {
                //Metadata.Clear();
                try
                {
                    if (_currentfile.FileItem.IsRaw)
                    {
                        WriteableBitmap writeableBitmap = null;
                        Log.Debug("Loading raw file.");
                        BitmapDecoder bmpDec = BitmapDecoder.Create(new Uri(_currentfile.FileItem.FileName),
                                                                    BitmapCreateOptions.None,
                                                                    BitmapCacheOption.Default);
                        if (bmpDec.CodecInfo != null)
                            Log.Debug("Raw codec: " + bmpDec.CodecInfo.FriendlyName);
                        if (bmpDec.Thumbnail != null)
                        {
                            WriteableBitmap bitmap = new WriteableBitmap(bmpDec.Thumbnail);
                            bitmap.Freeze();
                            bitmapFile.DisplayImage = bitmap;
                        }

                        if (ServiceProvider.Settings.LowMemoryUsage)
                        {
                            if (bmpDec.Thumbnail != null)
                            {
                                writeableBitmap = BitmapFactory.ConvertToPbgra32Format(bmpDec.Thumbnail);
                            }
                            else
                            {
                                writeableBitmap = BitmapFactory.ConvertToPbgra32Format(bmpDec.Frames[0]);
                                double dw = 2000 / writeableBitmap.Width;
                                writeableBitmap = writeableBitmap.Resize((int)(writeableBitmap.PixelWidth * dw),
                                                                         (int)(writeableBitmap.PixelHeight * dw),
                                                                         WriteableBitmapExtensions.Interpolation.Bilinear);
                            }
                            bmpDec = null;
                        }
                        else
                        {
                            writeableBitmap = BitmapFactory.ConvertToPbgra32Format(bmpDec.Frames.Single());
                        }
                        GetMetadata(_currentfile, writeableBitmap);
                        Log.Debug("Loading raw file done.");
                    }
                    else
                    {
                        BitmapImage bi = new BitmapImage();
                        // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                        bi.BeginInit();

                        if (ServiceProvider.Settings.LowMemoryUsage)
                            bi.DecodePixelWidth = 2000;

                        bi.UriSource = new Uri(_currentfile.FileItem.FileName);
                        bi.EndInit();
                        WriteableBitmap writeableBitmap = BitmapFactory.ConvertToPbgra32Format(bi);
                        GetMetadata(_currentfile, writeableBitmap);
                        Log.Debug("Loading bitmap file done.");
                    }
                }
                catch (FileFormatException)
                {
                    _currentfile.RawCodecNeeded = true;
                    Log.Debug("Raw codec not installed or unknown file format");
                    StaticHelper.Instance.SystemMessage = "Raw codec not installed or unknown file format";
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
                if (_nextfile == null)
                {
                    Thread threadPhoto = new Thread(GetAdditionalData);
                    threadPhoto.Start(_currentfile);
                    _currentfile.OnBitmapLoaded();
                    _currentfile = null;
                    _isworking = false;

                }
                else
                {
                    _isworking = false;
                    GetBitmap(_nextfile);
                }
            }
        }

        private void GenerateCache(BitmapFile file, WriteableBitmap writeableBitmap)
        {
            //WriteableBitmap bitmap = writeableBitmap.Clone();
            //SaveThumb(file.FileItem.SmallThumb, 255, bitmap);
            //SaveThumb(file.FileItem.LargeThumb, 1920, bitmap);
        }

        private void SaveThumb(string filename, int width, WriteableBitmap bitmap)
        {
            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //bi.CacheOption = BitmapCacheOption.OnLoad;
            //bi.DecodePixelWidth = width;
            //bi.StreamSource = new MemoryStream(bitmap.ToByteArray());
            //bi.EndInit();
            Save2Jpg(bitmap, filename);
        }

        public static void Save2Jpg(BitmapSource source, string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            if(dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (FileStream stream5 = new FileStream(filename, FileMode.Create))
            {
                JpegBitmapEncoder encoder5 = new JpegBitmapEncoder();
                encoder5.Frames.Add(BitmapFrame.Create(source));
                encoder5.Save(stream5);
                stream5.Close();
            }
        }

        private void GetAdditionalData(object o)
        {
            BitmapFile file = o as BitmapFile;
            try
            {
                if (!file.FileItem.IsRaw)
                {
                    using (Bitmap bmp = new Bitmap(file.FileItem.FileName))
                    {
                        // Luminance
                        ImageStatisticsHSL hslStatistics = new ImageStatisticsHSL(bmp);
                        file.LuminanceHistogramPoints = ConvertToPointCollection(hslStatistics.Luminance.Values);
                        // RGB
                        ImageStatistics rgbStatistics = new ImageStatistics(bmp);
                        file.RedColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Red.Values);
                        file.GreenColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Green.Values);
                        file.BlueColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Blue.Values);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        void DrawRect(WriteableBitmap bmp, int x1, int y1, int x2, int y2, Color color, int line)
        {
            for (int i = 0; i < line; i++)
            {
                bmp.DrawRectangle(x1 - i, y1 - i, x2 - i, y2 - i, color);
            }
        }

        public void GetMetadata(BitmapFile file, WriteableBitmap writeableBitmap)
        {
            Exiv2Helper exiv2Helper = new Exiv2Helper();
            try
            {
                exiv2Helper.Load(file.FileItem.FileName, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight);
                file.Metadata.Clear();
                foreach (var exiv2Data in exiv2Helper.Tags)
                {
                    file.Metadata.Add(new DictionaryItem() { Name = exiv2Data.Value.Tag, Value = exiv2Data.Value.Value });
                }
                if (ServiceProvider.Settings.ShowFocusPoints)
                {
                    writeableBitmap.Lock();
                    foreach (Rect focuspoint in exiv2Helper.Focuspoints)
                    {
                        DrawRect(writeableBitmap, (int)focuspoint.X, (int)focuspoint.Y, (int)(focuspoint.X + focuspoint.Width),
                                 (int)(focuspoint.Y + focuspoint.Height), Colors.Aqua,
                                 ServiceProvider.Settings.LowMemoryUsage ? 2 : 7);
                    }
                    writeableBitmap.Unlock();
                }

                if (exiv2Helper.Tags.ContainsKey("Exif.Image.Orientation") && !file.FileItem.IsRaw)
                {
                    if (exiv2Helper.Tags["Exif.Image.Orientation"].Value == "bottom, right")
                        writeableBitmap = writeableBitmap.Rotate(180);

                    if (exiv2Helper.Tags["Exif.Image.Orientation"].Value == "right, top")
                        writeableBitmap = writeableBitmap.Rotate(90);

                    if (exiv2Helper.Tags["Exif.Image.Orientation"].Value == "left, bottom")
                        writeableBitmap = writeableBitmap.Rotate(270);
                }

                if (ServiceProvider.Settings.RotateIndex != 0)
                {
                    switch (ServiceProvider.Settings.RotateIndex)
                    {
                        case 1:
                            writeableBitmap = writeableBitmap.Rotate(90);
                            break;
                        case 2:
                            writeableBitmap = writeableBitmap.Rotate(180);
                            break;
                        case 3:
                            writeableBitmap = writeableBitmap.Rotate(270);
                            break;
                    }
                }

            }
            catch (Exception exception)
            {
                Log.Error("Error loading metadata ", exception);
            }

            writeableBitmap.Freeze();
            file.DisplayImage = writeableBitmap;
            file.InfoLabel = Path.GetFileName(file.FileItem.FileName);
            file.InfoLabel += String.Format(" | {0}x{1}", exiv2Helper.Width, exiv2Helper.Height);
            if (exiv2Helper.Tags.ContainsKey("Exif.Photo.ExposureTime"))
                file.InfoLabel += " | E " + exiv2Helper.Tags["Exif.Photo.ExposureTime"].Value;
            if (exiv2Helper.Tags.ContainsKey("Exif.Photo.FNumber"))
                file.InfoLabel += " | " + exiv2Helper.Tags["Exif.Photo.FNumber"].Value;
            if (exiv2Helper.Tags.ContainsKey("Exif.Photo.ISOSpeedRatings"))
                file.InfoLabel += " | ISO " + exiv2Helper.Tags["Exif.Photo.ISOSpeedRatings"].Value;
            if (exiv2Helper.Tags.ContainsKey("Exif.Photo.ExposureBiasValue"))
                file.InfoLabel += " | " + exiv2Helper.Tags["Exif.Photo.ExposureBiasValue"].Value;
            if (exiv2Helper.Tags.ContainsKey("Exif.Photo.FocalLength"))
                file.InfoLabel += " | " + exiv2Helper.Tags["Exif.Photo.FocalLength"].Value;

        }


        private PointCollection ConvertToPointCollection(int[] values)
        {
            values = SmoothHistogram(values);

            int max = values.Max();

            PointCollection points = new PointCollection();
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


    }
}
