using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Devices.Others;
using MaterialDesignThemes.Wpf;

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
            Chip s = sender as Chip;
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
