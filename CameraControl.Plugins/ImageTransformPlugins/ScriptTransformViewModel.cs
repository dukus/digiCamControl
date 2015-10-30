using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;

namespace CameraControl.Plugins.ImageTransformPlugins
{
    public class ScriptTransformViewModel : ViewModelBase
    {
        private ValuePairEnumerator _config = new ValuePairEnumerator();
        private string _script;
        private List<string> _availableScripts;

        public RelayCommand LoadCommand { get; set; }
        public RelayCommand OpenCommand { get; set; }


        public string Script
        {
            get { return _config["Script"]; }
            set
            {
                _config["Script"] = value;
                RaisePropertyChanged(()=>Script);
            }
        }

        public string SelectedScript { get; set; }

        public List<string> AvailableScripts
        {
            get { return _availableScripts; }
            set
            {
                _availableScripts = value;
                RaisePropertyChanged(() => AvailableScripts);
            }
        }

        public ScriptTransformViewModel()
        {
            AvailableScripts = new List<string>();
            LoadCommand = new RelayCommand(Load);
            OpenCommand = new RelayCommand(Open);
        }

        public ScriptTransformViewModel(ValuePairEnumerator config)
        {
            _config = config;
            AvailableScripts = new List<string>();
            LoadCommand = new RelayCommand(Load);
            OpenCommand = new RelayCommand(Open);
            try
            {
                var files = Directory.GetFiles(Path.Combine(Settings.ApplicationFolder, "Data", "msl"), "*.msl");
                foreach (var file in files)
                {
                    AvailableScripts.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error load scrip list", ex);
            }
        }

        public void Load()
        {
            try
            {
                var file = Path.Combine(Settings.ApplicationFolder,"Data", "msl", SelectedScript + ".msl");
                Script = File.ReadAllText(file);
            }
            catch (Exception ex)
            {
                Log.Error("Error");
            }
        }

        public void SetScript(string s)
        {
            _script = s;
        }

        public void Open()
        {
            try
            {
                var file = Path.Combine(Settings.ApplicationFolder, "Data", "msl", SelectedScript + ".msl");
                PhotoUtils.Run("notepad.exe", file);
            }
            catch (Exception ex)
            {
                Log.Error("Error", ex);
            }
        }
    }
}
