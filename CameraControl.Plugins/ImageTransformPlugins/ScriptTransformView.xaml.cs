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

namespace CameraControl.Plugins.ImageTransformPlugins
{
    /// <summary>
    /// Interaction logic for ScriptTransformView.xaml
    /// </summary>
    public partial class ScriptTransformView : UserControl
    {
        public ScriptTransformView()
        {
            InitializeComponent();
        }

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            ((ScriptTransformViewModel) DataContext).Script = Editor.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ScriptTransformViewModel)DataContext).Load();
            Editor.Text = ((ScriptTransformViewModel)DataContext).Script;
        }

    }
}
