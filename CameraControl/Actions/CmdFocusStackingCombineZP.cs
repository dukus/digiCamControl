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
    public class CmdFocusStackingCombineZP : BaseFieldClass, IMenuAction, ICommand
    {
        #region Implementation of IMenuAction

        private int _progress;
        private bool _shouldStop;
        private string _tempdir = "";
        private string _pathtoexe = "";
        private string _resulfile = "";
        private List<string> _filenames = new List<string>();
        private AsyncObservableCollection<FileItem> _files;

        public event EventHandler ProgressChanged;
        public event EventHandler ActionDone;

        public CmdFocusStackingCombineZP()
        {
            _pathtoexe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Alan Hadley\\CombineZP",
                                      "CombineZP.exe");
        }

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
            get { return "Generate \"Focus Stacked\" image using CombineZP"; }
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
                    PhotoUtils.CopyPhotoScale(fileItem.FileName, randomFile, 1);
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


        private void Enfuse()
        {
            try
            {
                OnProgressChange("Combine images ..");
                OnProgressChange("This may take few minutes");
                _resulfile = Path.Combine(_tempdir, Path.GetFileName(_files[0].FileName) + _files.Count + "_enfuse.tif");
                _resulfile =
                  StaticHelper.GetUniqueFilename(
                    Path.GetDirectoryName(_files[0].FileName) + "\\" +
                    Path.GetFileNameWithoutExtension(_files[0].FileName) + "_enfuse", 0, ".jpg");
                //_resulfile = Path.Combine(_tempdir, Path.GetFileName(_resulfile));

                string param = "\"" + _tempdir + "\" \"Do Stack\" \"" + _resulfile + "\" -q +j100";
                //" -o " + _resulfile + " --exposure-weight=0 --saturation-weight=0 --contrast-weight=1 --hard-mask --contrast-window-size=9 " + _filenames[0] + "????.tif";
                ProcessStartInfo startInfo = new ProcessStartInfo(_pathtoexe);
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                startInfo.Arguments = param;
                Process process = Process.Start(startInfo);
                process.WaitForExit();
                if (File.Exists(_resulfile))
                {
                    string localfile = Path.Combine(Path.GetDirectoryName(_files[0].FileName), Path.GetFileName(_resulfile));
                    //File.Copy(_resulfile, localfile, true);
                    ServiceProvider.Settings.DefaultSession.AddFile(localfile);
                    ServiceProvider.Settings.DefaultSession.SelectNone();
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
            _files = ServiceProvider.Settings.DefaultSession.GetSelectedFiles();
            if (_files.Count < 2)
            {
                OnProgressChange("You should select more that 2 files");
                OnActionDone();
                return;
            }
            if (!File.Exists(_pathtoexe))
            {
                OnProgressChange("File not found: CombineZP.exe");
                OnProgressChange("Please install CombineZP");
                OnActionDone();
                return;
            }
            CopyFiles();
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
            return File.Exists(_pathtoexe);
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
