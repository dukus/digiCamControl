using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Printing;
using System.Windows.Forms;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PrintDialog = System.Windows.Controls.PrintDialog;

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
                RaisePropertyChanged(() => PageWidth);
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
                RaisePropertyChanged(() => Repeat);
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
                    InitItems();
                }
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
                    file = files[i%files.Count];
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
            try
            {
                if (Dlg.ShowDialog() == true)
                {
                    PrinterName = Dlg.PrintQueue.Name;
                    SavePrintTicket(Dlg);
                    LoadPrinterSettings();

                }

            }
            catch (Exception ex)
            {
                Log.Debug("Unable to show printer window ",ex);
                MessageBox.Show("Unable to show printer window "+ex.Message);
            }
        }

        public void LoadPrinterSettings()
        {
            try
            {
                Dlg.PrintQueue = new PrintQueue(new PrintServer(), PrinterName);
                LoadPrintTicket(Dlg);
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

        public void SavePrintTicket(PrintDialog dialog)
        {
            using (FileStream stream = new FileStream(Path.Combine(Settings.SessionFolder, ServiceProvider.Settings.DefaultSession.Name + "_printer.xml"), FileMode.Create))
            {
                dialog.PrintTicket.SaveTo(stream);
                stream.Close();
            }
        }

        public  void LoadPrintTicket(PrintDialog dialog)
        {
            string configFile = Path.Combine(Settings.SessionFolder, ServiceProvider.Settings.DefaultSession.Name + "_printer.xml");

            PrintTicket defaultTicket;

            try
            {
                defaultTicket = dialog.PrintQueue.UserPrintTicket ?? dialog.PrintQueue.DefaultPrintTicket;
            }
            catch (Exception e)
            {
                Log.Error("Unable to load printer settings.", e);
                return;
            }

            if (File.Exists(configFile))
            {
                try
                {
                    using (FileStream stream = new FileStream(configFile, FileMode.Open))
                    {
                        PrintTicket newTicket = new PrintTicket(stream);

                        System.Printing.ValidationResult result = dialog.PrintQueue.MergeAndValidatePrintTicket(defaultTicket, newTicket);
                        dialog.PrintTicket = result.ValidatedPrintTicket;
                        stream.Close();
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Unable to load printer settings.", e);
                    dialog.PrintTicket = defaultTicket;
                }
            }
            else
            {
                if (defaultTicket != null)
                    dialog.PrintTicket = defaultTicket;
            }
        }

    }
}
