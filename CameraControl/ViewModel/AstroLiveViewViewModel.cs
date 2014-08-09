using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        private Point _centralPoint;
        private int _zoomFactor;

        public AstroLiveViewViewModel(ICameraDevice device)
            :base(device)
        {
            ZoomFactor = 1;
        }

        public AstroLiveViewViewModel()
            :base()
        {
            
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

        public override void GetLiveImage()
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
                preview.Freeze();
                Preview = preview;


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

        private void DrawGrid(WriteableBitmap bitmap)
        {
            if (CentralPoint.X == 0 && CentralPoint.Y == 0)
            {
                CentralPoint = new System.Windows.Point(bitmap.PixelWidth / 2, bitmap.PixelHeight / 2);
            }
            WriteableBitmap tempbitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX,
                                                             bitmap.DpiY, PixelFormats.Pbgra32, bitmap.Palette);

            tempbitmap.DrawLine(0, (int)CentralPoint.Y - 5, bitmap.PixelWidth, (int)CentralPoint.Y - 5, Colors.White);
            tempbitmap.DrawLine(0, (int)CentralPoint.Y + 5, bitmap.PixelWidth, (int)CentralPoint.Y + 5, Colors.White);

            tempbitmap.DrawLine((int)CentralPoint.X - 5, 0, (int)CentralPoint.X - 5, bitmap.PixelHeight, Colors.White);
            tempbitmap.DrawLine((int)CentralPoint.X + 5, 0, (int)CentralPoint.X + 5, bitmap.PixelHeight, Colors.White);

            bitmap.Blit(new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), tempbitmap,
                        new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

        }

    }
}
