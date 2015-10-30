using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Plugins.ImageTransformPlugins;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ImageMagick;
using Microsoft.Win32;

namespace CameraControl.Plugins.ToolPlugins
{
    public class GenMovieViewModel : ViewModelBase
    {
        private bool _canceling;
        private AutoExportPluginConfig _config = new AutoExportPluginConfig() {Name = "Test"};
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private BitmapSource _bitmap;
        private int _totalImages;
        private int _currentImages;
        private Window _window;
        private int _fps;
        private int _minValue;
        private int _maxValue;
        private AsyncObservableCollection<string> _outPut;
        private ObservableCollection<VideoType> _videoTypes;
        private VideoType _videoType;
        private string _outPutFile;
        private bool _fillImage;
        private object _locker = new object();
        private bool _transformBefor;
        private int _progress;
        private int _progressMax;

        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChanged(() => Bitmap);
            }
        }

        public int TotalImages
        {
            get { return _totalImages; }
            set
            {
                _totalImages = value;
                RaisePropertyChanged(() => TotalImages);
            }
        }

        public int CurrentImages
        {
            get { return _currentImages; }
            set
            {
                _currentImages = value;
                RaisePropertyChanged(() => CurrentImages);
                RaisePropertyChanged(() => CounterText);
            }
        }

        public ObservableCollection<VideoType> VideoTypes
        {
            get { return _videoTypes; }
            set
            {
                _videoTypes = value;
                RaisePropertyChanged(() => VideoTypes);
            }
        }

        public bool IsBusy
        {
            get { return _backgroundWorker.IsBusy; }
        }

        public bool IsFree
        {
            get { return !_backgroundWorker.IsBusy; }
        }

        public string CounterText
        {
            get { return string.Format("{0}/{1}", CurrentImages, TotalImages); }
        }

        public int Fps
        {
            get { return _fps; }
            set
            {
                _fps = value;
                RaisePropertyChanged(() => Fps);
            }
        }

        public int MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                if (CurrentImages < MinValue)
                    CurrentImages = MinValue;
                RaisePropertyChanged(() => MinValue);
            }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                if (CurrentImages > MaxValue)
                    CurrentImages = MaxValue;
                RaisePropertyChanged(() => MaxValue);
            }
        }

        public AsyncObservableCollection<string> OutPut
        {
            get { return _outPut; }
            set
            {
                _outPut = value;
                RaisePropertyChanged(() => OutPut);
            }
        }

        public VideoType VideoType
        {
            get { return _videoType; }
            set
            {
                _videoType = value;
                RaisePropertyChanged(() => VideoType);
            }
        }

        public string OutPutFile
        {
            get { return _outPutFile; }
            set
            {
                _outPutFile = value;
                RaisePropertyChanged(() => OutPutFile);
            }
        }

        public bool FillImage
        {
            get { return _fillImage; }
            set
            {
                _fillImage = value;
                RaisePropertyChanged(() => FillImage);
            }
        }

        public bool TransformBefor
        {
            get { return _transformBefor; }
            set
            {
                _transformBefor = value;
                RaisePropertyChanged(() => TransformBefor);
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged(() => Progress);
            }
        }

        public int ProgressMax
        {
            get { return _progressMax; }
            set
            {
                _progressMax = value;
                RaisePropertyChanged(()=>ProgressMax);
            }
        }

        public RelayCommand StartCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand BrowseFileCommand { get; set; }
        public RelayCommand PlayVideoCommand { get; set; }
        public RelayCommand ConfPluginCommand { get; set; }
        
        public GenMovieViewModel()
        {

        }

        public GenMovieViewModel(Window window)
        {
            _window = window;
            _window.Closed += _window_Closed;
            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
            PlayVideoCommand = new RelayCommand(PlayVideo);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            ConfPluginCommand = new RelayCommand(ConfPlugin);
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.WorkerSupportsCancellation = true;
            TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count;
            MaxValue = TotalImages;
            VideoTypes = new ObservableCollection<VideoType>
            {
                new VideoType("4K 16:9", 3840, 2160),
                new VideoType("HD 1080 16:9", 1920, 1080),
                new VideoType("UXGA 4:3", 1600, 1200),
                new VideoType("HD 720 16:9", 1280, 720),
                new VideoType("Super VGA 4:3", 800, 600),
            };
            VideoType = VideoTypes[0];
            OutPutFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                ServiceProvider.Settings.DefaultSession.Name + ".mp4");
            OutPut = new AsyncObservableCollection<string>();
            Fps = 15;
        }

        private void ConfPlugin()
        {
            TransformPluginEditView wnd =new TransformPluginEditView();
            wnd.DataContext = new TransformPluginEditViewModel(_config);
            wnd.Owner = _window;
            wnd.ShowDialog();
        }

        private void _window_Closed(object sender, EventArgs e)
        {
            Stop();
        }

        private void BrowseFile()
        {

            var dialog = new SaveFileDialog();
            dialog.Filter = "Mp4 files (*.mp4)|*.mp4|All files (*.*)|*.*";
            dialog.AddExtension = true;
            dialog.FileName = OutPutFile;
            if (dialog.ShowDialog() == true)
            {
                OutPutFile = dialog.FileName;
            }
        }

        private void PlayVideo()
        {
            if (!File.Exists(OutPutFile))
            {
                MessageBox.Show("Video file not found !");
                return;
            }
            PhotoUtils.Run(OutPutFile);
        }

        private void Start()
        {
            try
            {
                _canceling = false;
                if (File.Exists(OutPutFile))
                {
                    if (
                        MessageBox.Show("Video file already exist. Do you want to continue ?", "Warning ",
                            MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        return;
                    File.Delete(OutPutFile);
                }
                _backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Log.Error("Error delete video", ex);
            }
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RaisePropertyChanged(() => IsBusy);
            RaisePropertyChanged(() => IsFree);
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RaisePropertyChanged(() => IsBusy);
            RaisePropertyChanged(() => IsFree);


            string tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string ffmpegPath = Path.Combine(Settings.ApplicationFolder, "ffmpeg.exe");
            if (!File.Exists(ffmpegPath))
            {
                MessageBox.Show("ffmpeg not found! Please reinstall the application.");
                return;
            }

            try
            {
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);
            }
            catch (Exception exception)
            {
                OutPut.Add(exception.Message);
                return;
            }

            Thread.Sleep(500);
            CurrentImages = 0;
            TotalImages = ServiceProvider.Settings.DefaultSession.Files.Count;
            int counter = 0;
            ProgressMax = MaxValue - MinValue;
            Progress = 0;
            for (int i = MinValue; i < MaxValue; i++)
            {
                if (_backgroundWorker.CancellationPending)
                {
                    DeleteTempFolder(tempFolder);
                    OutPut.Insert(0, "Operation CANCELED !!!");
                    return;
                }

                try
                {
                    Progress++;
                    FileItem item = ServiceProvider.Settings.DefaultSession.Files[i];
                    string outfile = Path.Combine(tempFolder, "img" + counter.ToString("000000") + ".jpg");
                    if (TransformBefor)
                    {
                        AutoExportPluginHelper.ExecuteTransformPlugins(item, _config, item.FileName, outfile);
                        CopyFile(outfile, outfile);
                    }
                    else
                    {
                        CopyFile(item.FileName, outfile);                        
                    }
                    //outfile =
                    if (!TransformBefor)
                        AutoExportPluginHelper.ExecuteTransformPlugins(item, _config, outfile, outfile);
                    OutPut.Insert(0,"Procesing file " + item.Name);
                    counter++;
                }
                catch (Exception exception)
                {
                    OutPut.Add(exception.Message);
                }
            }

            try
            {
                string parameters = @"-r {0} -i {1}\img00%04d.jpg -c:v libx264 -vf fps=25 -pix_fmt yuv420p {2}";
                if (VideoType.Name.StartsWith("4K"))
                {
                    parameters = @"-r {0} -i {1}\img00%04d.jpg -c:v libx265 -vf fps=25 {2}";
                }
                OutPut.Insert(0, "Generating video ..... ");
                Process newprocess = new Process();
                Progress = 0;
                ProgressMax = (MaxValue - MinValue)*25/Fps;
                newprocess.StartInfo = new ProcessStartInfo()
                {
                    FileName = ffmpegPath,
                    Arguments = string.Format(parameters, Fps, tempFolder, OutPutFile),
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Minimized,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                newprocess.Start();
                newprocess.OutputDataReceived += newprocess_OutputDataReceived;
                newprocess.ErrorDataReceived += newprocess_OutputDataReceived;
                newprocess.BeginOutputReadLine();
                newprocess.BeginErrorReadLine();
                newprocess.WaitForExit();
            }
            catch (Exception exception)
            {
                OutPut.Insert(0, "Converting error :" + exception.Message);
            }

            DeleteTempFolder(tempFolder);

            OutPut.Insert(0, "DONE !!!");
        }

        private void DeleteTempFolder(string tempFolder)
        {
            try
            {
                OutPut.Insert(0, "Removing temporary folder ..");
                Directory.Delete(tempFolder,true);
            }
            catch (Exception ex)
            {
                OutPut.Insert(0, "Error :" + ex.Message);
            }
            
        }

        private void newprocess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                lock (_locker)
                {
                    OutPut.Insert(0, e.Data);
                    //frame=   20 fps= 12 q=0.0 size=       0kB time=00:00:00.00 bitrate=N/A    
                    if (e.Data != null && e.Data.StartsWith("frame="))
                    {
                        var s = e.Data.Substring(7, 5).Trim();
                        int i = 0;
                        if (int.TryParse(e.Data.Substring(7, 5).Trim(), out i))
                            Progress = i;
                    }
                }
            }
            catch (Exception)
            {
  
            }
        }


        public void CopyFile(string filename, string destFile)
        {
           using (MagickImage image = new MagickImage(filename))
            {
                double zw = (double)VideoType.Width / image.Width;
                double zh = (double)VideoType.Height /image.Height;
                double za = FillImage ? ((zw <= zh) ? zw : zh) : ((zw >= zh) ? zw : zh);

                if (FillImage)
                {
                    double aspect = (double) VideoType.Width/VideoType.Height;
                    double pAspect = (double) image.Width/image.Height;
                    if (aspect > pAspect)
                        image.Crop(image.Width, (int) (image.Width/aspect), Gravity.Center);
                    else
                        image.Crop((int) (image.Height/aspect), image.Height, Gravity.Center);
                }

                MagickGeometry geometry = new MagickGeometry(VideoType.Width, VideoType.Height)
                {
                    IgnoreAspectRatio = false,
                    FillArea = false
                };

                image.FilterType = FilterType.Point;
                image.Resize(geometry);
                image.Quality = 80;
                image.Format = MagickFormat.Jpeg;
                image.Write(destFile);
            }
        }

        private void Stop()
        {
            _canceling = true;
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
            }
        }

    }
}
