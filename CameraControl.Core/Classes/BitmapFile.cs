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

using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Classes
{
    public class BitmapFile : BaseFieldClass
    {
        private PointCollection luminanceHistogramPoints = null;
        private PointCollection redColorHistogramPoints = null;
        private PointCollection greenColorHistogramPoints = null;
        private PointCollection blueColorHistogramPoints = null;

        public delegate void BitmapLoadedEventHandler(object sender);

        public virtual event BitmapLoadedEventHandler BitmapLoaded;

        private FileItem _fileItem;

        public FileItem FileItem
        {
            get { return _fileItem; }
            set
            {
                _fileItem = value;
            }
        }

        private bool _isLoaded;

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                _isLoaded = value;
                NotifyPropertyChanged("IsLoaded");
            }
        }


        public bool FullResLoaded { get; set; }

        private WriteableBitmap _displayImage;

        public WriteableBitmap DisplayImage
        {
            get { return _displayImage; }
            set
            {
                //if(_displayImage==null && FileItem!=null)
                //{
                //  _displayImage = FileItem.Thumbnail;
                //}
                _displayImage = value;
                NotifyPropertyChanged("DisplayImage");
            }
        }

        public BitmapSource Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                NotifyPropertyChanged("Preview");
            }
        }

        public PointCollection LuminanceHistogramPoints
        {
            get { return luminanceHistogramPoints; }
            set
            {
                luminanceHistogramPoints = value;
                NotifyPropertyChanged("LuminanceHistogramPoints");
            }
        }

        public PointCollection RedColorHistogramPoints
        {
            get { return redColorHistogramPoints; }
            set
            {
                redColorHistogramPoints = value;
                NotifyPropertyChanged("RedColorHistogramPoints");
            }
        }

        public PointCollection GreenColorHistogramPoints
        {
            get { return greenColorHistogramPoints; }
            set
            {
                greenColorHistogramPoints = value;
                NotifyPropertyChanged("GreenColorHistogramPoints");
            }
        }

        public PointCollection BlueColorHistogramPoints
        {
            get { return blueColorHistogramPoints; }
            set
            {
                blueColorHistogramPoints = value;
                NotifyPropertyChanged("BlueColorHistogramPoints");
            }
        }

        private bool _rawCodecNeeded;

        public bool RawCodecNeeded
        {
            get { return _rawCodecNeeded; }
            set
            {
                _rawCodecNeeded = value;
                NotifyPropertyChanged("RawCodecNeeded");
            }
        }

        private string _infoLabel;

        public string InfoLabel
        {
            get { return _infoLabel; }
            set
            {
                _infoLabel = value;
                NotifyPropertyChanged("InfoLabel");
            }
        }

        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                NotifyPropertyChanged("FileName");
            }
        }

        private string _comment;
        private BitmapSource _preview;

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                NotifyPropertyChanged("Comment");
            }
        }


        public AsyncObservableCollection<DictionaryItem> Metadata { get; set; }

        public void OnBitmapLoaded()
        {
            if (BitmapLoaded != null)
                BitmapLoaded(this);
        }

        /// <summary>
        /// Notifi UI about attached FileItem was changed
        /// </summary>
        public void Notify()
        {
            NotifyPropertyChanged("FileItem");
            NotifyPropertyChanged("LuminanceHistogramPoints");
            NotifyPropertyChanged("RedColorHistogramPoints");
        }

        public void SetFileItem(FileItem item)
        {
            FileItem = item;
            IsLoaded = false;
            FullResLoaded = false;
            if (DisplayImage == null)
                DisplayImage = new WriteableBitmap(FileItem.Thumbnail);
        }


        public BitmapFile()
        {
            IsLoaded = false;
            RawCodecNeeded = false;
            Metadata = new AsyncObservableCollection<DictionaryItem>();
            //Metadata.Add(new DictionaryItem() {Name = "Exposure mode"});
            //Metadata.Add(new DictionaryItem() {Name = "Exposure program"});
            //Metadata.Add(new DictionaryItem() {Name = "Exposure time"});
            //Metadata.Add(new DictionaryItem() {Name = "F number"});
            //Metadata.Add(new DictionaryItem() {Name = "Lens focal length"});
            //Metadata.Add(new DictionaryItem() {Name = "ISO speed rating"});
            //Metadata.Add(new DictionaryItem() {Name = "Metering mode"});
            //Metadata.Add(new DictionaryItem() {Name = "White balance"});
            //Metadata.Add(new DictionaryItem() {Name = "Exposure bias"});
        }
    }
}