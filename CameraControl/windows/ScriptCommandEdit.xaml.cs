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
using System.Windows.Shapes;
using CameraControl.Core;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for ScriptCommandEdit.xaml
    /// </summary>
    public partial class ScriptCommandEdit
    {
        public ScriptCommandEdit(UserControl control)
        {
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
            stackPanel.Children.Add(control);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
