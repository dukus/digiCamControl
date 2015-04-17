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
using CameraControl.Core.Classes;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome 
    {
        public Welcome()
        {
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
        }

        private void btn_facebook_Click(object sender, RoutedEventArgs e)
        {
            PhotoUtils.Run("http://www.facebook.com/DigiCamControl");
            Close();
        }

        private void btn_google_Click(object sender, RoutedEventArgs e)
        {
            PhotoUtils.Run("https://plus.google.com/+Digicamcontrol");
            Close();
        }

        private void btn_donate_Click(object sender, RoutedEventArgs e)
        {
            PhotoUtils.Run("http://digicamcontrol.com/donate");
            Close();
        }

        private void btn_twitter_Click(object sender, RoutedEventArgs e)
        {
            PhotoUtils.Run("https://twitter.com/digiCamControl");
            Close();
        }

        private void btn_flickr_Click(object sender, RoutedEventArgs e)
        {
            PhotoUtils.Run("https://www.flickr.com/groups/2224376@N22/");
            Close();
        }
    }
}
