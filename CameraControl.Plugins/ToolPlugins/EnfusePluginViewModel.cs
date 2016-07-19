using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Plugins.ImageTransformPlugins;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.ToolPlugins
{
    public class EnfusePluginViewModel : StackViewModel
    {
        private string _pathtoalign = "";
        private string _pathtoenfuse = "";
        private Process _alignProcess;
        private Process _enfuseProcess;
        private PluginSetting _pluginSetting;
        private bool _exposureEnabled;
        private bool _contrastEnabled;
        private bool _saturationEnabled;
        private bool _muEnabled;
        private bool _sigmaEnabled;
        private FileItem _selectedFileItem;

    


        public PluginSetting PluginSetting
        {
            get
            {
                if (_pluginSetting == null)
                {
                    _pluginSetting = ServiceProvider.Settings["EnfusePlugin"];
                    if (_pluginSetting.Values.Count == 0)
                        SetDefault();
                }
                return _pluginSetting;
            }
        }

        public int Mode
        {
            get { return PluginSetting.GetInt("Mode"); }
            set
            {
                var oldVal = Mode;
                PluginSetting["Mode"] = value;
                if (oldVal != value)
                    SetDefault();
                SetEnabled();
            }
        }


        public int ContrastWindowSize
        {
            get { return PluginSetting.GetInt("ContrastWindowSize"); }
            set
            {
                PluginSetting["ContrastWindowSize"] = value;
                RaisePropertyChanged(() => ContrastWindowSize);
            }
        }

        public int Exposure
        {
            get { return PluginSetting.GetInt("Exposure"); }
            set
            {
                PluginSetting["Exposure"] = value;
                RaisePropertyChanged(()=>Exposure);
            }
        }

        public bool ExposureEnabled
        {
            get { return _exposureEnabled; }
            set
            {
                _exposureEnabled = value;
                RaisePropertyChanged(() => ExposureEnabled);
            }
        }

        public int Contrast
        {
            get { return PluginSetting.GetInt("Contrast"); }
            set
            {
                PluginSetting["Contrast"] = value;
                RaisePropertyChanged(() => Contrast);
            }
        }

        public bool ContrastEnabled
        {
            get { return _contrastEnabled; }
            set
            {
                _contrastEnabled = value;
                RaisePropertyChanged(() => ContrastEnabled);
            }
        }

        public int Saturation
        {
            get { return PluginSetting.GetInt("Saturation"); }
            set
            {
                PluginSetting["Saturation"] = value;
                RaisePropertyChanged(() => Saturation);
            }
        }

        public bool SaturationEnabled
        {
            get { return _saturationEnabled; }
            set
            {
                _saturationEnabled = value;
                RaisePropertyChanged(()=>SaturationEnabled);
            }
        }

        public int Mu
        {
            get { return PluginSetting.GetInt("Mu"); }
            set
            {
                PluginSetting["Mu"] = value;
                RaisePropertyChanged(() => Mu);
            }
        }

        public bool MuEnabled
        {
            get { return _muEnabled; }
            set
            {
                _muEnabled = value;
                RaisePropertyChanged(() => MuEnabled);
            }
        }

        public int Sigma
        {
            get { return PluginSetting.GetInt("Sigma"); }
            set
            {
                PluginSetting["Sigma"] = value;
                RaisePropertyChanged(() => Sigma);
            }
        }

        public bool SigmaEnabled
        {
            get { return _sigmaEnabled; }
            set
            {
                _sigmaEnabled = value;
                RaisePropertyChanged(() => SigmaEnabled);
            }
        }

        public bool AutoPreview
        {
            get { return PluginSetting.GetBool("AutoPreview"); }
            set
            {
                PluginSetting["AutoPreview"] = value;
                RaisePropertyChanged(() => AutoPreview);
            }
        }


        public bool HardMask
        {
            get { return PluginSetting.GetBool("HardMask"); }
            set
            {
                PluginSetting["HardMask"] = value;
                RaisePropertyChanged(() => HardMask);
            }
        }

        public bool AlignImages
        {
            get { return PluginSetting.GetBool("AlignImages"); }
            set
            {
                PluginSetting["AlignImages"] = value;
                RaisePropertyChanged(() => AlignImages);
            }
        }

        public bool UseGpu
        {
            get { return PluginSetting.GetBool("UseGpu"); }
            set
            {
                PluginSetting["UseGpu"] = value;
                RaisePropertyChanged(() => UseGpu);
            }
        }

        public bool OptimizeFiledOfView
        {
            get { return PluginSetting.GetBool("OptimizeFiledOfView"); }
            set
            {
                PluginSetting["OptimizeFiledOfView"] = value;
                RaisePropertyChanged(() => OptimizeFiledOfView);
            }
        }

        public FileItem SelectedFileItem
        {
            get { return _selectedFileItem; }
            set
            {
                _selectedFileItem = value;
                if (_selectedFileItem!=null && File.Exists(_selectedFileItem.LargeThumb))
                    PreviewBitmap = BitmapLoader.Instance.LoadImage(_selectedFileItem.LargeThumb);
                RaisePropertyChanged(() => SelectedFileItem);
            }
        }


        public bool UseSmallThumb
        {
            get { return PluginSetting.GetBool("UseSmallThumb"); }
            set
            {
                PluginSetting["UseSmallThumb"] = value;
                RaisePropertyChanged(() => UseSmallThumb);
            }
        }

        public EnfusePluginViewModel()
        {
            
        }

        public EnfusePluginViewModel(Window window)
        {
            if (!IsInDesignMode)
            {
                window.Closing += window_Closing;
                ServiceProvider.FileTransfered += ServiceProvider_FileTransfered;
                ServiceProvider.WindowsManager.Event += WindowsManager_Event;
                SetEnabled();
                ResetCommand = new RelayCommand(SetDefault);
                PreviewCommand = new RelayCommand(Preview);
                GenerateCommand = new RelayCommand(Generate);
                StopCommand=new RelayCommand(Stop);
                ConfPluginCommand = new RelayCommand(ConfPlugin);
                InitCommands();
                Output = new AsyncObservableCollection<string>();
                LoadData();
                if (Files.Count > 0)
                    SelectedFileItem = Files[0];
                _pathtoalign = Path.Combine(Settings.ApplicationFolder, "Tools", "align_image_stack.exe");
                var hugin = @"c:\Program Files (x86)\Hugin\bin\align_image_stack.exe";
                // use hugin installation 
                if (File.Exists(hugin))
                {
                    _pathtoalign = hugin;
                    OnProgressChange("Hugin installed using align_image_stack.exe");
                }
                else
                {
                    OnProgressChange("Hugin not installed. Instal x86 version for betteer image align performance");
                }

                _pathtoenfuse = Path.Combine(Settings.ApplicationFolder, "Tools", "x64", "enfuse.exe");
            }
        }

        void WindowsManager_Event(string cmd, object o)
        {
            
        }

        private void ServiceProvider_FileTransfered(object sender, FileItem fileItem)
        {
            if (Files.Count > 0)
            {
                if (Files[0].Series != fileItem.Series)
                {
                    Files.Clear();
                }
            }
            Files.Add(fileItem);
            if (IsFree && AutoPreview && Files.Count > 1)
            {
                Task.Factory.StartNew(WaitAndPreview);
            }
        }

        private void WaitAndPreview()
        {
            Thread.Sleep(700);
            try
            {
                while (Files[Files.Count-1].Loading)
                {
                    Thread.Sleep(200);
                }
                Preview();
            }
            catch (Exception ex)
            {
                Log.Error("WaitAndPreview", ex);
            }
        }

        void window_Closing(object sender, CancelEventArgs e)
        {
            ServiceProvider.FileTransfered -= ServiceProvider_FileTransfered;
            ServiceProvider.WindowsManager.Event -= WindowsManager_Event;
        }

        private void ConfPlugin()
        {
            TransformPluginEditView wnd = new TransformPluginEditView();
            wnd.DataContext = new TransformPluginEditViewModel(PluginSetting.AutoExportPluginConfig);
//            wnd.Owner = window;
            wnd.ShowDialog();
        }


        private void Stop()
        {
            _shouldStop = true;
            try
            {
                if (_alignProcess != null)
                    _alignProcess.Kill();
                if (_enfuseProcess != null)
                    _enfuseProcess.Kill();
            }
            catch (Exception)
            {

            }
        }

        private void Preview()
        {
            Output.Clear();
            _shouldStop = false;
            Task.Factory.StartNew(PreviewTask);
        }

        private void Generate()
        {
            Output.Clear();
            _shouldStop = false;
            Task.Factory.StartNew(GenerateTask);
        }

        private void GenerateTask()
        {
            IsBusy = true;
            CopyFiles(false);
            if (!_shouldStop && AlignImages)
                AlignImagesProcess();
            if (!_shouldStop)
                EnfuseImge();
            if (File.Exists(_resulfile))
            {
                string newFile = Path.Combine(Path.GetDirectoryName(Files[0].FileName),
                    Path.GetFileNameWithoutExtension(Files[0].FileName) + "_enfuse" + ".jpg");
                newFile = PhotoUtils.GetNextFileName(newFile);

                File.Copy(_resulfile, newFile, true);

                if (ServiceProvider.Settings.DefaultSession.GetFile(newFile) == null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        FileItem im = new FileItem(newFile);
                        im.Transformed = true;
                        ServiceProvider.Settings.DefaultSession.Files.Add(im);
                    }));
                }
            }
            OnActionDone();
            IsBusy = false;
        }

        void PreviewTask()
        {
            IsBusy = true;
            CopyFiles(true);
            if (!_shouldStop && AlignImages)
                AlignImagesProcess();
            if(!_shouldStop)
                EnfuseImge();
            OnActionDone();
            IsBusy = false;
        }

        public void SetEnabled()
        {
            switch (Mode)
            {
                case 0:
                    ExposureEnabled = true;
                    ContrastEnabled = false;
                    SaturationEnabled = true;
                    MuEnabled = true;
                    SigmaEnabled = true;
                    OptimizeFiledOfView = true;
                    break;
                case 1:
                    ExposureEnabled = false;
                    ContrastEnabled = true;
                    SaturationEnabled = false;
                    MuEnabled = true;
                    SigmaEnabled = true;
                    OptimizeFiledOfView = false;
                    break;
            }
        }
        
        private void SetDefault()
        {
            switch (Mode)
            {
                case 0:
                    Exposure = 100;
                    Contrast = 0;
                    Saturation = 100;
                    Mu = 50;
                    Sigma = 20;
                    HardMask = false;
                    break;
                case 1:
                    Exposure = 0;
                    Contrast = 100;
                    Saturation = 0;
                    Mu = 50;
                    Sigma = 20;
                    HardMask = true;
                    break;
            }
            ContrastWindowSize = 5;
        }

        private void CopyFiles(bool preview)
        {
            int counter = 0;
            try
            {
                _filenames.Clear();
                OnProgressChange("Copying files");
                _tempdir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(_tempdir);
                foreach (FileItem fileItem in Files)
                {
                    string randomFile = Path.Combine(_tempdir, "image_" + counter.ToString("0000") + ".jpg");
                    OnProgressChange("Copying file " + fileItem.Name);
                    string source = preview
                        ? (UseSmallThumb ? fileItem.SmallThumb : fileItem.LargeThumb)
                        : fileItem.FileName;

                    AutoExportPluginHelper.ExecuteTransformPlugins(fileItem, PluginSetting.AutoExportPluginConfig,
                        source, randomFile);
                    
                    _filenames.Add(randomFile);
                    counter++;
                    if (_shouldStop)
                    {
                        OnActionDone();
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                OnProgressChange("Error copy files " + exception.Message);
                Log.Error("Error copy files ", exception);
                _shouldStop = true;
            }
        }


        private void AlignImagesProcess()
        {
            try
            {
                OnProgressChange("Align images ..");
                OnProgressChange("This may take few minutes (5-10) ");
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("-v ");
                stringBuilder.Append("-C ");
                if (UseGpu && _pathtoalign.Contains("Hugin"))
                    stringBuilder.Append("--gpu ");
                if (OptimizeFiledOfView)
                    stringBuilder.Append("-m");
                stringBuilder.Append(" -a " + _filenames[0]);
                foreach (string filename in _filenames)
                {
                    stringBuilder.Append(" " + filename);
                }
                Process newprocess = new Process();
                newprocess.StartInfo = new ProcessStartInfo()
                {
                    FileName = _pathtoalign,
                    Arguments = stringBuilder.ToString(),
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Minimized,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                newprocess.Start();
                _alignProcess = newprocess;
                newprocess.OutputDataReceived += newprocess_OutputDataReceived;
                newprocess.ErrorDataReceived += newprocess_OutputDataReceived;
                newprocess.BeginOutputReadLine();
                newprocess.BeginErrorReadLine();
                newprocess.WaitForExit();
            }
            catch (Exception exception)
            {
                OnProgressChange("Error copy files " + exception.Message);
                Log.Error("Error copy files ", exception);
                _shouldStop = true;
            }
            _alignProcess = null;
        }

        private void EnfuseImge()
        {
            try
            {
                OnProgressChange("Enfuse images ..");
                OnProgressChange("This may take few minutes too");
                _resulfile = Path.Combine(_tempdir, Path.GetFileName(Files[0].FileName) + Files.Count + "_enfuse.jpg");
                _resulfile =
                    StaticHelper.GetUniqueFilename(
                        Path.GetDirectoryName(Files[0].FileName) + "\\" +
                        Path.GetFileNameWithoutExtension(Files[0].FileName) + "_enfuse", 0, ".jpg");
                _resulfile = Path.Combine(_tempdir, Path.GetFileName(_resulfile));
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(" -o " + _resulfile);
                stringBuilder.Append(" --exposure-weight=" + decimal.Round((decimal)(Exposure / 100.0), 2));
                stringBuilder.Append(" --saturation-weight=" + decimal.Round((decimal)(Saturation / 100.2), 2));
                stringBuilder.Append(" --contrast-weight=" + decimal.Round((decimal)(Contrast / 100.0), 2));
                //stringBuilder.Append(" --entropy-weight=" + decimal.Round((decimal)(_settings.EnfuseEnt / 100), 2));
                stringBuilder.Append(" --exposure-sigma=" + decimal.Round((decimal)(Sigma / 100.0), 2));
                stringBuilder.Append(" --contrast-window-size=" + ContrastWindowSize);
                stringBuilder.Append(" --gray-projector=l-star");
                
                if (HardMask)
                {
                    stringBuilder.Append(" --hard-mask");
                }
                if (AlignImages)
                    stringBuilder.Append(" " + _filenames[0] + "????.tif");
                else
                    stringBuilder.Append(" " + Path.GetDirectoryName(_filenames[0]) + "\\image_????.jpg");
                //string param = " -o " + _resulfile + " --exposure-weight=0 --saturation-weight=0 --contrast-weight=1 --hard-mask --contrast-window-size=9 " + _filenames[0] + "????.tif";

                Process newprocess = new Process();
                newprocess.StartInfo = new ProcessStartInfo()
                {
                    FileName = _pathtoenfuse,
                    Arguments = stringBuilder.ToString().Replace(",", "."),
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Minimized,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                newprocess.Start();
                _enfuseProcess = newprocess;
                newprocess.OutputDataReceived += newprocess_OutputDataReceived;
                newprocess.ErrorDataReceived += newprocess_OutputDataReceived;
                newprocess.BeginOutputReadLine();
                newprocess.BeginErrorReadLine();
                newprocess.WaitForExit();
                if (File.Exists(_resulfile))
                {
                    //string localfile = Path.Combine(Path.GetDirectoryName(_files[0].FileName),
                    //                                Path.GetFileName(_resulfile));
                    //File.Copy(_resulfile, localfile, true);
                    //ServiceProvider.Settings.DefaultSession.AddFile(localfile);
                    PreviewBitmap = BitmapLoader.Instance.LoadImage(_resulfile);
                }
                else
                {
                    OnProgressChange("No output file something went wrong !");
                }
                _enfuseProcess = null;
            }
            catch (Exception exception)
            {
                OnProgressChange("Error copy files " + exception.Message);
                Log.Error("Error copy files ", exception);
                _shouldStop = true;
            }
        }

        private void newprocess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnProgressChange(e.Data);
        }


    }
}
