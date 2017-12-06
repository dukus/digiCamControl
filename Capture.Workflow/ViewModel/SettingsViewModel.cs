using Capture.Workflow.Core.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Capture.Workflow.ViewModel
{
    public class SettingsViewModel: ViewModelBase
    {
        public Settings Settings => Settings.Instance;
        public RelayCommand CloseCommand { get; set; }


        public SettingsViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        private void Close()
        {
            Settings.Save();
        }
    }
}
