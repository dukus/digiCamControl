using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.ToolPlugins
{
    public class ArduinoCommandViewModel : ViewModelBase
    {
        public List<ArduinoButton> Buttons { get; set; }

        public  RelayCommand<ArduinoButton> ExecuteCommand { get; set; }
        public ArduinoViewModel ArduinoViewModel { get; set; }



        public ArduinoCommandViewModel()
        {
            ExecuteCommand = new RelayCommand<ArduinoButton>(Execute);
            Buttons = new List<ArduinoButton>();
            for (int i = 0; i < 16; i++)
            {
                Buttons.Add(new ArduinoButton() {Title = "Button " + i, Visible = true});
            }
        }

        private void Execute(ArduinoButton obj)
        {
            
        }
    }
}
