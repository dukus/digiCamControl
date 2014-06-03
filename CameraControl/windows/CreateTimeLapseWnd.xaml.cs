using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
//using System.Windows.Shapes;
using AForge.Imaging.Filters;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using FreeImageAPI;
using Image = System.Drawing.Image;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for CreateTimeLapseWnd.xaml
    /// </summary>
    public partial class CreateTimeLapseWnd
    {
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private string _tempFolder = "";
        private int _counter = 0;
        private string _basedir = "";
        private string _virtualdubdir = "";
        private DateTime _starTime;

        public CreateTimeLapseWnd()
        {
            InitializeComponent();
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _tempFolder = Path.Combine(Path.GetTempPath(), "DCC_TimeLapse", ServiceProvider.Settings.DefaultSession.Name);
            try
            {
                if (Directory.Exists(_tempFolder))
                {
                    Directory.Delete(_tempFolder, true);
                }
                Directory.CreateDirectory(_tempFolder);
            }
            catch (Exception exception)
            {
                Log.Error("Unable to reinitialize timelapse folder", exception);
            }
            _basedir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            _virtualdubdir = Path.Combine(_basedir, "VirtualDub", "VirtualDub.exe");
            btn_paly.Visibility = Visibility.Hidden;
            ServiceProvider.Settings.ApplyTheme(this);
        }

        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Content = "Done";
            btn_paly.Visibility = Visibility.Visible;
            this.BringIntoView();
        }

        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<FileItem> _files = ServiceProvider.Settings.DefaultSession.Files.Where(fileItem => fileItem.IsChecked).ToList();
            if (_files.Count == 0)
                _files = ServiceProvider.Settings.DefaultSession.Files.ToList();
            _starTime = DateTime.Now;
            for (int i = 0; i < _files.Count; i++)
            {

                //}
                //foreach (FileItem fileItem in ServiceProvider.Settings.DefaultSession.Files)
                //{
                _counter++;
                FileItem item = _files[i];
                if (ServiceProvider.Settings.DefaultSession.TimeLapse.VirtualMove)
                {
                    ResizeImageMove(item.FileName, i, _files.Count);
                }
                else
                {
                    ResizeImage(item.FileName);
                }

                Dispatcher.BeginInvoke(new ThreadStart(delegate
                                                         {
                                                             progressBar1.Value++;
                                                             image1.Source = item.Thumbnail;
                                                             label1.Content = string.Format("{0}/{1} ({2})", _counter,
                                                                                            _files.Count,
                                                                                            (DateTime.Now - _starTime).ToString(@"hh\:mm\:ss"));
                                                         }));
                if (_backgroundWorker.CancellationPending)
                {
                    return;
                }
            }
            string script = "";
            using (TextReader reader = new StreamReader(Path.Combine(_basedir, "VirtualDub.script")))
            {
                script = reader.ReadToEnd();
            }
            script = script.Replace("$InFile$", Path.Combine(_tempFolder, "img000001.jpg").Replace(@"\", @"\\"));
            script = script.Replace("$FPS$", ServiceProvider.Settings.DefaultSession.TimeLapse.Fps.ToString());
            script = script.Replace("$Count$", _files.Count.ToString());
            script = script.Replace("$OutFile$", ServiceProvider.Settings.DefaultSession.TimeLapse.OutputFIleName.Replace(@"\", @"\\"));
            using (TextWriter writer = new StreamWriter(Path.Combine(_tempFolder, "VirtualDub.script")))
            {
                writer.Write(script);
            }

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(_virtualdubdir);
            //psi.RedirectStandardOutput = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            psi.Arguments = "/min /s \"" + Path.Combine(_tempFolder, "VirtualDub.script") + "\" /x";
            psi.UseShellExecute = false;
            System.Diagnostics.Process listFiles;
            listFiles = System.Diagnostics.Process.Start(psi);
            listFiles.WaitForExit();
            //MessageBox.Show("Conversion Done !");
            //Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ServiceProvider.Settings.DefaultSession.Files.Count == 0)
            {
                MessageBox.Show("No image files in current session !");
                Close();
                return;
            }
            if (ServiceProvider.Settings.DefaultSession.TimeLapse.VideoType == null || ServiceProvider.Settings.DefaultSession.TimeLapse.VideoType.Width == 0 || ServiceProvider.Settings.DefaultSession.TimeLapse.VideoType.Height == 0)
            {
                MessageBox.Show("Wrong video settings !");
                Close();
                return;
            }
            if (string.IsNullOrEmpty(ServiceProvider.Settings.DefaultSession.TimeLapse.OutputFIleName))
            {
                MessageBox.Show("No output file defined !");
                Close();
                return;
            }
            if (!File.Exists(_virtualdubdir))
            {
                MessageBox.Show("VirtualDub not found. Reinstall the application ! !");
                Close();
                return;
            }
            progressBar1.Maximum = ServiceProvider.Settings.DefaultSession.Files.Count;
            _backgroundWorker.RunWorkerAsync();
        }

        private void ResizeImageMove(string file, int index, int total)
        {
            string newfile = Path.Combine(_tempFolder, "img" + _counter.ToString("000000") + ".jpg");

            Bitmap image = (Bitmap)Image.FromFile(file);
            // format image
            //AForge.Imaging.Image.FormatImage(ref image);
            //FIBITMAP dib = FreeImage.LoadEx(file);


            int dw = (int)image.Width; //FreeImage.GetWidth(dib);
            int dh = (int)image.Height; //FreeImage.GetHeight(dib);
            int tw = ServiceProvider.Settings.DefaultSession.TimeLapse.VideoType.Width;
            int th = ServiceProvider.Settings.DefaultSession.TimeLapse.VideoType.Height;
            double movieaspectration = (double)th / tw;
            double mouviesize = ((double)(ServiceProvider.Settings.DefaultSession.TimeLapse.MovePercent) / 100);
            int frame_h = dh >= dw ? dh : (int)(dw * movieaspectration);
            int frame_w = dw >= dh ? dw : (int)(dh * movieaspectration);
            int frame_h_dif = (int)(dh * mouviesize);
            int frame_w_dif = (int)(dw * mouviesize);
            frame_h = frame_h - frame_h_dif;
            frame_w = frame_w - frame_w_dif;
            double zw = (tw / (double)dw);
            double zh = (th / (double)dh);
            double z = ((zw <= zh) ? zw : zh); //!ServiceProvider.Settings.DefaultSession.TimeLapse.FillImage ? ((zw <= zh) ? zw : zh) : ((zw >= zh) ? zw : zh);

            int crop_x = frame_h_dif / total * index;
            int crop_y = 0;

            switch (ServiceProvider.Settings.DefaultSession.TimeLapse.MoveDirection)
            {
                // left to right
                case 0:
                    {
                        crop_x = (int)((dw - frame_w) / (double)total * index);
                        switch (ServiceProvider.Settings.DefaultSession.TimeLapse.MoveAlignment)
                        {
                            case 0:
                                crop_y = 0;
                                break;
                            case 1:
                                crop_y = (dh - frame_h) / 2;
                                break;
                            case 2:
                                crop_y = (dh - frame_h);
                                break;
                        }
                    }
                    break;
                // right to left
                case 1:
                    {
                        crop_x = (int)((dw - frame_w) / (double)total * (total - index));
                        switch (ServiceProvider.Settings.DefaultSession.TimeLapse.MoveAlignment)
                        {
                            case 0:
                                crop_y = 0;
                                break;
                            case 1:
                                crop_y = (dh - frame_h) / 2;
                                break;
                            case 2:
                                crop_y = (dh - frame_h);
                                break;
                        }
                    }
                    break;
                // top to bottom
                case 2:
                    {
                        crop_y = (int)((dh - frame_h) / (double)total * index);
                        switch (ServiceProvider.Settings.DefaultSession.TimeLapse.MoveAlignment)
                        {
                            case 0:
                                crop_x = 0;
                                break;
                            case 1:
                                crop_x = (dw - frame_w) / 2;
                                break;
                            case 2:
                                crop_x = (dw - frame_w);
                                break;
                        }
                    }
                    break;
                // bottom to top
                case 3:
                    {
                        crop_y = (int)((dh - frame_h) / (double)total * (total - index));
                        switch (ServiceProvider.Settings.DefaultSession.TimeLapse.MoveAlignment)
                        {
                            case 0:
                                crop_x = 0;
                                break;
                            case 1:
                                crop_x = (dw - frame_w) / 2;
                                break;
                            case 2:
                                crop_x = (dw - frame_w);
                                break;
                        }
                    }
                    break;
                case 4:
                    crop_x = (int)((dw - frame_w) / (double)total * index);
                    crop_y = (int)((dh - frame_h) / (double)total * index);
                    break;
                case 5:
                    crop_x = (int)((dw - frame_w) / (double)total * (total - index));
                    crop_y = (int)((dh - frame_h) / (double)total * (total - index));
                    break;
            }

            Crop crop = new Crop(new Rectangle(crop_x, crop_y, frame_w, frame_h));
            Bitmap newimage = crop.Apply(image);
            ResizeBilinear resizeBilinearFilter = new ResizeBilinear(tw, th);
            newimage = resizeBilinearFilter.Apply(newimage);
            newimage.Save(newfile, ImageFormat.Jpeg);
            image.Dispose();
            newimage.Dispose();
        }

        private void ResizeImage(string file)
        {
            string newfile = Path.Combine(_tempFolder, "img" + _counter.ToString("000000") + ".jpg");
            FIBITMAP dib = FreeImage.LoadEx(file);

            uint dw = FreeImage.GetWidth(dib);
            uint dh = FreeImage.GetHeight(dib);
            int tw = ServiceProvider.Settings.DefaultSession.TimeLapse.VideoType.Width;
            int th = ServiceProvider.Settings.DefaultSession.TimeLapse.VideoType.Height;
            double zw = (tw / (double)dw);
            double zh = (th / (double)dh);
            double z = 0;
            z = !ServiceProvider.Settings.DefaultSession.TimeLapse.FillImage ? ((zw <= zh) ? zw : zh) : ((zw >= zh) ? zw : zh);
            dw = (uint)(dw * z);
            dh = (uint)(dh * z);
            int difw = (int)(tw - dw);
            int difh = (int)(th - dh);
            if (FreeImage.GetFileType(file, 0) == FREE_IMAGE_FORMAT.FIF_RAW)
            {
                FIBITMAP bmp = FreeImage.ToneMapping(dib, FREE_IMAGE_TMO.FITMO_REINHARD05, 0, 0); // ConvertToType(dib, FREE_IMAGE_TYPE.FIT_BITMAP, false);
                FIBITMAP resized = FreeImage.Rescale(bmp, (int)dw, (int)dh, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                FIBITMAP final = FreeImage.EnlargeCanvas<RGBQUAD>(resized, difw / 2, difh / 2, difw - (difw / 2), difh - (difh / 2),
                                                                  new RGBQUAD(System.Drawing.Color.Black),
                                                                  FREE_IMAGE_COLOR_OPTIONS.FICO_RGB);
                FreeImage.SaveEx(final, newfile);
                FreeImage.UnloadEx(ref final);
                FreeImage.UnloadEx(ref resized);
                FreeImage.UnloadEx(ref dib);
                FreeImage.UnloadEx(ref bmp);
            }
            else
            {
                FIBITMAP resized = FreeImage.Rescale(dib, (int)dw, (int)dh, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                FIBITMAP final = FreeImage.EnlargeCanvas<RGBQUAD>(resized, difw / 2, difh / 2, difw - (difw / 2), difh - (difh / 2),
                                                                  new RGBQUAD(System.Drawing.Color.Black),
                                                                  FREE_IMAGE_COLOR_OPTIONS.FICO_RGB);
                FreeImage.SaveEx(final, newfile);
                FreeImage.UnloadEx(ref final);
                FreeImage.UnloadEx(ref resized);
                FreeImage.UnloadEx(ref dib);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_backgroundWorker.IsBusy)
            {
                if (MessageBox.Show("A task is running !\n Do you want to cancel it ?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (_backgroundWorker.IsBusy)
                        _backgroundWorker.CancelAsync();
                }
            }
        }

        private void btn_paly_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(ServiceProvider.Settings.DefaultSession.TimeLapse.OutputFIleName))
                System.Diagnostics.Process.Start(ServiceProvider.Settings.DefaultSession.TimeLapse.OutputFIleName);
            else
                MessageBox.Show("Output file not found");
        }

    }
}
