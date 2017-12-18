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
using Capture.Workflow.Core;
using Capture.Workflow.Core.Classes;
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
            WorkflowManager.Instance.Message += Instance_Message;
        }

        private void Instance_Message(object sender, Core.Classes.MessageEventArgs e)
        {
            switch (e.Name)
            {
                case Messages.SessionCanceled:
                    Application.Current.Dispatcher.BeginInvoke(new Action(Close));
                    break;
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            ((WorkflowViewViewModel)DataContext).Dispose();
        }

    }
}
