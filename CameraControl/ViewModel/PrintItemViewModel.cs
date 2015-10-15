using System.Windows.Media.Imaging;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.ViewModel
{
    public class PrintItemViewModel : ViewModelBase
    {
        private string _image;
        private int _angle;
        private PrintViewModel _parent;
        private FileItem _fileItem;
        private BitmapSource _bitmapSource;
        private string _fill;

        public PrintViewModel Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (_parent.Rotate)
                    Angle = 90;
                Fill = _parent.Fill ? "UniformToFill" : "Uniform";
            }
        }

        public FileItem FileItem
        {
            get
            {
                return _fileItem;
            }
            set
            {
                _fileItem = value;
                ImageFile = _fileItem != null ? _fileItem.FileName : "";
            }
        }

        public string ImageFile
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged(()=>ImageFile);
            }
        }

        public BitmapSource BitmapSource
        {
            get
            {
                if (_bitmapSource == null)
                {
                    if (FileItem != null)
                    {
                        _bitmapSource = BitmapLoader.Instance.LoadImage(FileItem, false, false);
                    }
                }
                return _bitmapSource;
            }
            set { _bitmapSource = value; }
        }

        public string Fill
        {
            get { return _fill; }
            set
            {
                _fill = value;
                RaisePropertyChanged(() => Fill);
            }
        }

        public int Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                RaisePropertyChanged(()=>Angle);
            }
        }
    }
}
