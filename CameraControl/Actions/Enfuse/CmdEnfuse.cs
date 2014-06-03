using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CameraControl.Classes;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.windows;
using CameraControl.Core;

namespace CameraControl.Actions.Enfuse
{
    class CmdEnfuse : BaseFieldClass, IMenuAction, ICommand
    {
        #region Implementation of IMenuAction

        private string _title;
        private bool _isBusy;
        public event EventHandler ProgressChanged;
        public event EventHandler ActionDone;
        private bool _shouldStop;
        private string _tempdir = "";
        private string _pathtoalign = "";
        private string _pathtoenfuse = "";
        private string _resulfile = "";
        private List<string> _filenames = new List<string>();
        private AsyncObservableCollection<FileItem> _files;

        private EnfuseSettings _settings = new EnfuseSettings();

        public int Progress { get; set; }

        public string Title
        {
            get { return "Enfuse images"; }
            set { _title = value; }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; }
        }

        public void Run(List<string> files)
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
            if (_settings.AlignImages)
                AlignImages();
            if (_shouldStop)
            {
                OnActionDone();
                return;
            }
            EnfuseImge();
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

        #region Implementation of ICommand

        public void Execute(object parameter)
        {
            EnfuseSettingsWnd wnds = new EnfuseSettingsWnd { DataContext = _settings };
            if (wnds.ShowDialog() == true)
            {
                IMenuAction action = parameter as IMenuAction;
                if (action != null)
                {
                    ActionExecuteWnd wnd = new ActionExecuteWnd(action);
                    wnd.ShowDialog();
                }
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
        private void CopyFiles()
        {
            int counter = 0;
            try
            {
                _filenames.Clear();
                OnProgressChange("Copying files");
                _tempdir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(_tempdir);
                foreach (FileItem fileItem in _files)
                {
                    string randomFile = Path.Combine(_tempdir, "image_" + counter.ToString("0000") + ".jpg");
                    OnProgressChange("Copying file " + fileItem.Name);
                    PhotoUtils.CopyPhotoScale(fileItem.FileName, randomFile, _settings.Scale == 0 ? 1 : (double)1 / (_settings.Scale * 2));
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
            ServiceProvider.Settings.DefaultSession.SelectNone();
            IsBusy = false;
        }

        private void AlignImages()
        {
            try
            {
                OnProgressChange("Align images ..");
                OnProgressChange("This may take few minutes (5-10) ");
                StringBuilder stringBuilder = new StringBuilder();
                if (_settings.OptimizeFiledOfView)
                    stringBuilder.Append("-m");
                stringBuilder.Append(" -a " + _filenames[0]);
                foreach (string filename in _filenames)
                {
                    stringBuilder.Append(" " + filename);
                }
                ProcessStartInfo startInfo = new ProcessStartInfo(_pathtoalign);
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                startInfo.Arguments = stringBuilder.ToString();
                Process process = Process.Start(startInfo);
                process.WaitForExit();
            }
            catch (Exception exception)
            {
                OnProgressChange("Error copy files " + exception.Message);
                Log.Error("Error copy files ", exception);
                _shouldStop = true;
            }
        }

        private void EnfuseImge()
        {
            try
            {
                OnProgressChange("Enfuse images ..");
                OnProgressChange("This may take few minutes too");
                _resulfile = Path.Combine(_tempdir, Path.GetFileName(_files[0].FileName) + _files.Count + "_enfuse.jpg");
                _resulfile =
                  StaticHelper.GetUniqueFilename(
                    Path.GetDirectoryName(_files[0].FileName) + "\\" +
                    Path.GetFileNameWithoutExtension(_files[0].FileName) + "_enfuse", 0, ".jpg");
                _resulfile = Path.Combine(_tempdir, Path.GetFileName(_resulfile));
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(" -l 20 -o " + _resulfile);
                stringBuilder.Append(" --exposure-weight=" + decimal.Round((decimal)(_settings.EnfuseExp / 100), 2));
                stringBuilder.Append(" --saturation-weight=" + decimal.Round((decimal)(_settings.EnfuseSat / 100), 2));
                stringBuilder.Append(" --contrast-weight=" + decimal.Round((decimal)(_settings.EnfuseCont / 100), 2));
                stringBuilder.Append(" --entropy-weight=" + decimal.Round((decimal)(_settings.EnfuseEnt / 100), 2));
                stringBuilder.Append(" --exposure-sigma=" + decimal.Round((decimal)(_settings.EnfuseSigma / 100), 2));
                stringBuilder.Append(" --contrast-window-size=" + _settings.ContrasWindow);
                if (_settings.HardMask)
                {
                    stringBuilder.Append(" --hard-mask");
                }
                if (_settings.AlignImages)
                    stringBuilder.Append(" " + _filenames[0] + "????.tif");
                else
                    stringBuilder.Append(" " + Path.GetDirectoryName(_filenames[0]) + "\\image_????.jpg");
                //string param = " -o " + _resulfile + " --exposure-weight=0 --saturation-weight=0 --contrast-weight=1 --hard-mask --contrast-window-size=9 " + _filenames[0] + "????.tif";
                ProcessStartInfo startInfo = new ProcessStartInfo(_pathtoenfuse);
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                startInfo.Arguments = stringBuilder.ToString().Replace(",", ".");
                Process process = Process.Start(startInfo);
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
    }
}
