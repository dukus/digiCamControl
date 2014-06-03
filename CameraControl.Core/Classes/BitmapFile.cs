using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Devices.Classes;

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
                NotifyPropertyChanged("FileItem");
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

        public PointCollection LuminanceHistogramPoints
        {
            get
            {
                return this.luminanceHistogramPoints;
            }
            set
            {
                if (this.luminanceHistogramPoints != value)
                {
                    this.luminanceHistogramPoints = value;
                    NotifyPropertyChanged("LuminanceHistogramPoints");
                }
            }
        }

        public PointCollection RedColorHistogramPoints
        {
            get
            {
                return this.redColorHistogramPoints;
            }
            set
            {
                if (this.redColorHistogramPoints != value)
                {
                    this.redColorHistogramPoints = value;
                    NotifyPropertyChanged("RedColorHistogramPoints");
                }
            }
        }

        public PointCollection GreenColorHistogramPoints
        {
            get
            {
                return this.greenColorHistogramPoints;
            }
            set
            {
                if (this.greenColorHistogramPoints != value)
                {
                    this.greenColorHistogramPoints = value;
                    NotifyPropertyChanged("GreenColorHistogramPoints");
                }
            }
        }

        public PointCollection BlueColorHistogramPoints
        {
            get
            {
                return this.blueColorHistogramPoints;
            }
            set
            {
                if (this.blueColorHistogramPoints != value)
                {
                    this.blueColorHistogramPoints = value;
                    NotifyPropertyChanged("BlueColorHistogramPoints");
                }
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


        public void SetFileItem(FileItem item)
        {
            FileItem = item;
            IsLoaded = false;
            FullResLoaded = false;
            //if (DisplayImage == null)
                DisplayImage = new WriteableBitmap(FileItem.Thumbnail);
        }


        public BitmapFile()
        {
            IsLoaded = false;
            RawCodecNeeded = false;
            Metadata = new AsyncObservableCollection<DictionaryItem>();
            Metadata.Add(new DictionaryItem() { Name = "Exposure mode" });
            Metadata.Add(new DictionaryItem() { Name = "Exposure program" });
            Metadata.Add(new DictionaryItem() { Name = "Exposure time" });
            Metadata.Add(new DictionaryItem() { Name = "F number" });
            Metadata.Add(new DictionaryItem() { Name = "Lens focal length" });
            Metadata.Add(new DictionaryItem() { Name = "ISO speed rating" });
            Metadata.Add(new DictionaryItem() { Name = "Metering mode" });
            Metadata.Add(new DictionaryItem() { Name = "White balance" });
            Metadata.Add(new DictionaryItem() { Name = "Exposure bias" });
        }
    }
}
