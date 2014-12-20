using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.ViewModel
{
    public class PrintViewModel : ViewModelBase
    {
        private string _printerName;
        private string _paperName;
        private int _pageWidth;
        private int _pageHeight;
        private int _rows;
        private int _cols;
        private ObservableCollection<PrintItemViewModel> _items;
        public RelayCommand PrintSetupCommand { get; set; }
        public RelayCommand PageSetupCommand { get; set; }
        public RelayCommand PrintCommand { get; set; }

        public PrintDialog dlg = new PrintDialog();
        private int _marginBetweenImages;
        private bool _rotate;
        private bool _repeat;


        public string PrinterName
        {
            get { return _printerName; }
            set
            {
                _printerName = value;
                RaisePropertyChanged(() => PrinterName);
            }
        }

        public string PaperName
        {
            get { return _paperName; }
            set
            {
                _paperName = value;
                RaisePropertyChanged(() => PaperName);
            }
        }

        public int PageWidth
        {
            get { return _pageWidth; }
            set
            {
                _pageWidth = value;
                RaisePropertyChanged(()=>PageWidth);
            }
        }

        public int PageHeight
        {
            get { return _pageHeight; }
            set
            {
                _pageHeight = value;
                RaisePropertyChanged(() => PageHeight);
            }
        }

        public int Rows
        {
            get { return _rows; }
            set
            {
                _rows = value;
                RaisePropertyChanged(() => Rows);
                InitItems();
            }
        }

        public int Cols
        {
            get { return _cols; }
            set
            {
                _cols = value;
                RaisePropertyChanged(() => Cols);
                InitItems();
            }
        }

        public int MarginBetweenImages
        {
            get { return _marginBetweenImages; }
            set
            {
                _marginBetweenImages = value;
                RaisePropertyChanged(() => MarginBetweenImages);
            }
        }

        public bool Repeat
        {
            get { return _repeat; }
            set
            {
                _repeat = value;
                RaisePropertyChanged(()=>Repeat);
                InitItems();
            }
        }

        public ObservableCollection<PrintItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged(() => Items);
            }
        }

        public bool Rotate
        {
            get { return _rotate; }
            set
            {
                _rotate = value;
                RaisePropertyChanged(() => Rotate);
                InitItems();
            }
        }


        public PrintViewModel()
        {
            PrintSetupCommand = new RelayCommand(PrintSetup);
            PageSetupCommand = new RelayCommand(PageSetup);
            PrintCommand = new RelayCommand(Print);
            if (!IsInDesignMode)
            {
                Items = new ObservableCollection<PrintItemViewModel>();
                Rows = 1;
                Cols = 1;
                MarginBetweenImages = 0;
            }
            LoadPrinterSettings();
        }

        private void Print()
        {
            
        }

        private void InitItems()
        {
            Items = new ObservableCollection<PrintItemViewModel>();
            var files = ServiceProvider.Settings.DefaultSession.GetSelectedFiles();
            if (files.Count == 0)
                files.Add(ServiceProvider.Settings.SelectedBitmap.FileItem);
            for (int i = 0; i < Rows*Cols; i++)
            {
                FileItem file = null;
                if (Repeat)
                {
                    file = files[i % files.Count];
                }
                else
                {
                    if (i < files.Count)
                    file = files[i];
                }
                Items.Add(new PrintItemViewModel() {FileItem = file, Parent = this});
            }
        }

        private void PrintSetup()
        {

            if (dlg.ShowDialog() == true)
            {
                LoadPrinterSettings();
            }
        }

        private void LoadPrinterSettings()
        {
            try
            {
                PrinterName = dlg.PrintQueue.Name;
                System.Printing.PrintCapabilities capabilities = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);
                PageWidth = (int) capabilities.PageImageableArea.ExtentWidth;
                PageHeight = (int) capabilities.PageImageableArea.ExtentHeight;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Unable to load settings " + exception.Message);
                Log.Error("Unable to load settings", exception);
            }
        }

        private void PageSetup()
        {

        }

    }
}
