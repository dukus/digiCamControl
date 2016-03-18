using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Scripting.ScriptCommands;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Plugins.ImageTransformPlugins;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using Newtonsoft.Json;

namespace CameraControl.Plugins.ToolPlugins
{
    public class CombineZpViewModel : StackViewModel
    {
        private Process _enfuseProcess;
        private PluginSetting _pluginSetting;
        private string _pathtoenfuse = "";
        private string _combineZpFolder;


        public PluginSetting PluginSetting
        {
            get
            {
                if (_pluginSetting == null)
                {
                    _pluginSetting = ServiceProvider.Settings["CombineZp"];
                }
                return _pluginSetting;
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

        public string CombineZpFolder
        {
            get { return PluginSetting["CombineZpFolder"] as string; }
            set
            {
                PluginSetting["CombineZpFolder"] = value;
                RaisePropertyChanged(()=>CombineZpFolder);
            }
        }

        public List<string> Macros { get; set; }

        public string Macro
        {
            get { return PluginSetting["Macro"] as string; }
            set
            {
                PluginSetting["Macro"] = value;
                RaisePropertyChanged(() => Macro);
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

        public CombineZpViewModel()
        {
            
        }

        public CombineZpViewModel(Window window)
        {
            InitCommands();
            Output = new AsyncObservableCollection<string>();
            Macros = new List<string>();
            LoadData();
            window.Closing += window_Closing;
            ServiceProvider.FileTransfered += ServiceProvider_FileTransfered;
            ServiceProvider.WindowsManager.Event += WindowsManager_Event;
            GenerateCommand = new RelayCommand(Generate);
            PreviewCommand = new RelayCommand(Preview);
            GenerateCommand = new RelayCommand(Generate);
            StopCommand = new RelayCommand(Stop);
            ConfPluginCommand = new RelayCommand(ConfPlugin);
            CombineZpFolder = @"c:\Program Files (x86)\Alan Hadley\CombineZP\";
            _pathtoenfuse = Path.Combine(CombineZpFolder, "CombineZP.exe");
            PopulateMacros();
            if (string.IsNullOrEmpty(Macro))
                Macro = "Do Weighted Average";
        }

        private void PopulateMacros()
        {
            if (Directory.Exists(CombineZpFolder))
            {
                var files = Directory.GetFiles(CombineZpFolder, "*.czm");
                foreach (string file in files)
                {
                    Macros.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        private void ConfPlugin()
        {
            TransformPluginEditView wnd = new TransformPluginEditView();
            wnd.DataContext = new TransformPluginEditViewModel(PluginSetting.AutoExportPluginConfig);
            wnd.ShowDialog();
        }

        private void WindowsManager_Event(string cmd, object o)
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
                while (Files[Files.Count - 1].Loading)
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

        private void Stop()
        {
            _shouldStop = true;
            try
            {
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
            Init();
            Task.Factory.StartNew(GenerateTask);
        }

        public void Init()
        {
            Output.Clear();
            _shouldStop = false;
           
        }

        public void CopyFiles(bool preview)
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

        private void GenerateTask()
        {
            IsBusy = true;
            CopyFiles(false);
            if (!_shouldStop)
                Combine();
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
            if (!_shouldStop)
                Combine();
            OnActionDone();
            IsBusy = false;
        }

        public void Combine()
        {
            try
            {
                OnProgressChange("Enfuse images ..");
                OnProgressChange("This may take few minutes too");
                _resulfile = Path.Combine(_tempdir, Path.GetFileNameWithoutExtension(Files[0].FileName) + Files.Count + ".jpg");
                _resulfile = Path.Combine(Path.GetTempPath(), Path.GetFileName(_resulfile));
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("\""+Path.GetDirectoryName(_filenames[0])+"\" ");
                stringBuilder.Append("\""+Macro+"\" ");
                stringBuilder.Append(_resulfile + " ");
                stringBuilder.Append("-q -k +j100");

                Process newprocess = new Process();
                newprocess.StartInfo = new ProcessStartInfo()
                {
                    FileName = _pathtoenfuse,
                    Arguments = stringBuilder.ToString().Replace(",", "."),
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Minimized,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(_filenames[0])
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

        private void window_Closing(object sender, CancelEventArgs e)
        {
            ServiceProvider.FileTransfered -= ServiceProvider_FileTransfered;
            ServiceProvider.WindowsManager.Event -= WindowsManager_Event;   
        }
    }
}
