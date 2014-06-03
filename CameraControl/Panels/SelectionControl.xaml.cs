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
using CameraControl.Core;
using CameraControl.Core.Classes;

namespace CameraControl.Panels
{
    /// <summary>
    /// Interaction logic for SelectionControl.xaml
    /// </summary>
    public partial class SelectionControl : UserControl
    {
        public SelectionControl()
        {
            SelectAllCommand = new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectAll(); });
            SelectNoneCommand = new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectNone(); });
            SelectLiked = new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectLiked(); });
            SelectUnLiked = new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectUnLiked(); });
            SelectInvertCommand = new RelayCommand<object>(delegate { ServiceProvider.Settings.DefaultSession.SelectInver(); });
            InitializeComponent();
        }

        public RelayCommand<object> SelectAllCommand
        {
            get;
            private set;
        }

        public RelayCommand<object> SelectLiked
        {
            get;
            private set;
        }

        public RelayCommand<object> SelectUnLiked
        {
            get;
            private set;
        }


        public RelayCommand<object> SelectNoneCommand
        {
            get;
            private set;
        }

        public RelayCommand<object> SelectInvertCommand
        {
            get;
            private set;
        }
    }
}
