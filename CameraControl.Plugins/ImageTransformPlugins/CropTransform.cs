using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class CropTransform : IImageTransformPlugin
    {
        public string Name
        {
            get { return "Crop"; }
        }

        public string Execute(FileItem item,string infile, string dest, ValuePairEnumerator configData)
        {
            var conf = new CropTransformViewModel(configData);
            using (var fileStream = new MemoryStream(File.ReadAllBytes(infile)))
            {
                BitmapDecoder bmpDec = BitmapDecoder.Create(fileStream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);
                WriteableBitmap writeableBitmap = BitmapFactory.ConvertToPbgra32Format(bmpDec.Frames[0]);
                if (conf.FromLiveView && ServiceProvider.DeviceManager.SelectedCameraDevice != null)
                {
                    var prop = ServiceProvider.DeviceManager.SelectedCameraDevice.LoadProperties();
                    conf.Left = (int) (writeableBitmap.PixelWidth*prop.LiveviewSettings.HorizontalMin/100);
                    conf.Width =
                        (int)
                            (writeableBitmap.PixelWidth*
                             (prop.LiveviewSettings.HorizontalMax - prop.LiveviewSettings.HorizontalMin)/100);
                    conf.Top = (int) (writeableBitmap.Height*prop.LiveviewSettings.VerticalMin/100);
                    conf.Height =
                        (int)
                            (writeableBitmap.Height*
                             (prop.LiveviewSettings.VerticalMax - prop.LiveviewSettings.VerticalMin)/100);
                }

                BitmapLoader.Save2Jpg(writeableBitmap.Crop(conf.Left,conf.Top,conf.Width,conf.Height), dest);
            }
            return dest;
        }

        public UserControl GetConfig(ValuePairEnumerator configData)
        {
            var control = new CropTransformView {DataContext = new CropTransformViewModel(configData)};
            return control;
        }
    }
}
