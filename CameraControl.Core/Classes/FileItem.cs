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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using CameraControl.Core.Classes.Queue;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

#endregion

namespace CameraControl.Core.Classes
{
    public enum FileItemType
    {
        File,
        CameraObject,
        Missing
    }

    public class FileItem : BaseFieldClass, IDisposable
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
                    FileDate = File.GetCreationTime(_fileName);
                }
                NotifyPropertyChanged("FileName");
                NotifyPropertyChanged("ToolTip");
            }
        }

        public string BackupFileName
        {
            get { return _backupFileName; }
            set
            {
                _backupFileName = value;
                NotifyPropertyChanged("BackupFileName");
            }
        }

        public bool HaveBackupFile
        {
            get { return File.Exists(BackupFileName); }
        }

        [XmlAttribute]
        public string CameraSerial { get; set; }

        [XmlAttribute]
        public string OriginalName { get; set; }

        [XmlAttribute]
        public int Series { get; set; }

        public bool IsRaw
        {
            get
            {
                var extension = Path.GetExtension(FileName);
                return extension != null && (!string.IsNullOrEmpty(FileName) && (extension.ToLower() == ".nef" || extension.ToLower() == ".cr2"));
            }
        }

        [XmlAttribute]
        public DateTime FileDate { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public string ToolTip
        {
            get { return string.Format("File name: {0}\nFile date :{1}", FileName, FileDate); }
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

        public bool HaveGeneratedThumbnail
        {
            get { return File.Exists(SmallThumb) && File.Exists(LargeThumb); }
        }


        private BitmapImage _bitmapImage;

        [JsonIgnore]
        [XmlIgnore]
        public BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
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

        public AsyncObservableCollection<ValuePair> FileNameTemplates { get; set; }

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
            Name = FileName;
            FileDate = deviceObject.FileDate;
            IsChecked = true;
            IsLiked = false;
            IsUnLiked = false;
            ThumbData = deviceObject.ThumbData;
        }


        public FileItem(string file)
        {
            IsLoaded = false;
            FileName = file;
            Name = Path.GetFileName(file);
            ItemType = FileItemType.File;
            //FileInfo = new FileInfo();
            FileNameTemplates = new AsyncObservableCollection<ValuePair>();
        }

        public void SetFile(string file)
        {
            IsLoaded = false;
            FileName = file;
            Name = Path.GetFileName(file);
            ItemType = FileItemType.File;
            //FileInfo = new FileInfo();
        }

        public void AddTemplates(ICameraDevice device,PhotoSession session)
        {
            string[] skipItems =
            {
                "[Session Name]", "[Counter 3 digit]", "[Counter 4 digit]", "[Counter 5 digit]",
                "[Counter 6 digit]", "[Counter 7 digit]", "[Counter 8 digit]", "[Counter 9 digit]",
                "[Camera Counter 3 digit]","[Camera Counter 4 digit]","[Camera Counter 5 digit]","[Camera Counter 6 digit]",
                "[Camera Counter 7 digit]","[Camera Counter 8 digit]","[Camera Counter 9 digit]"
            };
            foreach (var template in ServiceProvider.FilenameTemplateManager.Templates)
            {
                if(skipItems.Contains(template.Key))
                    continue;
                var val = template.Value.Pharse(template.Key, session, device, FileName);
                if (!string.IsNullOrWhiteSpace(val))
                    FileNameTemplates.Add(new ValuePair() {Name = template.Key, Value = val});
            }
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
                if (_id == 0)
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


        /// <summary>
        /// Gets the small thumb file name.
        /// </summary>
        /// <value>
        /// The small thumb.
        /// </value>
        public string SmallThumb
        {
            get { return Path.Combine(Settings.DataFolder, "Cache\\Small", Id + ".jpg"); }
        }

        /// <summary>
        /// Gets the large thumb file name.
        /// </summary>
        /// <value>
        /// The large thumb.
        /// </value>
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
            try
            {
                if (File.Exists(SmallThumb))
                    File.Delete(SmallThumb);
                if (File.Exists(LargeThumb))
                    File.Delete(LargeThumb);
                if (File.Exists(InfoFile))
                    File.Delete(InfoFile);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to remove thumb data", ex);
            }
        }


        private BitmapSource _thumbnail;
        private string _backupFileName;

        [JsonIgnore]
        [XmlIgnore]
        public BitmapSource Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                {
                    if (ItemType == FileItemType.CameraObject)
                    {
                        if (ThumbData != null && ThumbData.Length > 4)
                        {
                            try
                            {
                                using (var stream = new MemoryStream(ThumbData, 0, ThumbData.Length))
                                {
                                    var image = new BitmapImage();
                                    image.BeginInit();
                                    // for better memory usage 
                                    image.DecodePixelWidth = 90;
                                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                                    image.CacheOption = BitmapCacheOption.OnLoad;
                                    image.UriSource = null;
                                    image.StreamSource = stream;
                                    image.EndInit();
                                    image.Freeze();
                                    Thumbnail = image;
                                }
                            }
                            catch (Exception exception)
                            {
                                Log.Debug("Error loading device thumb ", exception);
                            }
                        }
                        else
                        {
                           // _thumbnail = BitmapLoader.Instance.DefaultThumbnail;
                        }

                    }
                    else
                    {
                        _thumbnail = ItemType == FileItemType.Missing
                            ? BitmapLoader.Instance.NoImageThumbnail
                            : BitmapLoader.Instance.DefaultThumbnail;
                        if (!ServiceProvider.Settings.DontLoadThumbnails)
                            ServiceProvider.QueueManager.Add(new QueueItemFileItem { FileItem = this });
                    }
                }
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                NotifyPropertyChanged("Thumbnail");
            }
        }

        public byte[] ThumbData { get; set; }

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
                    Stream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    // or any stream
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
                FileInfo.ValidateValues();
                XmlSerializer serializer = new XmlSerializer(typeof(FileInfo));
                // Create a FileStream to write with.
                System.Text.Encoding code = Encoding.GetEncoding("UTF-8");
                StreamWriter writer = new StreamWriter(InfoFile, false, code);
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

        public void Dispose()
        {
            Thumbnail = null;
        }
    }
}