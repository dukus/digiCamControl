using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CLROBS;

namespace DccObsPlugin
{
    public class DccObsImageSource : AbstractImageSource
    {
        private Object textureLock = new Object();
        private Texture texture = null;
        private XElement config;
        private string _serverUrl = "http://localhost:5513/";

        public DccObsImageSource(XElement config)
        {
            ExecuteCmd("LiveViewWnd_Show");
            Thread.Sleep(1000);
            ExecuteCmd("All_Minimize");
            this.config = config;
            UpdateSettings();
        }

        private void ExecuteCmd(string cmd)
        {
            try
            {
                var c = new WebClient();
                var bytes = c.DownloadData(_serverUrl + "?CMD=" + cmd);
            }
            catch (Exception)
            {
                
                
            }
            
        }

        public override void Render(float x, float y, float width, float height)
        {
            lock (textureLock)
            {
                try
                {
                    if (texture != null)
                    {
                        texture.Dispose();
                        texture = null;
                    }

                    //if (File.Exists(imageFile))
                    //{
                    var c = new WebClient();
                    var bytes = c.DownloadData(_serverUrl + "liveview.jpg");
                    var ms = new MemoryStream(bytes);

                    BitmapImage src = new BitmapImage();

                    src.BeginInit();
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.StreamSource = ms;
                    src.EndInit();

                    WriteableBitmap wb = new WriteableBitmap(new FormatConvertedBitmap(src, PixelFormats.Bgra32, null, 0));

                    texture = GS.CreateTexture((UInt32)wb.PixelWidth, (UInt32)wb.PixelHeight, GSColorFormat.GS_BGRA, null, false, false);

                    texture.SetImage(wb.BackBuffer, GSImageFormat.GS_IMAGEFORMAT_BGRA, (UInt32)(wb.PixelWidth * 4));

                    config.Parent.SetInt("cx", wb.PixelWidth);
                    config.Parent.SetInt("cy", wb.PixelHeight);

                    Size.X = (float)wb.PixelWidth;
                    Size.Y = (float)wb.PixelHeight;

                    GS.DrawSprite(texture, 0xFFFFFFFF, x, y, x + width, y + height);

                }
                catch (Exception exception)
                {
                    this.Api.Log(exception.Message);
                }

                //}
                //else
                //{
                //    texture = null;
                //}
            }
        }

        public override void UpdateSettings()
        {
            this.Api.Log("UpdateSettings");
        }
    }
}
