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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CameraControl.Core.Classes;

namespace CameraControl.Plugins.ToolPlugins
{
    /// <summary>
    /// Interaction logic for CombineZpWindows.xaml
    /// </summary>
    public partial class CombineZpWindow 
    {
        public CombineZpWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PhotoUtils.Run("http://www.hadleyweb.pwp.blueyonder.co.uk/");
        }
    }
}
