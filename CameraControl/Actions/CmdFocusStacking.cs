using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using CameraControl.Classes;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.windows;
using CameraControl.Core;

namespace CameraControl.Actions
{
    public class CmdFocusStacking : BaseFieldClass, IMenuAction, ICommand
    {
        #region Implementation of IMenuAction

        private int _progress;
        private bool _shouldStop;
        private string _tempdir = "";
        private string _pathtoalign = "";
        private string _pathtoenfuse = "";
        private string _resulfile = "";
        private List<string> _filenames = new List<string>();
        private AsyncObservableCollection<FileItem> _files;

        public event EventHandler ProgressChanged;
        public event EventHandler ActionDone;


        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        public string Title
        {
            get { return "Generate \"Focus Stacked\" image"; }
            set { throw new NotImplementedException(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged("IsBusy");
            }
        }

        private void CopyFiles()
        {
            try
            {
                OnProgressChange("Copying files");
                _tempdir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(_tempdir);
                foreach (FileItem fileItem in _files)
                {
                    string randomFile = Path.Combine(_tempdir, Path.GetRandomFileName() + Path.GetExtension(fileItem.FileName));
                    OnProgressChange("Copying file " + fileItem.Name);
                    File.Copy(fileItem.FileName, randomFile, true);
                    _filenames.Add(randomFile);
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

        private void AlignImages()
        {
            try
            {
                OnProgressChange("Align images ..");
                OnProgressChange("This may take few minutes (5-15) ");
                string param = " -m -a " + _filenames[0];
                foreach (string filename in _filenames)
                {
                    param += " " + filename;
                }
                ProcessStartInfo startInfo = new ProcessStartInfo(_pathtoalign);
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = param;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                Process process = Process.Start(startInfo);

                process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
            catch (Exception exception)
            {
                OnProgressChange("Error copy files " + exception.Message);
                Log.Error("Error copy files ", exception);
                _shouldStop = true;
            }
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                OnProgressChange(e.Data);
        }

        private void Enfuse()
        {
            try
            {
                OnProgressChange("Enfuse images ..");
                OnProgressChange("This may take few minutes too");
                _resulfile = Path.Combine(_tempdir, Path.GetFileName(_files[0].FileName) + _files.Count + "_enfuse.tif");
                string param = " -o " + _resulfile + " --exposure-weight=0 --saturation-weight=0 --contrast-weight=1 --hard-mask --contrast-window-size=5 " + _filenames[0] + "????.tif";
                ProcessStartInfo startInfo = new ProcessStartInfo(_pathtoenfuse);
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = param;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                Process process = Process.Start(startInfo);
                process.OutputDataReceived += process_OutputDataReceived;
                process.BeginOutputReadLine();
                process.WaitForExit();
                if (File.Exists(_resulfile))
                {
                    string localfile = Path.Combine(Path.GetDirectoryName(_files[0].FileName), Path.GetFileName(_resulfile));
                    File.Copy(_resulfile, localfile, true);
                    ServiceProvider.Settings.DefaultSession.AddFile(localfile);
                }
                else
                {
                    OnProgressChange("No output file something went wrong !");
                }
            }
            catch (Exception exception)
            {
                OnProgressChange("Error copy files " + exception.Message);
                Log.Error("Error copy files ", exception);
                _shouldStop = true;
            }
        }

        public void Run(List<string> f)
        {
            IsBusy = true;
            _shouldStop = false;
            _pathtoalign = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Tools", "align_image_stack.exe");
            _pathtoenfuse = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Tools", "enfuse.exe");
            _files = ServiceProvider.Settings.DefaultSession.GetSelectedFiles();
            if (_files.Count < 2)
            {
                OnProgressChange("You should select more that 2 files");
                OnActionDone();
                return;
            }
            if (!File.Exists(_pathtoalign))
            {
                OnProgressChange("File not found: align_image_stack.exe");
                OnProgressChange("Hugin not installed or not configured !");
                OnActionDone();
                return;
            }
            if (!File.Exists(_pathtoenfuse))
            {
                OnProgressChange("File not found: enfuse.exe");
                OnProgressChange("Hugin not installed or not configured !");
                OnActionDone();
                return;
            }
            CopyFiles();
            if (_shouldStop)
            {
                OnActionDone();
                return;
            }
            AlignImages();
            if (_shouldStop)
            {
                OnActionDone();
                return;
            }
            Enfuse();
            if (_shouldStop)
            {
                OnActionDone();
                return;
            }
            OnProgressChange("Opeation done");
            OnActionDone();
        }

        public void Stop()
        {
            _shouldStop = true;
            OnProgressChange("Stopping ....");
        }

        #endregion

        private void OnProgressChange(string s)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new ActionEventArgs() { Message = s });
        }

        private void OnActionDone()
        {
            try
            {
                if (Directory.Exists(_tempdir))
                    Directory.Delete(_tempdir, true);
            }
            catch (Exception)
            {
                OnProgressChange("Unable to delete temporary folder");
            }
            if (ActionDone != null)
                ActionDone(this, new EventArgs());
            IsBusy = false;
            ServiceProvider.Settings.DefaultSession.SelectNone();
        }

        #region Implementation of ICommand

        public void Execute(object parameter)
        {
            IMenuAction action = parameter as IMenuAction;
            if (action != null)
            {
                ActionExecuteWnd wnd = new ActionExecuteWnd(action);
                wnd.ShowDialog();
            }
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void OnCanExecuteChanged(EventArgs e)
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}
