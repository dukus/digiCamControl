using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Classes;
using GalaSoft.MvvmLight;

namespace Capture.Workflow.ViewModel
{
    public class NewViewSelectorViewModel:ViewModelBase
    {
        public List<ValueItem> Items { get; set; }
        public ValueItem SelectedItem { get; set; }
        public string Name { get; set; }
        public ValueItem ReturnItem { get; set; }

        public NewViewSelectorViewModel()
        {
            Items=new List<ValueItem>();
        }
    }
}
