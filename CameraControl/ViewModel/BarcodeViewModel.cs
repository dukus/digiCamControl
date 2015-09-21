using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace CameraControl.ViewModel
{
    public class BarcodeViewModel : ViewModelBase
    {
        private ObservableCollection<ExternalData> _data;
        private bool _captureOnEnter;
        private bool _keepActive;
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }

        public string Barcode
        {
            get { return ServiceProvider.Settings.DefaultSession.Barcode; }
            set
            {
                ServiceProvider.Settings.DefaultSession.Barcode = value;
                SelectedRowData = Data.FirstOrDefault(x => x.GetAllData().Contains(Barcode.ToLower()));
                RaisePropertyChanged(() => Barcode);
            }
        }

        public bool CaptureOnEnter
        {
            get { return _captureOnEnter; }
            set
            {
                _captureOnEnter = value;
                RaisePropertyChanged(() => CaptureOnEnter);
            }
        }

        public bool KeepActive
        {
            get { return _keepActive; }
            set
            {
                _keepActive = value;
                RaisePropertyChanged(() => KeepActive);
            }
        }


        public int Delay { get; set; }

        public ObservableCollection<ExternalData> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(() => Data);
            }
        }

        public ExternalData SelectedRowData
        {
            get { return ServiceProvider.Settings.DefaultSession.ExternalData; }
            set
            {
                ServiceProvider.Settings.DefaultSession.ExternalData = value;
                RaisePropertyChanged(() => SelectedRowData);
            }
        }

        public BarcodeViewModel()
        {
            ImportCommand = new RelayCommand(Import);
            ExportCommand = new RelayCommand(Export);
            Data = new ObservableCollection<ExternalData>();
            if (!IsInDesignMode)
            {
                ServiceProvider.WindowsManager.Event += WindowsManager_Event;
                try
                {
                    string file = Path.Combine(Path.GetDirectoryName(ServiceProvider.Settings.DefaultSession.ConfigFile),
                        ServiceProvider.Settings.DefaultSession.Name + "_data.csv");
                    if (File.Exists(file))
                        ImportCsv(file);
                }
                catch (Exception ex)
                {
                    Log.Error("Unable to load CSV data", ex);
                }
            }
            KeepActive = true;
        }

        private void WindowsManager_Event(string cmd, object o)
        {
            if (cmd == CmdConsts.All_Close || cmd == WindowsCmdConsts.BarcodeWnd_Hide)
            {
                string file = Path.Combine(Path.GetDirectoryName(ServiceProvider.Settings.DefaultSession.ConfigFile),
                    ServiceProvider.Settings.DefaultSession.Name + "_data.csv");
                ExportCsv(file);
            }
        }

        private void Export()
        {
            var state = KeepActive;
            KeepActive = false;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Csv file|*.csv|All files|*.*";
            if (dialog.ShowDialog() == true)
            {
                ExportCsv(dialog.FileName);
            }
            KeepActive = state;
        }

        private void Import()
        {
            var state = KeepActive;
            KeepActive = false;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Csv file|*.csv|All files|*.*";
            if (dialog.ShowDialog() == true)
            {
                var extension = Path.GetExtension(dialog.FileName);
                if (extension != null)
                {
                    extension = extension.ToLower();
                    switch (extension)
                    {
                        case ".txt":
                        case ".csv":
                            ImportCsv(dialog.FileName);
                            break;
                        default:
                            MessageBox.Show("Unknow file format");
                            break;
                    }
                }
            }
            KeepActive = state;
        }

        public void EnterPressed()
        {
            if (CaptureOnEnter)
                ServiceProvider.WindowsManager.ExecuteCommand(CmdConsts.Capture);
        }

        private void ExportCsv(string file)
        {
            try
            {
                using (TextWriter writer = File.CreateText(file))
                {
                    foreach (var data in Data)
                    {
                        writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", data.Row1, data.Row2, data.Row3,
                            data.Row4, data.Row5, data.Row6, data.Row7, data.Row8, data.Row9, data.FileName);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to export CSV", exception);
            }
        }

        private void ImportCsv(string file)
        {
            try
            {
                string[] lines = File.ReadAllLines(file);
                Data = new ObservableCollection<ExternalData>();
                foreach (string line in lines)
                {
                    ExternalData externalData=new ExternalData();
                    var data = line.Split(',');
                    if (data.Length > 0)
                        externalData.Row1 = data[0];
                    if (data.Length > 1)
                        externalData.Row2 = data[1];
                    if (data.Length > 2)
                        externalData.Row3 = data[2];
                    if (data.Length > 3)
                        externalData.Row4 = data[3];
                    if (data.Length > 4)
                        externalData.Row5 = data[4];
                    if (data.Length > 5)
                        externalData.Row6 = data[5];
                    if (data.Length > 6)
                        externalData.Row7= data[6];
                    if (data.Length > 7)
                        externalData.Row8 = data[7];
                    if (data.Length > 8)
                        externalData.Row9 = data[8];
                    if (data.Length > 9)
                        externalData.FileName = data[9];
                    Data.Add(externalData);
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error import file", ex);
            }
        }
    }
}
