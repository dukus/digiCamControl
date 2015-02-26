using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Imaging.Filters;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.windows;
using Point = System.Windows.Point;


namespace CameraControl.ViewModel
{
    public class AstroLiveViewViewModel:LiveViewViewModel
    {
        private object _locker = new object();
        private Point _centralPoint;
        private int _zoomFactor;
        private BitmapSource _starWindow;

        private int Threshold = 400;
        private double StarSizeOld = 0;
        private double K = 10;
        private double _starSize;
        private int _starWindowSize;

        public AstroLiveViewViewModel(ICameraDevice device)
            :base(device)
        {
            ZoomFactor = 1;
            StarWindowSize = 30;
            StarSize = 123.11;
        }

        public AstroLiveViewViewModel()
            :base()
        {
            
        }

        public int StarWindowSize
        {
            get { return _starWindowSize; }
            set
            {
                _starWindowSize = value;
                RaisePropertyChanged(() => StarWindowSize);
            }
        }

        public double DisplayStarSize
        {
            get { return Math.Round(StarSize,2); }
        }


        public double StarSize
        {
            get { return _starSize; }
            set
            {
                _starSize = value;
                RaisePropertyChanged(() => StarSize);
                RaisePropertyChanged(() => DisplayStarSize);
            }
        }


        public Point CentralPoint
        {
            get { return _centralPoint; }
            set
            {
                _centralPoint = value;
                RaisePropertyChanged(() => CentralPoint);
            }
        }

        public int ZoomFactor
        {
            get { return _zoomFactor; }
            set
            {
                _zoomFactor = value;
                RaisePropertyChanged(() => ZoomFactor);
            }
        }

        public BitmapSource StarWindow
        {
            get { return _starWindow; }
            set
            {
                _starWindow = value;
                RaisePropertyChanged(() => StarWindow);
            }
        }

        public bool IsFullImage
        {
            get { return CameraDevice.LiveViewImageZoomRatio.Value == "All"; }
        }


        public void CalculateStarSize(WriteableBitmap bitmap)
        {
            int Max = 0;
            int Min = 800;
            int Count = 0;
            for (int i = 0; i < StarWindowSize-1; i++)
            {

                for (int j = 0; j < StarWindowSize-1; j++)
                {
                    var c = bitmap.GetPixel(i, j);
                    var greyVal = c.R + c.G + c.B;
                    if (greyVal > Threshold)
                        Count = Count + 1;
                    if (greyVal > Max)
                        Max = greyVal;

                    if (greyVal < Min)
                        Min = greyVal;
                }
            }
            Threshold = (Max + Min)/2;
            StarSize = ((double) Count + K*StarSizeOld)/(K + 1);
            StarSizeOld = StarSize;

        }


        public override void GetLiveImage()
        {
            lock (_locker)
            {
                try
                {
                    LiveViewData = LiveViewManager.GetLiveViewImage(CameraDevice);
                }
                catch (Exception)
                {
                    return;
                }

                if (LiveViewData == null || LiveViewData.ImageData == null)
                    return;
                MemoryStream stream = new MemoryStream(LiveViewData.ImageData,
                    LiveViewData.ImageDataPosition,
                    LiveViewData.ImageData.Length -
                    LiveViewData.ImageDataPosition);
                using (var bmp = new Bitmap(stream))
                {
                    Bitmap res = bmp;
                    var preview = BitmapFactory.ConvertToPbgra32Format(BitmapSourceConvert.ToBitmapSource(res));
                    var zoow = preview.Crop((int) (CentralPoint.X - (StarWindowSize/2)),
                        (int) (CentralPoint.Y - (StarWindowSize/2)),
                        StarWindowSize, StarWindowSize);
                    CalculateStarSize(zoow);
                    zoow.Freeze();
                    StarWindow = zoow;
                    if (CameraDevice.LiveViewImageZoomRatio.Value == "All")
                    {
                        preview.Freeze();
                        Preview = preview;
                    }


                    if (Brightness != 0)
                    {
                        BrightnessCorrection filter = new BrightnessCorrection(Brightness);
                        res = filter.Apply(res);
                    }
                    if (EdgeDetection)
                    {
                        var filter = new FiltersSequence(
                            Grayscale.CommonAlgorithms.BT709,
                            new HomogenityEdgeDetector()
                            );
                        res = filter.Apply(res);
                    }

                    var _bitmap = BitmapFactory.ConvertToPbgra32Format(BitmapSourceConvert.ToBitmapSource(res));
                    DrawGrid(_bitmap);

                    if (ZoomFactor > 1)
                    {
                        double d = _bitmap.PixelWidth/(double) ZoomFactor;
                        double h = _bitmap.PixelHeight/(double) ZoomFactor;
                        _bitmap = _bitmap.Crop((int) (CentralPoint.X - (d/2)), (int) (CentralPoint.Y - (h/2)),
                            (int) d, (int) h);
                    }

                    _bitmap.Freeze();
                    Bitmap = _bitmap;
                }

            }
        }

        public override void SetFocusPos(int x, int y)
        {
            lock (_locker)
            {
                base.SetFocusPos(x, y);                
            }
        }

        private void DrawGrid(WriteableBitmap bitmap)
        {
            if (CentralPoint.X == 0 && CentralPoint.Y == 0)
            {
                CentralPoint = new Point(bitmap.PixelWidth / 2, bitmap.PixelHeight / 2);
            }
            WriteableBitmap tempbitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX,
                                                             bitmap.DpiY, PixelFormats.Pbgra32, bitmap.Palette);

            tempbitmap.DrawLine(0, (int)CentralPoint.Y - (StarWindowSize / 2), bitmap.PixelWidth, (int)CentralPoint.Y - (StarWindowSize / 2), Colors.White);
            tempbitmap.DrawLine(0, (int)CentralPoint.Y + (StarWindowSize / 2), bitmap.PixelWidth, (int)CentralPoint.Y + (StarWindowSize / 2), Colors.White);

            tempbitmap.DrawLine((int)CentralPoint.X - (StarWindowSize / 2), 0, (int)CentralPoint.X - (StarWindowSize / 2), bitmap.PixelHeight, Colors.White);
            tempbitmap.DrawLine((int)CentralPoint.X + (StarWindowSize / 2), 0, (int)CentralPoint.X + (StarWindowSize / 2), bitmap.PixelHeight, Colors.White);

            bitmap.Blit(new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), tempbitmap,
                        new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

        }

    }
}
