using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class PrintPluginViewModel : BasePluginViewModel
    {
        public PrintDialog dlg = new PrintDialog();

        public RelayCommand ConfigureCommand { get; set; }

        public bool Rotate
        {
            get { return _config.ConfigData["Rotate"]=="True"; }
            set
            {
                _config.ConfigData["Rotate"] = value.ToString();
                RaisePropertyChanged(() => Rotate);
            }
        }

        public int Margin
        {
            get { return ToInt("Margin"); }
            set
            {
                _config.ConfigData["Margin"] = value.ToString();
                RaisePropertyChanged(() => Margin);
            }
        }

        public PrintPluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
        }

        public PrintPluginViewModel()
        {
            
        }

        public void Configure()
        {
            try
            {
                dlg.ShowDialog();
            }
            catch (Exception exception)
            {
                Log.Error("Error config printer", exception);
            }
        }
    }
}
