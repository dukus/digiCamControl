using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Capture.Workflow.ViewModel;

namespace Capture.Workflow.View
{
    /// <summary>
    /// Interaction logic for WorkflowViewView.xaml
    /// </summary>
    public partial class WorkflowViewView 
    {
        public WorkflowViewView()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            ((WorkflowViewViewModel)DataContext).Dispose();
        }
    }
}
