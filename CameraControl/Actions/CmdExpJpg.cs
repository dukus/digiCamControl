#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

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
using CameraControl.windows;

#endregion

namespace CameraControl.Actions
{
    public class CmdExpJpg : BaseFieldClass, IMenuAction, ICommand
    {
        #region Implementation of IMenuAction

        private bool _shouldStop;

        public event EventHandler ProgressChanged;
        public event EventHandler ActionDone;
        public int Progress { get; set; }

        public string Title
        {
            get { return "Convert raw to exposure bracketing jpg's"; }
            set { }
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

        public void Run(List<string> files)
        {
            for (double i = -4; i <= 4; i += 1)
            {
                OnProgressChange("Exposure " + i);
                Exec(i);
                if (_shouldStop)
                {
                    OnActionDone();
                    return;
                }
            }
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
            if (!ServiceProvider.Settings.SelectedBitmap.FileItem.IsRaw)
            {
                MessageBox.Show("Raw file is needed for this action");
                return;
            }

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

        private void OnProgressChange(string s)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new ActionEventArgs() {Message = s});
        }

        private void OnActionDone()
        {
            if (ActionDone != null)
                ActionDone(this, new EventArgs());
            IsBusy = false;
            ServiceProvider.Settings.DefaultSession.SelectNone();
        }

        private void Exec(double br)
        {
            string _infile = ServiceProvider.Settings.SelectedBitmap.FileItem.FileName;
            string _otfile = Path.Combine(Path.GetDirectoryName(_infile),
                                          Path.GetFileNameWithoutExtension(_infile) + (br > 0 ? "+" : "") + br + ".jpg");
            string _pathtoufraw =
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "ufraw",
                             "ufraw-batch.exe");
            if (File.Exists(_pathtoufraw))
            {
                if (PhotoUtils.RunAndWait(_pathtoufraw,
                                          " --wb=camera --saturation=1.2 --exposure=" + br +
                                          " --black-point=auto --overwrite --out-type=jpg --output=" +
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
    }
}