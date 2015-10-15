using System;
using System.Collections.ObjectModel;
using System.Printing;
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
        private ObservableCollection<PrintItemViewModel> _items;
        public RelayCommand PrintSetupCommand { get; set; }
        public RelayCommand PageSetupCommand { get; set; }
        public RelayCommand PrintCommand { get; set; }

        public readonly PrintDialog Dlg = new PrintDialog();

        public PrintSettings PrintSettings { get; set; }

        public string PrinterName
        {
            get { return PrintSettings.PrinterName; }
            set
            {
                PrintSettings.PrinterName = value;
                RaisePropertyChanged(() => PrinterName);
            }
        }

        public string PaperName
        {
            get { return PrintSettings.PaperName; }
            set
            {
                PrintSettings.PaperName = value;
                RaisePropertyChanged(() => PaperName);
            }
        }

        public int PageWidth
        {
            get { return PrintSettings.PageWidth; }
            set
            {
                PrintSettings.PageWidth = value;
                RaisePropertyChanged(()=>PageWidth);
            }
        }

        public int PageHeight
        {
            get { return PrintSettings.PageHeight; }
            set
            {
                PrintSettings.PageHeight = value;
                RaisePropertyChanged(() => PageHeight);
            }
        }

        public int Rows
        {
            get { return PrintSettings.Rows; }
            set
            {
                PrintSettings.Rows = value;
                RaisePropertyChanged(() => Rows);
                InitItems();
            }
        }

        public int Cols
        {
            get { return PrintSettings.Cols; }
            set
            {
                PrintSettings.Cols = value;
                RaisePropertyChanged(() => Cols);
                InitItems();
            }
        }

        public int MarginBetweenImages
        {
            get { return PrintSettings.MarginBetweenImages; }
            set
            {
                PrintSettings.MarginBetweenImages = value;
                RaisePropertyChanged(() => MarginBetweenImages);
            }
        }

        public bool Repeat
        {
            get { return PrintSettings.Repeat; }
            set
            {
                PrintSettings.Repeat = value;
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
            get { return PrintSettings.Rotate; }
            set
            {
                PrintSettings.Rotate = value;
                RaisePropertyChanged(() => Rotate);
                InitItems();
            }
        }

        public bool Fill
        {
            get { return PrintSettings.Fill; }
            set
            {
                PrintSettings.Fill = value;
                RaisePropertyChanged(() => Fill);
                InitItems();
            }
        }

        public PrintViewModel()
        {
            try
            {
                PrintSettings = ServiceProvider.Settings.DefaultSession.PrintSettings;
                PrintSetupCommand = new RelayCommand(PrintSetup);
                PageSetupCommand = new RelayCommand(PageSetup);
                PrintCommand = new RelayCommand(Print);
                if (!IsInDesignMode)
                {
                    Items = new ObservableCollection<PrintItemViewModel>();
                }
                LoadPrinterSettings();
                InitItems();
            }
            catch (Exception ex)
            {
                Log.Error("Error init PrintViewModel", ex);
            }
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
                Items.Add(new PrintItemViewModel {FileItem = file, Parent = this});
            }
        }

        private void PrintSetup()
        {

            if (Dlg.ShowDialog() == true)
            {
                LoadPrinterSettings();
            }
        }

        private void LoadPrinterSettings()
        {
            try
            {
                PrinterName = Dlg.PrintQueue.Name;
                PrintCapabilities capabilities = Dlg.PrintQueue.GetPrintCapabilities(Dlg.PrintTicket);
                if (capabilities.PageImageableArea != null)
                {
                    PageWidth = (int) capabilities.PageImageableArea.ExtentWidth;
                    PageHeight = (int) capabilities.PageImageableArea.ExtentHeight;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to load printer settings", exception);
            }
        }

        private void PageSetup()
        {

        }

    }
}
