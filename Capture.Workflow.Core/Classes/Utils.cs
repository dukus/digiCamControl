using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Devices;

namespace Capture.Workflow.Core.Classes
{
    public class Utils
    {

        public static BitmapSource LoadImage(Stream stream, int width=0, int rotateAngle=0)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            if (width > 0)
                bi.DecodePixelWidth = width;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = stream;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        public static BitmapSource LoadImage(string filename, int width=0, int rotateAngle=0)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            if (width > 0)
                bi.DecodePixelWidth = width;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(filename);
            bi.EndInit();
            bi.Freeze();
            return bi;
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
                Save2Jpg(source, stream);
                stream.Close();
            }
        }

        public static void Save2Jpg(BitmapSource source, Stream stream)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.QualityLevel = 90;
            encoder.Save(stream);
        }


        public static unsafe WriteableBitmap Highlight(WriteableBitmap bitmap, bool under, bool over)
        {
            if (!under && !over)
            {
                bitmap.Freeze();
                return bitmap;
            }
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
                    byte R = (byte)((num1 >> 16 & Byte.MaxValue) * num3 >> 8);
                    byte G = (byte)((num1 >> 8 & Byte.MaxValue) * num3 >> 8);
                    byte B = (byte)((num1 & Byte.MaxValue) * num3 >> 8);

                    if (under && R < treshold && G < treshold && B < treshold)
                        bitmapContext.Pixels[i] = color1;
                    if (over && R > 255 - treshold && G > 255 - treshold && B > 255 - treshold)
                        bitmapContext.Pixels[i] = color2;
                }
            }
            bitmap.Freeze();
            return bitmap;
        }

        private static int ConvertColor(Color color)
        {
            int num = (int)color.A + 1;
            return (int)color.A << 24 | (int)(byte)((int)color.R * num >> 8) << 16 |
                   (int)(byte)((int)color.G * num >> 8) << 8 | (int)(byte)((int)color.B * num >> 8);
        }

        public static int[] SmoothHistogram(int[] originalValues)
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


        /// <summary>
        /// Delete the file
        /// Error safe will not throw any exception  
        /// </summary>
        /// <param name="file">Full path of the file name</param>
        public static void DeleteFile(string file)
        {
            try
            {
                if (!File.Exists(file))
                    return;
                WaitForFile(file);
                File.Delete(file);
            }
            catch (Exception e)
            {
                Log.Error("Unable to delete file "+file,e);
            }
        }

        public static void WaitForFile(string file)
        {
            if (!File.Exists(file))
                return;
            int retry = 15;
            while (IsFileLocked(file) && retry > 0)
            {
                Thread.Sleep(100);
                retry--;
            }
        }

        public static bool IsFileLocked(string file)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void CopyFiles(string source, string dest)
        {
            if (!Directory.Exists(source))
                return;

            var files = Directory.GetFiles(source);
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);

            foreach (string file in files)
            {
                var newFile = Path.Combine(dest, Path.GetFileName(file));
                if (!File.Exists(newFile))
                    File.Copy(file, newFile);
            }
        }

        /// <summary>
        /// Creates the folder for the specified filename
        /// </summary>
        /// <param name="filename">The filename.</param>
        public static void CreateFolder(string filename)
        {
            var folder = Path.GetDirectoryName(filename);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public static string GetNextFileName(string file)
        {
            if (!File.Exists(file))
                return file;

            var ext = Path.GetExtension(file);
            var fileName = Path.GetFileNameWithoutExtension(file);
            var folder = Path.GetDirectoryName(file);
            int counter = 0;
            while (true)
            {
                var newfile = Path.Combine(folder, fileName + counter.ToString("000000") + ext);
                if (!File.Exists(newfile))
                    return newfile;
                counter++;
            }
        }


        public static Process Run(string exe, string param = "", ProcessWindowStyle processWindowStyle = ProcessWindowStyle.Normal)
        {
            Process process = null;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exe);
                startInfo.WindowStyle = processWindowStyle;
                if (!string.IsNullOrEmpty(param))
                    startInfo.Arguments = param;
                process = Process.Start(startInfo);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return null;
            }
            return process;
        }

        public static bool UrlExists(string file)
        {
            bool exists = false;
            HttpWebResponse response = null;
            var request = (HttpWebRequest)WebRequest.Create(file);
            request.Method = "HEAD";
            request.Timeout = 5000; // milliseconds
            request.AllowAutoRedirect = false;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                exists = response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                exists = false;
            }
            finally
            {
                // close your response.
                response?.Close();
            }
            return exists;
        }

        public static void SendEmail(string body, string subject, string from, string to, string file = null)
        {
            using (SmtpClient mailClient = new SmtpClient("smtp.sendgrid.net", 587))
            {
                // Set the network credentials.
                mailClient.Credentials = new NetworkCredential(CameraControl.Private.Ids.SendgridUser, CameraControl.Private.Ids.SendgridPass);

                //Enable SSL.
                //mailClient.EnableSsl = true;

                var message = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body ?? "",
                    IsBodyHtml = false
                };
                if (File.Exists(file))
                    message.Attachments.Add(new Attachment(file));

                mailClient.Send(message);
                message.Dispose();
            }
        }

    }
}
