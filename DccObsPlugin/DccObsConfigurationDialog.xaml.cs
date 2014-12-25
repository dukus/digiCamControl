using CLROBS;
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

namespace DccObsPlugin
{
    /// <summary>
    /// Interaction logic for DccObsConfigurationDialog.xaml
    /// </summary>
    public partial class DccObsConfigurationDialog 
    {
        private XElement config;

        public DccObsConfigurationDialog(XElement config)
        {
            this.config = config;
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
