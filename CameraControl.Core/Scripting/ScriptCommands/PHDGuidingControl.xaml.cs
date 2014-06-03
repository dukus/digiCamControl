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
    /// Interaction logic for PHDGuidingControl.xaml
    /// </summary>
    public partial class PHDGuidingControl : UserControl
    {
        public PHDGuidingControl(PHDGuiding guiding)
        {
            InitializeComponent();
            cmb_move.Items.Add("Move 1");
            cmb_move.Items.Add("Move 2");
            cmb_move.Items.Add("Move 3");
            cmb_move.Items.Add("Move 4");
            cmb_move.Items.Add("Move 5");
            DataContext = guiding;
        }
    }
}
