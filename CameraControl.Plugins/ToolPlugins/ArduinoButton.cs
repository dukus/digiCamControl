using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CameraControl.Devices.Classes;

namespace CameraControl.Plugins.ToolPlugins
{
    public class ArduinoButton:BaseFieldClass
    {
        private bool _visible;
        private string _title;
        private string _command;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                NotifyPropertyChanged("Visible");
                NotifyPropertyChanged("Visibility");
            }
        }

        public Visibility Visibility
        {
            get { return Visible ? Visibility.Visible : Visibility.Collapsed; }
        }


        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public string Command
        {
            get { return _command; }
            set
            {
                _command = value;
                NotifyPropertyChanged("Command");
            }
        }
    }
}
