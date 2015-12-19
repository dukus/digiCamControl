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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

#endregion

namespace CameraControl.Core.Classes
{
    public class PhotoSession : BaseFieldClass
    {
        private object _locker = new object();
        private string _lastFilename = null;

        [JsonIgnore] [XmlIgnore] public List<string> SupportedExtensions = new List<string>
        {
            ".jpg",
            ".nef",
            ".tif",
            ".png",
            ".bmp",
            ".cr2",
            ".mov",
            ".avi"
        };

        [JsonIgnore]
        [XmlIgnore]
        public List<string> RawExtensions = new List<string> { ".cr2", ".nef" };

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {

                if (!AlowFolderChange && Path.GetDirectoryName(Folder) != null && Path.GetFileName(Folder) == _name)
                {
                    if (Folder != null) Folder = Path.Combine(Path.GetDirectoryName(Folder), value);
                }
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string CaptureName
        {
            get { return _captureName; }
            set
            {
                _captureName = value;
                NotifyPropertyChanged("CaptureName");
            }
        }

        public int Series
        {
            get { return _series; }
            set
            {
                _series = value;
                NotifyPropertyChanged("Series");
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

        public bool ReloadOnFolderChange
        {
            get { return _reloadOnFolderChange; }
            set
            {
                _reloadOnFolderChange = value;
                NotifyPropertyChanged("ReloadOnFolderChange");
            }
        }

        private string _folder;

        public string Folder
        {
            get { return _folder; }
            set
            {
                //if (_folder != value)
                //{
                    
                //    if (!Directory.Exists(value))
                //    {
                //        try
                //        {
                //            Directory.CreateDirectory(value);
                //        }
                //        catch (Exception exception)
                //        {
                //            string folder = Path.Combine(
                //                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Name);
                //            if (value != folder)
                //                value = folder;
                //            Log.Error("Error creating session folder", exception);
                //        }
                //    }
                //    _systemWatcher.Path = value;
                //    _systemWatcher.EnableRaisingEvents = true;
                //    _systemWatcher.IncludeSubdirectories = true;
                //}
                if (AlowFolderChange && ReloadOnFolderChange && _folder != value)
                {
                    _folder = value;
                    ServiceProvider.QueueManager.Clear();
                    Files.Clear();
                    ServiceProvider.Settings.LoadData(this);
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

        public string BackUpPath
        {
            get { return _backUpPath; }
            set
            {
                _backUpPath = value;
                NotifyPropertyChanged("BackUpPath");
            }
        }

        public bool BackUp
        {
            get { return _backUp; }
            set
            {
                _backUp = value;
                NotifyPropertyChanged("BackUp");
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

        private BracketingClass _braketing;

        public BracketingClass Braketing
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

        public string Barcode { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ExternalData ExternalData { get; set; }


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

        public bool LowerCaseExtension
        {
            get { return _lowerCaseExtension; }
            set
            {
                _lowerCaseExtension = value;
                NotifyPropertyChanged("LowerCaseExtension");
            }
        }

        public bool AskSavePath
        {
            get { return _askSavePath; }
            set
            {
                _askSavePath = value;
                NotifyPropertyChanged("AskSavePath");
            }
        }

        private string _configFile;

        public string ConfigFile
        {
            get { return _configFile; }
            set { _configFile = value; }
        }

        public TimeLapseSettings TimeLapseSettings { get; set; }
        public PrintSettings PrintSettings { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public RelayCommand<AutoExportPluginConfig> RemovePluginCommand { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public RelayCommand<AutoExportPluginConfig> ApplyPluginCommand { get; set; }

        public PhotoSession()
        {
            var systemWatcher = new FileSystemWatcher();
            systemWatcher.EnableRaisingEvents = false;
            //_systemWatcher.Deleted += _systemWatcher_Deleted;
            //_systemWatcher.Created += new FileSystemEventHandler(_systemWatcher_Created);
            RemovePluginCommand = new RelayCommand<AutoExportPluginConfig>(RemovePlugin);
            ApplyPluginCommand = new RelayCommand<AutoExportPluginConfig>(ApplyPlugin);

            Name = "Session1";
            CaptureName = "Capture";
            Braketing = new BracketingClass();
            try
            {
                Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),"digiCamControl", Name);
            }
            catch (Exception exception)
            {
                Log.Error("Error set My pictures folder", exception);
                Folder = "c:\\";
            }
            Files = new AsyncObservableCollection<FileItem>();
            FileNameTemplate = "DSC_[Counter 4 digit]";

            UseOriginalFilename = false;
            AlowFolderChange = false;
            Tags = new AsyncObservableCollection<TagItem>();
            UseCameraCounter = false;
            DownloadOnlyJpg = false;
            LeadingZeros = 4;
            WriteComment = false;
            AllowOverWrite = false;
            LowerCaseExtension = true;
            AutoExportPluginConfigs = new AsyncObservableCollection<AutoExportPluginConfig>();
            TimeLapseSettings = new TimeLapseSettings();
            PrintSettings = new PrintSettings();
        }


        public AutoExportPluginConfig AddPlugin(IAutoExportPlugin plugin)
        {
            var res = new AutoExportPluginConfig(plugin);
            AutoExportPluginConfigs.Add(res);
            return res;
        }

        private void RemovePlugin(AutoExportPluginConfig plugin)
        {
            AutoExportPluginConfigs.Remove(plugin);
        }

        public static void ApplyPlugin(AutoExportPluginConfig plugin)
        {
            plugin.IsError = true;
            plugin.Error = "";
            plugin.IsRedy = true;
            var pl = ServiceProvider.PluginManager.GetAutoExportPlugin(plugin.Type);
            try
            {
                pl.Execute(ServiceProvider.Settings.SelectedBitmap.FileItem, plugin);
                ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Refresh_Image);
            }
            catch (Exception ex)
            {
                plugin.IsError = true;
                plugin.Error = ex.Message;
                plugin.IsRedy = true;
                Log.Error("Error to apply plugin", ex);
            }
        }

        private void _systemWatcher_Created(object sender, FileSystemEventArgs e)
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

        private void _systemWatcher_Deleted(object sender, FileSystemEventArgs e)
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
        private string _backUpPath;
        private bool _backUp;
        private string _captureName;
        private int _series;
        private bool _lowerCaseExtension;
        private bool _reloadOnFolderChange;
        private bool _askSavePath;

        public bool AllowOverWrite
        {
            get { return _allowOverWrite; }
            set
            {
                _allowOverWrite = value;
                NotifyPropertyChanged("AllowOverWrite");
            }
        }

        public string GetNextFileName(string file, ICameraDevice device)
        {
            lock (_locker)
            {
                var ext = Path.GetExtension(file);

                if (string.IsNullOrEmpty(ext))
                    ext = ".nef";
                if (!string.IsNullOrEmpty(_lastFilename) && RawExtensions.Contains(ext.ToLower()) &&
                    !RawExtensions.Contains(Path.GetExtension(_lastFilename).ToLower()))
                {
                    string rawfile = Path.Combine(Folder,
                                                  FormatFileName(device, ext, false) + (!ext.StartsWith(".") ? "." : "") +
                                                  ext);
                    if (!File.Exists(rawfile))
                        return rawfile;
                }

                string fileName = Path.Combine(Folder,
                                               FormatFileName(device, file) + (!ext.StartsWith(".") ? "." : "") + ext);

                if (File.Exists(fileName) && !AllowOverWrite)
                {
                    // the template should contain a counter type tag
                    if (!FileNameTemplate.Contains("[Counter") && !AllowOverWrite)
                        FileNameTemplate += "[Counter 4 digit]";
                    return GetNextFileName(file, device);
                }
                _lastFilename = fileName;
                return fileName;
            }
        }

        private string FormatFileName(ICameraDevice device, string file, bool incremetCounter = true)
        {
            CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(device);
            string res = FileNameTemplate;

            if (incremetCounter)
            {
                property.Counter = property.Counter + property.CounterInc;
                Counter++;
            }

            Regex regPattern = new Regex(@"\[(.*?)\]", RegexOptions.Singleline);
            MatchCollection matchX = regPattern.Matches(res);
            foreach (Match match in matchX)
            {
                if (ServiceProvider.FilenameTemplateManager.Templates.ContainsKey(match.Value))
                {
                    res = res.Replace(match.Value,
                        ServiceProvider.FilenameTemplateManager.Templates[match.Value].Pharse(match.Value, this, device,
                            file));
                }
            }

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


        public FileItem AddFile(string fileName)
        {
            lock (_locker)
            {
                FileItem oitem = GetFile(fileName);
                if (oitem != null)
                    return oitem;
                FileItem item = new FileItem(fileName);
                Files.Add(item);
                return item;
            }
        }

        public FileItem Add(FileItem item)
        {
            lock (_locker)
            {
                Files.Add(item);
                return item;
            }
        }

        /// <summary>
        /// Will return a new file item based on file name parameter
        /// If file already added then will retun that file item
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public FileItem GetNewFileItem(string fileName)
        {
            lock (_locker)
            {
                FileItem oitem = GetFile(fileName);
                if (oitem != null)
                    return oitem;
                FileItem item = new FileItem(fileName);
                return item;
            }
        }

        public bool ContainFile(string fileName)
        {
            lock (_locker)
            {
                return !string.IsNullOrEmpty(fileName) && Files.Any(fileItem => fileItem.FileName.ToUpper() == fileName.ToUpper());                
            }
        }

        public FileItem GetFile(string fileName)
        {
            lock (_locker)
            {
                if (string.IsNullOrEmpty(fileName))
                    return null;
                return
                    Files.FirstOrDefault(
                        fileItem =>
                            fileItem != null && !string.IsNullOrEmpty(fileItem.FileName) &&
                            String.Equals(fileItem.FileName, fileName, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public FileItem GetByName(string name)
        {
            lock (_locker)
            {
                if (string.IsNullOrEmpty(name))
                    return null;
                return Files.FirstOrDefault(fileItem => !string.IsNullOrEmpty(fileItem.FileName) && String.Equals(fileItem.Name, name, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        /// <summary>
        /// Gets a file item by the original name and  captured camera serial.
        /// </summary>
        /// <param name="fileName">Original file name.</param>
        /// <param name="serial">The camera serial number serial.</param>
        /// <returns></returns>
        public FileItem GetFile(string fileName, string serial)
        {
            lock (_locker)
            {
                return Files.FirstOrDefault(fileItem => fileItem.OriginalName == fileName && fileItem.CameraSerial == serial);                
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public AsyncObservableCollection<FileItem> GetSelectedFiles()
        {
            lock (_locker)
            {
                AsyncObservableCollection<FileItem> list = new AsyncObservableCollection<FileItem>();
                foreach (FileItem fileItem in Files)
                {
                    if (fileItem.IsChecked)
                        list.Add(fileItem);
                }
                return list;
            }
        }

        public void SelectAll()
        {
            lock (_locker)
            {
                foreach (FileItem fileItem in Files)
                {
                    fileItem.IsChecked = true;
                }
            }
        }

        public void SelectNone()
        {
            lock (_locker)
            {
                foreach (FileItem fileItem in Files)
                {
                    fileItem.IsChecked = false;
                }
            }
        }

        public void SelectLiked()
        {
            lock (_locker)
            {
                foreach (FileItem fileItem in Files)
                {
                    fileItem.IsChecked = fileItem.IsLiked;
                }
            }
        }

        public void SelectUnLiked()
        {
            lock (_locker)
            {
                foreach (FileItem fileItem in Files)
                {
                    fileItem.IsChecked = fileItem.IsUnLiked;
                }
            }
        }

        public void SelectInver()
        {
            lock (_locker)
            {
                foreach (FileItem fileItem in Files)
                {
                    fileItem.IsChecked = !fileItem.IsChecked;
                }
            }
        }

        public void SelectSameSeries(int s)
        {
            lock (_locker)
            {
                foreach (FileItem fileItem in Files)
                {
                    fileItem.IsChecked = fileItem.Series == s;
                }
            }
        }

        public string CopyBackUp(string source, string dest)
        {
            try
            {
                if (string.IsNullOrEmpty(BackUpPath))
                    return null;
                if (!Directory.Exists(BackUpPath))
                    Directory.CreateDirectory(BackUpPath);
                string backupFile = Path.Combine(BackUpPath, Path.GetFileName(dest));
                // if file already exist with same name generate a unique name
                if (File.Exists(backupFile))
                {
                    backupFile = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                }
                File.Copy(source, backupFile);
                return backupFile;
            }
            catch (Exception ex)
            {
                Log.Error("Unable to make backup ", ex);
                return "";
            }
        }

        public AsyncObservableCollection<AutoExportPluginConfig> AutoExportPluginConfigs { get; set; }
    }
}