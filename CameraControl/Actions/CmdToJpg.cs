using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;

namespace CameraControl.Actions
{
    public class CmdToJpg : BaseFieldClass, IMenuAction, ICommand
    {
        #region Implementation of IMenuAction

        public event EventHandler ProgressChanged;

        public void OnProgressChanged(EventArgs e)
        {
            EventHandler handler = ProgressChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler ActionDone;

        public void OnActionDone(EventArgs e)
        {
            EventHandler handler = ActionDone;
            if (handler != null) handler(this, e);
        }

        public int Progress { get; set; }

        public string Title
        {
            get { return "Convert raw to jpg"; }
            set { }
        }

        public bool IsBusy { get; set; }

        public void Run(List<string> files)
        {

        }

        public void Stop()
        {

        }

        #endregion

        #region Implementation of ICommand

        public void Execute(object parameter)
        {
            if (!ServiceProvider.Settings.SelectedBitmap.FileItem.IsRaw)
            {
                MessageBox.Show("Raw file is needed for this action");
                return;
            }
            string _infile = ServiceProvider.Settings.SelectedBitmap.FileItem.FileName;
            string _otfile = Path.Combine(Path.GetDirectoryName(_infile), Path.GetFileNameWithoutExtension(_infile) + ".jpg");
            string _pathtoufraw =
              Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "ufraw",
                           "ufraw-batch.exe");
            if (File.Exists(_pathtoufraw))
            {
                if (PhotoUtils.RunAndWait(_pathtoufraw,
                                          " --wb=camera --saturation=1.2 --exposure=0 --black-point=auto --overwrite --out-type=jpg --output=" +
                                          _otfile + " " + _infile))
                {
                    ServiceProvider.Settings.DefaultSession.AddFile(_otfile);
                }
            }
            else
            {
                MessageBox.Show("Ufraw not found !");
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
