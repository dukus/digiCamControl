using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using CameraControl.Core.Classes.Queue;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using FreeImageAPI;
using Newtonsoft.Json;

namespace CameraControl.Core.Classes
{

    public enum FileItemType
    {
        File,
        CameraObject,
        Missing

    }

    public class FileItem : BaseFieldClass
    {

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                if (File.Exists(_fileName))
                {
                    FileDate = File.GetCreationTimeUtc(_fileName);
                }
                NotifyPropertyChanged("FileName");
                NotifyPropertyChanged("ToolTip");
            }
        }

        public bool IsRaw
        {
            get
            {
                var extension = Path.GetExtension(FileName);
                return extension != null && (!string.IsNullOrEmpty(FileName) && extension.ToLower() == ".nef");
            }
        }

        public DateTime FileDate { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public string ToolTip
        {
            get { return string.Format("File name: {0}\nFile date :{1}", FileName, FileDate.ToString()); }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        private bool _isLiked;
        public bool IsLiked
        {
            get { return _isLiked; }
            set
            {
                _isLiked = value;
                _isUnLiked = !(_isUnLiked && _isLiked) && _isUnLiked;
                NotifyPropertyChanged("IsLiked");
                NotifyPropertyChanged("IsUnLiked");
            }
        }

        private bool _isUnLiked;
        public bool IsUnLiked
        {
            get { return _isUnLiked; }
            set
            {
                _isUnLiked = value;
                _isLiked = !(_isUnLiked && _isLiked) && _isLiked;
                NotifyPropertyChanged("IsLiked");
                NotifyPropertyChanged("IsUnLiked");
            }
        }

        private BitmapImage _bitmapImage;
        [JsonIgnore]
        [XmlIgnore]
        public BitmapImage BitmapImage
        {
            get
            {
                return _bitmapImage;
            }
            set
            {
                if (value == null)
                {

                }
                _bitmapImage = value;
                NotifyPropertyChanged("BitmapImage");
            }
        }

        private DeviceObject _deviceObject;
        [JsonIgnore]
        [XmlIgnore]
        public DeviceObject DeviceObject
        {
            get { return _deviceObject; }
            set
            {
                _deviceObject = value;
                NotifyPropertyChanged("DeviceObject");
            }
        }

        private ICameraDevice _device;
        [JsonIgnore]
        [XmlIgnore]
        public ICameraDevice Device
        {
            get { return _device; }
            set
            {
                _device = value;
                NotifyPropertyChanged("Device");
            }
        }

        private FileItemType _itemType;
        [JsonIgnore]
        [XmlIgnore]
        public FileItemType ItemType
        {
            get { return _itemType; }
            set
            {
                _itemType = value;
                NotifyPropertyChanged("ItemType");
            }
        }

        public FileItem()
        {
            IsLoaded = false;
            //FileInfo = new FileInfo();
        }

        public FileItem(DeviceObject deviceObject, ICameraDevice device)
        {
            Device = device;
            DeviceObject = deviceObject;
            ItemType = FileItemType.CameraObject;
            FileName = deviceObject.FileName;
            FileDate = deviceObject.FileDate;
            IsChecked = true;
            IsLiked = false;
            IsUnLiked = false;
            if (deviceObject.ThumbData != null && deviceObject.ThumbData.Length > 4)
            {
                try
                {
                    var stream = new MemoryStream(deviceObject.ThumbData, 0, deviceObject.ThumbData.Length);

                    using (var bmp = new Bitmap(stream))
                    {
                        Thumbnail = BitmapSourceConvert.ToBitmapSource(bmp);
                    }
                    stream.Close();
                }
                catch (Exception exception)
                {
                    Log.Debug("Error loading device thumb ", exception);
                }
            }
        }


        public FileItem(string file)
        {
            IsLoaded = false;
            FileName = file;
            Name = Path.GetFileName(file);
            ItemType = FileItemType.File;
            //FileInfo = new FileInfo();
        }

        public FileItem(ICameraDevice device, DateTime time)
        {
            Device = device;
            ItemType = FileItemType.CameraObject;
            IsChecked = true;
            ItemType = FileItemType.Missing;
            FileName = "Missing";
            FileDate = time;
        }

        private int _id;
        public int Id
        {
            get
            {
                if(_id==0)
                {
                    if (!string.IsNullOrEmpty(FileName) && File.Exists(FileName))
                    {
                        _id = Math.Abs(FileName.GetHashCode() + File.GetCreationTimeUtc(FileName).GetHashCode());
                    }
                }
                return _id;
            }
            set { _id = value; }
        }

        [JsonIgnore]
        [XmlIgnore]
        public bool IsLoaded { get; set; }
        
        [JsonIgnore]
        [XmlIgnore]
        public FileInfo FileInfo { get; set; }


        public string SmallThumb
        {
            get { return Path.Combine(Settings.DataFolder, "Cache\\Small", Id + ".jpg"); }
        }

        public string LargeThumb
        {
            get { return Path.Combine(Settings.DataFolder, "Cache\\Large", Id + ".jpg"); }
        }

        public string InfoFile
        {
            get { return Path.Combine(Settings.DataFolder, "Cache\\InfoFile", Id + ".xml"); }
        }
        
        public void RemoveThumbs()
        {
            if (File.Exists(SmallThumb))
                File.Delete(SmallThumb);
            if (File.Exists(LargeThumb))
                File.Delete(LargeThumb);
            if (File.Exists(InfoFile))
                File.Delete(InfoFile);
        }


        private BitmapSource _thumbnail;
        [JsonIgnore]
        [XmlIgnore]
        public BitmapSource Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                {
                    _thumbnail = ItemType == FileItemType.Missing
                                     ? BitmapLoader.Instance.NoImageThumbnail
                                     : BitmapLoader.Instance.DefaultThumbnail;
                    if (!ServiceProvider.Settings.DontLoadThumbnails)
                        ServiceProvider.QueueManager.Add(new QueueItemFileItem { FileItem = this });
                }
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                NotifyPropertyChanged("Thumbnail");
            }
        }

        public void GetExtendedThumb()
        {
            if (ItemType != FileItemType.File)
                return;
            try
            {
                if (IsRaw)
                {
                    try
                    {
                        BitmapDecoder bmpDec = BitmapDecoder.Create(new Uri(FileName),
                                                                    BitmapCreateOptions.None,
                                                                    BitmapCacheOption.Default);
                        if (bmpDec.Thumbnail != null)
                        {
                            WriteableBitmap bitmap = new WriteableBitmap(bmpDec.Thumbnail);
                            bitmap.Freeze();
                            Thumbnail = bitmap;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    Image.GetThumbnailImageAbort myCallback = ThumbnailCallback;
                    Stream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); // or any stream
                    Image tempImage = Image.FromStream(fs, true, false);

                    Thumbnail =
                      BitmapSourceConvert.ToBitmapSource(
                        (Bitmap)tempImage.GetThumbnailImage(160, 120, myCallback, IntPtr.Zero));
                    tempImage.Dispose();
                    fs.Close();
                }
            }
            catch (Exception exception)
            {
                Log.Debug("Unable load thumbnail: " + FileName, exception);
            }
        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        public void SaveInfo()
        {
            if (FileInfo == null)
                return;
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(InfoFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(InfoFile));

                XmlSerializer serializer = new XmlSerializer(typeof(FileInfo));
                // Create a FileStream to write with.

                Stream writer = new FileStream(InfoFile, FileMode.Create);
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, FileInfo);
                writer.Close();
            }
            catch (Exception)
            {
                Log.Error("Unable to save session branding file");
            }
        }

        public void LoadInfo()
        {
            try
            {
                
                if (File.Exists(InfoFile))
                {
                    XmlSerializer mySerializer =
                      new XmlSerializer(typeof(FileInfo));
                    FileStream myFileStream = new FileStream(InfoFile, FileMode.Open);
                    FileInfo = (FileInfo)mySerializer.Deserialize(myFileStream);
                    myFileStream.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

    }
}
