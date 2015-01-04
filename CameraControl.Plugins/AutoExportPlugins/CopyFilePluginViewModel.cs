using System.Windows.Forms;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class CopyFilePluginViewModel : BasePluginViewModel
    {

        public RelayCommand BrowseCommand { get; set; }

        public CopyFilePluginViewModel(AutoExportPluginConfig config)
        {
            _config = config;
            BrowseCommand = new RelayCommand(Browse);
        }

        private void Browse()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Path;
            if (dialog.ShowDialog() == DialogResult.OK)
                Path = dialog.SelectedPath;
        }

        public CopyFilePluginViewModel()
        {

        }

        public string Path
        {
            get { return _config.ConfigData["Path"]; }
            set
            {
                _config.ConfigData["Path"] = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public string FileName
        {
            get { return _config.ConfigData["FileName"]; }
            set
            {
                _config.ConfigData["FileName"] = value;
                RaisePropertyChanged(() => Path);
            }
        }
    }
}
