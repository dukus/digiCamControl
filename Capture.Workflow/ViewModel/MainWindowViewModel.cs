using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Capture.Workflow.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        public RelayCommand EditCommand { get; set; }

        public MainWindowViewModel()
        {
            EditCommand=new RelayCommand(Edit);
        }

        private void Edit()
        {
            WorkflowEditorView wnd = new WorkflowEditorView();
            wnd.ShowDialog();
        }
    }
}
