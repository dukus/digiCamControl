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
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

#endregion

namespace CameraControl.Core.Classes
{
    public class LiveviewSettings : BaseFieldClass
    {
        private double _canvasWidt;

        [XmlIgnore]
        [JsonIgnore]
        public double CanvasWidt
        {
            get { return _canvasWidt; }
            set
            {
                _canvasWidt = value;
                GridHorizontalMinSize = CanvasWidt*(_gridHorizontalMin)/100;
                GridHorizontalMaxSize = CanvasWidt*(_gridHorizontalMax)/100;
            }
        }

        private double _canvasHeight;

        [XmlIgnore]
        [JsonIgnore]
        public double CanvasHeight
        {
            get { return _canvasHeight; }
            set
            {
                _canvasHeight = value;
                GridVerticalMaxSize = CanvasHeight*(100 - _gridVerticalMax)/100;
                GridVerticalMinSize = CanvasHeight*(100 - _gridVerticalMin)/100;
            }
        }

        [XmlIgnore]
        [JsonIgnore]
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
        [JsonIgnore]
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
                GridVerticalMinSize = CanvasHeight*(100 - _gridVerticalMin)/100;
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
                GridVerticalMaxSize = CanvasHeight*(100 - _gridVerticalMax)/100;
                NotifyPropertyChanged("GridVerticalMax");
            }
        }

        //------------------------------
        [XmlIgnore]
        [JsonIgnore]
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
        [JsonIgnore]
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
                GridHorizontalMinSize = CanvasWidt*(_gridHorizontalMin)/100;
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
                GridHorizontalMaxSize = CanvasWidt*(_gridHorizontalMax)/100;
                NotifyPropertyChanged("GridHorizontalMax");
            }
        }


        private bool _gridVisible;
        private int _snapshotCaptureCount;
        private int _snapshotTimeCapture;
        private Color _gridColor;

        public bool GridVisible
        {
            get { return _gridVisible; }
            set
            {
                _gridVisible = value;
                NotifyPropertyChanged("GridVisible");
            }
        }

        public int SnapshotCaptureCount
        {
            get { return _snapshotCaptureCount; }
            set
            {
                _snapshotCaptureCount = value;
                NotifyPropertyChanged("SnapshotCaptureCount");
            }
        }

        public int SnapshotCaptureTime
        {
            get { return _snapshotTimeCapture; }
            set
            {
                _snapshotTimeCapture = value;
                NotifyPropertyChanged(nameof(SnapshotCaptureTime));
            }
        }

        public Color GridColor
        {
            get { return _gridColor; }
            set
            {
                _gridColor = value;
                NotifyPropertyChanged(nameof(GridColor));
            }
        }


        public string SelectedOverlay { get; set; }
        public bool BlackAndWhite { get; set; }
        public bool Invert { get; set; }
        public bool EdgeDetection { get; set; }
        public bool HighlightOverExp { get; set; }
        public bool HighlightUnderExp { get; set; }
        public int RotationIndex { get; set; }
        public int MotionThreshold { get; set; }
        public int WaitForMotionSec { get; set; }
        public bool MotionAutofocusBeforCapture { get; set; }
        public bool DetectMotion { get; set; }
        public bool DetectMotionArea { get; set; }
        public int MotionAction { get; set; }
        public int MotionMovieLength { get; set; }

        public bool ShowFocusRect { get; set; }
        public bool ShowLeftTab { get; set; }
        public bool NoProcessing { get; set; }
        public int Brightness { get; set; }

        public int HorizontalMin { get; set; }
        public int HorizontalMax { get; set; }
        public int VerticalMin { get; set; }
        public int VerticalMax { get; set; }
        public bool ShowRuler { get; set; }
        public bool FlipImage { get; set; }
        public int PreviewTime { get; set; }
        public int CropRatio { get; set; }
        public int CaptureCount { get; set; }
        public int CaptureDelay { get; set; }

        public LiveviewSettings()
        {
            GridVisible = false;
            BlackAndWhite = false;
            EdgeDetection = false;
            HighlightOverExp = false;
            HighlightUnderExp = false;
            MotionThreshold = 20;
            ShowFocusRect = true;
            ShowLeftTab = true;
            NoProcessing = false;

            HorizontalMin = 0;
            HorizontalMax = 100;
            VerticalMin = 0;
            VerticalMax = 100;
            FlipImage = false;
            CropRatio = 0;
            MotionMovieLength = 30;

            CaptureCount = 1;

            SnapshotCaptureCount = 1;
            SnapshotCaptureTime = 500;
            GridColor = Colors.Gray;
        }


    }
}