using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class LiveviewSettings : BaseFieldClass
    {
        private double _canvasWidt;

        [XmlIgnore]
        public double CanvasWidt
        {
            get { return _canvasWidt; }
            set
            {
                _canvasWidt = value;
                GridHorizontalMinSize = CanvasWidt * (_gridHorizontalMin) / 100;
                GridHorizontalMaxSize = CanvasWidt * (_gridHorizontalMax) / 100;
            }
        }

        private double _canvasHeight;

        [XmlIgnore]
        public double CanvasHeight
        {
            get { return _canvasHeight; }
            set
            {
                _canvasHeight = value;
                GridVerticalMaxSize = CanvasHeight * (100 - _gridVerticalMax) / 100;
                GridVerticalMinSize = CanvasHeight * (100 - _gridVerticalMin) / 100;
            }
        }

        [XmlIgnore]
        private double _gridVerticalMinSize;
        public double GridVerticalMinSize
        {
            get { return _gridVerticalMinSize; }
            set
            {
                _gridVerticalMinSize = value;
                NotifyPropertyChanged("GridVerticalMinSize");
            }
        }

        [XmlIgnore]
        private double _gridVerticalMaxSize;
        public double GridVerticalMaxSize
        {
            get { return _gridVerticalMaxSize; }
            set
            {
                _gridVerticalMaxSize = value;
                NotifyPropertyChanged("GridVerticalMaxSize");
            }
        }

        private double _gridVerticalMin;
        public double GridVerticalMin
        {
            get { return _gridVerticalMin; }
            set
            {
                _gridVerticalMin = value;
                GridVerticalMinSize = CanvasHeight * (100-_gridVerticalMin) / 100;
                NotifyPropertyChanged("GridVerticalMin");
            }
        }

        private double _gridVerticalMax;
        public double GridVerticalMax
        {
            get { return _gridVerticalMax; }
            set
            {
                _gridVerticalMax = value;
                GridVerticalMaxSize = CanvasHeight * (100 - _gridVerticalMax) / 100;
                NotifyPropertyChanged("GridVerticalMax");
            }
        }
        //------------------------------
        [XmlIgnore]
        private double _gridHorizontalMinSize;
        public double GridHorizontalMinSize
        {
            get { return _gridHorizontalMinSize; }
            set
            {
                _gridHorizontalMinSize = value;
                NotifyPropertyChanged("GridHorizontalMinSize");
            }
        }

        [XmlIgnore]
        private double _gridHorizontalMaxSize;
        public double GridHorizontalMaxSize
        {
            get { return _gridHorizontalMaxSize; }
            set
            {
                _gridHorizontalMaxSize = value;
                NotifyPropertyChanged("GridHorizontalMaxSize");
            }
        }

        private double _gridHorizontalMin;
        public double GridHorizontalMin
        {
            get { return _gridHorizontalMin; }
            set
            {
                _gridHorizontalMin = value;
                GridHorizontalMinSize = CanvasWidt * (_gridHorizontalMin) / 100;
                NotifyPropertyChanged("GridHorizontalMin");
            }
        }

        private double _gridHorizontalMax;
        public double GridHorizontalMax
        {
            get { return _gridHorizontalMax; }
            set
            {
                _gridHorizontalMax = value;
                GridHorizontalMaxSize = CanvasWidt * (_gridHorizontalMax) / 100;
                NotifyPropertyChanged("GridHorizontalMax");
            }
        }

        private bool _gridVisible;
        public bool GridVisible
        {
            get { return _gridVisible; }
            set
            {
                _gridVisible = value;
                NotifyPropertyChanged("GridVisible");
            }
        }

        public LiveviewSettings()
        {
            GridVisible = false;
        }
    }
}
