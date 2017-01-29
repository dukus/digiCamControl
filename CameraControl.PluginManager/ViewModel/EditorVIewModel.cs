using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using CameraControl.Core.Classes;
using CameraControl.Core.Plugin;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.PluginManager.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ionic.Zip;
using Microsoft.Win32;

namespace CameraControl.PluginManager.ViewModel
{
    public class EditorViewModel : ViewModelBase
    {
        private PluginInfo _pluginInfo;
        private AsyncObservableCollection<ProjectFileItem> _files;
        private string _fileName;


        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(()=>FileName);
            }
        }


        public PluginInfo PluginInfo
        {
            get { return _pluginInfo; }
            set
            {
                _pluginInfo = value;
                RaisePropertyChanged(() => PluginInfo);
            }
        }

        public AsyncObservableCollection<ProjectFileItem> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                RaisePropertyChanged(() => Files);
            }
        }

        public RelayCommand AddFilesCommand { get; set; }
        public RelayCommand LoadPluginInfoCommand { get; set; }
        public RelayCommand SavePluginInfoCommand { get; set; }
        public RelayCommand LoadProjectCommand { get; set; }
        public RelayCommand SaveProjectCommand { get; set; }
        public RelayCommand NewCommand { get; set; }
        public RelayCommand GenerateCommand { get; set; }


        public EditorViewModel()
        {
            PluginInfo = new PluginInfo();
            Files = new AsyncObservableCollection<ProjectFileItem>();
            AddFilesCommand = new RelayCommand(AddFiles);
            LoadPluginInfoCommand = new RelayCommand(LoadPluginInfo);
            SavePluginInfoCommand = new RelayCommand(SavePluginInfo);
            LoadProjectCommand = new RelayCommand(LoadProject);
            SaveProjectCommand = new RelayCommand(SaveProject);
            NewCommand = new RelayCommand(New);
            GenerateCommand = new RelayCommand(Generate);
        }

        private void Generate()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Plugin package file (*.dccplugin)|*.dccplugin|All files|*.*";
            dialog.OverwritePrompt = true;
            dialog.FileName = PluginInfo.Name + "." + PluginInfo.Version + ".dccplugin";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var tempfile = Path.Combine(Path.GetTempPath(), "dcc.plugin");
                    Save(tempfile, PluginInfo);
                    using (ZipFile zip = new ZipFile())
                    {
                        foreach (var file in Files)
                        {
                            zip.AddFile(file.FileName,"");
                        }
                        zip.AddFile(tempfile, "");
                        zip.Save(dialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to generate plugin package " + ex.Message);

                }
            }
        }

        private void New()
        {
            PluginInfo = new PluginInfo();
            Files = new AsyncObservableCollection<ProjectFileItem>();
        }

        private void SaveProject()
        {
            var project = new ProjectFile();
            project.Files = Files;
            project.PluginInfo = PluginInfo;
            var dialog = new SaveFileDialog();
            dialog.Filter = "Project file (*.dccproj)|*.dccproj|All files|*.*";
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    Save(dialog.FileName, project);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to project file " + ex.Message);

                }
            }
        }

        private void LoadProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Project file (*.dccproj)|*.dccproj|All files|*.*";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var project = Load<ProjectFile>(dialog.FileName);
                    Files = project.Files;
                    PluginInfo = project.PluginInfo;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load plugin file " + ex.Message);

                }
            }

        }

        private void SavePluginInfo()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Plugin file (*.plugin)|*.plugin|All files|*.*";
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    Save(dialog.FileName, PluginInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to save plugin file " + ex.Message);

                }
            }
        }

        private void LoadPluginInfo()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Plugin file (*.plugin)|*.plugin|All files|*.*";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    PluginInfo = Load<PluginInfo>(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load plugin file "+ex.Message);
                    
                }
            }
        }

        private void AddFiles()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                foreach (string fileName in dialog.FileNames)
                {
                    Files.Add(new ProjectFileItem(fileName));
                }
            }
        }

        public static void Save<T>(string file, T data)
        {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                // Create a FileStream to write with.

                Stream writer = new FileStream(file, FileMode.Create);
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, data);
                writer.Close();
        }

        public T Load<T>(string file)
        {


            XmlSerializer mySerializer =
                new XmlSerializer(typeof(T));
            FileStream myFileStream = new FileStream(file, FileMode.Open);
            var defaultSettings = (T)mySerializer.Deserialize(myFileStream);
            myFileStream.Close();

            return defaultSettings;
        }
    }
}

