using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    /// <summary>
    /// Interaction logic for BulbCaptureControl.xaml
    /// </summary>
    public partial class BulbCaptureControl : UserControl
    {
        public BulbCapture BulbCapture { get; set; }

        public BulbCaptureControl(BulbCapture capture)
        {
            BulbCapture = capture;
            InitializeComponent();
            DataContext = capture;
        }
    }
}
