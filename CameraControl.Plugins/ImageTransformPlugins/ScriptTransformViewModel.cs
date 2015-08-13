using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Classes;
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


        public string Script
        {
            get { return _config["Script"]; }
            set
            {
                _config["Script"] = value;
                RaisePropertyChanged(()=>Script);
            }
        }

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
        }

        public ScriptTransformViewModel(ValuePairEnumerator config)
        {
            _config = config;
            AvailableScripts = new List<string>();
            LoadCommand = new RelayCommand(Load);
        }

        private void Load()
        {
            
        }
    }
}
