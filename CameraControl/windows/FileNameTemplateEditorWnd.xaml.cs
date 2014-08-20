using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using CameraControl.Devices.Others;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for FileNameTemplateEditorWnd.xaml
    /// </summary>
    public partial class FileNameTemplateEditorWnd : INotifyPropertyChanged
    {
        private string _templateString;

        public string TemplateString
        {
            get { return _templateString; }
            set
            {
                _templateString = value;
                NotifyPropertyChanged("TemplateString");
                NotifyPropertyChanged("ExampleTemplateString");
            }
        }

        public string ExampleTemplateString
        {
            get
            {
                try
                {
                    return ServiceProvider.FilenameTemplateManager.GetExample(TemplateString, ServiceProvider.Settings.DefaultSession, ServiceProvider.DeviceManager.SelectedCameraDevice ?? new FakeCameraDevice(), "filename.jpg");
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                return "";
            }
        }

        public FileNameTemplateEditorWnd()
        {
            InitializeComponent();
        }


        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button s = sender as Button;
            if (s != null)
            {
                var text = "_" + (string) s.Content;
                if (txt_templateName.SelectionLength > 0)
                {
                    txt_templateName.SelectedText = text;
                    txt_templateName.SelectionLength = 0;
                    txt_templateName.CaretIndex += text.Length;
                }
                else
                {
                    int index = txt_templateName.CaretIndex;
                    txt_templateName.Text = txt_templateName.Text.Insert(index, text);
                    txt_templateName.CaretIndex = index + text.Length;
                    txt_templateName.SelectionLength = 0;
                }
                txt_templateName.Focus();
                NotifyPropertyChanged("ExampleTemplateString");
            }
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txt_templateName.CaretIndex += TemplateString.Length;
        }

    }
}
