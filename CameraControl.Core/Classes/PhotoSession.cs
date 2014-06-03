using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

namespace CameraControl.Core.Classes
{
    public class PhotoSession : BaseFieldClass
    {
        private object _locker = new object();
        private string _lastFilename = null;
        
        [JsonIgnore]
        [XmlIgnore]
        public List<string> SupportedExtensions = new List<string> { ".jpg", ".nef", ".tif", ".png", ".cr2" };
        [JsonIgnore]
        [XmlIgnore]
        public List<string> RawExtensions = new List<string> { ".cr2", ".nef" };

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private bool _alowFolderChange;
        public bool AlowFolderChange
        {
            get { return _alowFolderChange; }
            set
            {
                _alowFolderChange = value;
                NotifyPropertyChanged("AlowFolderChange");
            }
        }


        private string _folder;
        public string Folder
        {
            get { return _folder; }
            set
            {
                if (_folder != value)
                {
                    if (!Directory.Exists(value))
                    {
                        try
                        {
                            Directory.CreateDirectory(value);
                        }
                        catch (Exception exception)
                        {
                            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Name);
                            if (value != folder)
                                value = folder;
                            Log.Error("Error creating session folder", exception);
                        }
                    }
                    _systemWatcher.Path = value;
                    _systemWatcher.EnableRaisingEvents = true;
                    _systemWatcher.IncludeSubdirectories = true;
                }
                _folder = value;
                NotifyPropertyChanged("Folder");
            }
        }

        private string _fileNameTemplate;

        public string FileNameTemplate
        {
            get { return _fileNameTemplate; }
            set
            {
                _fileNameTemplate = value;
                NotifyPropertyChanged("FileNameTemplate");
            }
        }

        private int _counter;
        public int Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                NotifyPropertyChanged("Counter");
            }
        }

        private bool _useOriginalFilename;
        public bool UseOriginalFilename
        {
            get { return _useOriginalFilename; }
            set
            {
                _useOriginalFilename = value;
                NotifyPropertyChanged("UseOriginalFilename");
            }
        }


        private AsyncObservableCollection<FileItem> _files;

        public AsyncObservableCollection<FileItem> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                NotifyPropertyChanged("Files");
            }
        }

        private TimeLapseClass _timeLapse;
        public TimeLapseClass TimeLapse
        {
            get { return _timeLapse; }
            set
            {
                _timeLapse = value;
                NotifyPropertyChanged("TimeLapse");
            }
        }

        private BraketingClass _braketing;
        public BraketingClass Braketing
        {
            get { return _braketing; }
            set
            {
                _braketing = value;
                NotifyPropertyChanged("Braketing");
            }
        }

        private AsyncObservableCollection<TagItem> _tags;
        public AsyncObservableCollection<TagItem> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                NotifyPropertyChanged("Tags");
            }
        }

        private TagItem _selectedTag1;
        public TagItem SelectedTag1
        {
            get { return _selectedTag1; }
            set
            {
                _selectedTag1 = value;
                NotifyPropertyChanged("SelectedTag1");
            }
        }

        private TagItem _selectedTag2;
        public TagItem SelectedTag2
        {
            get { return _selectedTag2; }
            set
            {
                _selectedTag2 = value;
                NotifyPropertyChanged("SelectedTag2");
            }
        }

        private TagItem _selectedTag3;
        public TagItem SelectedTag3
        {
            get { return _selectedTag3; }
            set
            {
                _selectedTag3 = value;
                NotifyPropertyChanged("SelectedTag3");
            }
        }

        private TagItem _selectedTag4;
        public TagItem SelectedTag4
        {
            get { return _selectedTag4; }
            set
            {
                _selectedTag4 = value;
                NotifyPropertyChanged("SelectedTag4");
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

        private string _barcode;
        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                NotifyPropertyChanged("Barcode");
            }
        }


        private bool _writeComment;
        public bool WriteComment
        {
            get { return _writeComment; }
            set
            {
                _writeComment = value;
                NotifyPropertyChanged("WriteComment");
            }
        }

        private bool _useCameraCounter;
        public bool UseCameraCounter
        {
            get { return _useCameraCounter; }
            set
            {
                _useCameraCounter = value;
                NotifyPropertyChanged("UseCameraCounter");
            }
        }

        private bool _downloadOnlyJpg;
        public bool DownloadOnlyJpg
        {
            get { return _downloadOnlyJpg; }
            set
            {
                _downloadOnlyJpg = value;
                NotifyPropertyChanged("DownloadOnlyJpg");
            }
        }

        private int _leadingZeros;
        public int LeadingZeros
        {
            get { return _leadingZeros; }
            set
            {
                _leadingZeros = value;
                NotifyPropertyChanged("LeadingZeros");
            }
        }

        private string _configFile;
        public string ConfigFile
        {
            get
            {
                return _configFile;
            }
            set { _configFile = value; }
        }

        private FileSystemWatcher _systemWatcher;

        public PhotoSession()
        {
            _systemWatcher = new FileSystemWatcher();
            _systemWatcher.EnableRaisingEvents = false;
            //_systemWatcher.Deleted += _systemWatcher_Deleted;
            //_systemWatcher.Created += new FileSystemEventHandler(_systemWatcher_Created);

            Name = "Default";
            Braketing = new BraketingClass();
            try
            {
                Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Name);
            }
            catch (Exception exception)
            {
                Log.Error("Error set My pictures folder", exception);
                Folder = "c:\\";
            }
            Files = new AsyncObservableCollection<FileItem>();
            FileNameTemplate = "DSC_$C";
            TimeLapse = new TimeLapseClass();
            if (ServiceProvider.Settings != null && ServiceProvider.Settings.VideoTypes.Count > 0)
                TimeLapse.VideoType = ServiceProvider.Settings.VideoTypes[0];
            try
            {
                TimeLapse.OutputFIleName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                                                        Name + ".avi");
            }
            catch (Exception exception)
            {
                Log.Error("Error set My videos folder", exception);
                TimeLapse.OutputFIleName = "c:\\";
            }

            UseOriginalFilename = false;
            AlowFolderChange = false;
            Tags = new AsyncObservableCollection<TagItem>();
            UseCameraCounter = false;
            DownloadOnlyJpg = false;
            LeadingZeros = 4;
            WriteComment = false;
            AllowOverWrite = false;
        }

        void _systemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                //AddFile(e.FullPath);
            }
            catch (Exception exception)
            {
                Log.Error("Add file error", exception);
            }
        }

        void _systemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            FileItem deletedItem = null;
            lock (this)
            {
                foreach (FileItem fileItem in Files)
                {
                    if (fileItem.FileName == e.FullPath)
                        deletedItem = fileItem;
                }
            }
            try
            {
                if (deletedItem != null)
                    Files.Remove(deletedItem);
                //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Files.Remove(
                //  deletedItem)));
            }
            catch (Exception)
            {


            }
        }

        private bool _allowOverWrite;
        public bool AllowOverWrite
        {
            get { return _allowOverWrite; }
            set
            {
                _allowOverWrite = value;
                NotifyPropertyChanged("AllowOverWrite");
            }
        }

        public string GetNextFileName(string ext, ICameraDevice device)
        {
            lock (_locker)
            {
                if (string.IsNullOrEmpty(ext))
                    ext = ".nef";
                if (!string.IsNullOrEmpty(_lastFilename) && RawExtensions.Contains(ext.ToLower()) && !RawExtensions.Contains(Path.GetExtension(_lastFilename).ToLower()))
                {
                    string rawfile = Path.Combine(Folder,
                                                  FormatFileName(device, ext,false) + (!ext.StartsWith(".") ? "." : "") + ext);
                    if (!File.Exists(rawfile))
                        return rawfile;
                }
                    
                string fileName = Path.Combine(Folder,
                                               FormatFileName(device, ext) + (!ext.StartsWith(".") ? "." : "") + ext);
                
                if (File.Exists(fileName) && !AllowOverWrite)
                    return GetNextFileName(ext, device);
                _lastFilename = fileName;
                return fileName;
            }
        }

        private string FormatFileName(ICameraDevice device, string ext, bool incremetCounter=true)
        {
            CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(device);
            string res = FileNameTemplate;
            if (!res.Contains("$C") && !AllowOverWrite)
                res += "$C";

            if (UseCameraCounter)
            {
                if (incremetCounter)
                    property.Counter = property.Counter + property.CounterInc;
                res = res.Replace("$C", property.Counter.ToString(new string('0', LeadingZeros)));
            }
            else
            {
                if (incremetCounter)
                    Counter++;
                res = res.Replace("$C", Counter.ToString(new string('0', LeadingZeros)));
            }
            res = res.Replace("$N", Name.Trim());
            if (device.ExposureCompensation != null)
                res = res.Replace("$E", device.ExposureCompensation.Value != "0" ? device.ExposureCompensation.Value : "");
            res = res.Replace("$D", DateTime.Now.ToString("yyyy-MM-dd"));
            res = res.Replace("$B", Barcode ?? "");

            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTime.Now.Kind);
            var unixTimestamp = System.Convert.ToInt64((DateTime.Now - date).TotalSeconds);
            res = res.Replace("$UTime", unixTimestamp.ToString());
            
            res = res.Replace("$Type", GetType(ext));

            res = res.Replace("$X", property.DeviceName.Replace(":", "_").Replace("?", "_").Replace("*", "_"));
            res = res.Replace("$Tag1", SelectedTag1 != null ? SelectedTag1.Value.Trim() : "");
            res = res.Replace("$Tag2", SelectedTag1 != null ? SelectedTag2.Value.Trim() : "");
            res = res.Replace("$Tag3", SelectedTag1 != null ? SelectedTag3.Value.Trim() : "");
            res = res.Replace("$Tag4", SelectedTag1 != null ? SelectedTag4.Value.Trim() : "");
            //prevent multiple \ if a tag is empty 
            while (res.Contains(@"\\"))
            {
                res = res.Replace(@"\\", @"\");
            }
            // if the file name start with \ the Path.Combine isn't work right 
            if (res.StartsWith("\\"))
                res = res.Substring(1);
            return res;
        }

        private string GetType(string ext)
        {
            if (ext.StartsWith("."))
                ext = ext.Substring(1);
            switch (ext.ToLower())
            {
                case "jpg":
                    return "Jpg";
                case "nef":
                    return "Raw";
                case "cr2":
                    return "Raw";
                case "tif":
                    return "Tif";
            }
            return ext;
        }

        public FileItem AddFile(string fileName)
        {
            FileItem oitem = GetFile(fileName);
            if (oitem != null)
                return oitem;
            FileItem item = new FileItem(fileName);
            Files.Add(item);
            return item;
        }

        public bool ContainFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;
            return Files.Any(fileItem => fileItem.FileName.ToUpper() == fileName.ToUpper());
        }

        public FileItem GetFile(string fileName)
        {
            foreach (FileItem fileItem in Files)
            {
                if (fileItem.FileName.ToUpper() == fileName.ToUpper())
                    return fileItem;
            }
            return null;
        }

        public override string ToString()
        {
            return Name;
        }

        public AsyncObservableCollection<FileItem> GetSelectedFiles()
        {
            AsyncObservableCollection<FileItem> list = new AsyncObservableCollection<FileItem>();
            foreach (FileItem fileItem in Files)
            {
                if (fileItem.IsChecked)
                    list.Add(fileItem);
            }
            return list;
        }

        public void SelectAll()
        {
            foreach (FileItem fileItem in Files)
            {
                fileItem.IsChecked = true;
            }
        }

        public void SelectNone()
        {
            foreach (FileItem fileItem in Files)
            {
                fileItem.IsChecked = false;
            }
        }

        public void SelectLiked()
        {
            foreach (FileItem fileItem in Files)
            {
                fileItem.IsChecked = fileItem.IsLiked;
            }
        }

        public void SelectUnLiked()
        {
            foreach (FileItem fileItem in Files)
            {
                fileItem.IsChecked = fileItem.IsUnLiked;
            }
        }

        public void SelectInver()
        {
            foreach (FileItem fileItem in Files)
            {
                fileItem.IsChecked = !fileItem.IsChecked;
            }
        }
    }
}
