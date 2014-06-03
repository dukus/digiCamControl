using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using CameraControl.Devices;
using FreeImageAPI;

namespace CameraControl.Core.Classes
{
    public class PhotoUtils
    {
        public static bool RunAndWait(string exe, string param)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exe);
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                startInfo.Arguments = param;
                Process process = Process.Start(startInfo);
                process.WaitForExit();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool Run(string exe)
        {
            return Run(exe, "", ProcessWindowStyle.Minimized);
        }

        public static bool Run(string exe, string param)
        {
            return Run(exe, param, ProcessWindowStyle.Minimized);
        }

        public static bool Run(string exe, string param, ProcessWindowStyle processWindowStyle)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exe);
                startInfo.WindowStyle = processWindowStyle;
                startInfo.Arguments = param;
                Process process = Process.Start(startInfo);
                //process.WaitForExit();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void CopyPhotoScale(string sourse, string dest, double scale)
        {
            if (scale == 1 && Path.GetExtension(sourse).ToUpper() == Path.GetExtension(dest).ToUpper())
            {
                File.Copy(sourse, dest, true);
                return;
            }

            string newfile = dest;
            FIBITMAP dib = FreeImage.LoadEx(sourse);

            uint dw = FreeImage.GetWidth(dib);
            uint dh = FreeImage.GetHeight(dib);
            int tw = (int)(dw * scale);
            int th = (int)(dh * scale);
            double zw = (tw / (double)dw);
            double zh = (th / (double)dh);
            double z = 0;
            z = ((zw <= zh) ? zw : zh);
            dw = (uint)(dw * z);
            dh = (uint)(dh * z);
            int difw = (int)(tw - dw);
            int difh = (int)(th - dh);
            if (FreeImage.GetFileType(sourse, 0) == FREE_IMAGE_FORMAT.FIF_RAW)
            {
                FIBITMAP bmp = FreeImage.ToneMapping(dib, FREE_IMAGE_TMO.FITMO_REINHARD05, 0, 0);
                // ConvertToType(dib, FREE_IMAGE_TYPE.FIT_BITMAP, false);
                FIBITMAP resized = FreeImage.Rescale(bmp, (int)dw, (int)dh, FREE_IMAGE_FILTER.FILTER_CATMULLROM);
                FIBITMAP final = FreeImage.EnlargeCanvas<RGBQUAD>(resized, difw / 2, difh / 2, difw - (difw / 2), difh - (difh / 2),
                                                                  new RGBQUAD(Color.Black),
                                                                  FREE_IMAGE_COLOR_OPTIONS.FICO_RGB);
                FreeImage.SaveEx(final, newfile);
                FreeImage.UnloadEx(ref final);
                FreeImage.UnloadEx(ref resized);
                FreeImage.UnloadEx(ref dib);
                FreeImage.UnloadEx(ref bmp);
            }
            else
            {
                {
                    FIBITMAP resized = FreeImage.Rescale(dib, (int)dw, (int)dh, FREE_IMAGE_FILTER.FILTER_CATMULLROM);
                    FIBITMAP final = FreeImage.EnlargeCanvas<RGBQUAD>(resized, difw / 2, difh / 2, difw - (difw / 2), difh - (difh / 2),
                                                                      new RGBQUAD(Color.Black),
                                                                      FREE_IMAGE_COLOR_OPTIONS.FICO_RGB);
                    FreeImage.SaveEx(final, newfile);
                    FreeImage.UnloadEx(ref final);
                    FreeImage.UnloadEx(ref resized);
                    FreeImage.UnloadEx(ref dib);
                }
            }
        }


        public static void PlayCaptureSound()
        {
            try
            {
                string basedir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                if (basedir != null)
                {
                    var mplayer = new SoundPlayer(Path.Combine(basedir, "Data", "takephoto.wav"));
                    mplayer.Play();
                }
            }
            catch (Exception exception)
            {
                Log.Debug(exception);
            }
        }

        public static string DateTimeToString(DateTime time)
        {
            TimeSpan span = time - DateTime.MinValue;
            return string.Format("{0} days {1} hours {2} minutes {3} seconds", span.Days, span.Hours, span.Minutes,
                                 span.Seconds);
        }

        public static void Donate()
        {
            Run("http://www.digicamcontrol.com/donate/");
        }
    }
}
