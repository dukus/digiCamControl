using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }

        public string Barcode
        {
            get { return ServiceProvider.Settings.DefaultSession.Barcode; }
            set
            {
                ServiceProvider.Settings.DefaultSession.Barcode = value;
                RaisePropertyChanged(() => Barcode);
            }
        }

        public ObservableCollection<ExternalData> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(() => Data);
            }
        }


        public BarcodeViewModel()
        {
            ImportCommand = new RelayCommand(Import);
            ExportCommand = new RelayCommand(Export);
            Data = new ObservableCollection<ExternalData>();

        }

        private void Export()
        {
            
        }

        private void Import()
        {
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
