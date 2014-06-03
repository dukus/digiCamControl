using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PhotoBooth
{
    /// <summary>
    /// Interaction logic for PhotoBoothControlWindow.xaml
    /// </summary>
    public partial class PhotoBoothControlWindow : Window
    {
        private PhotoBoothCamera camera;
        private PrintTicket printerSetupTicket;
        private bool initializing = false;
        private PhotoCardTemplate selectedTemplate;

        public IEnumerable<WindowsMessage> OneButtonMessages
        {
            get { return WindowsMessage.OneButtonMessages; }
        }

        public bool SaveCards
        {
            get { return (bool)GetValue(SaveCardsProperty); }
            set { SetValue(SaveCardsProperty, value); }
        }

        public string SaveFileFolder
        {
            get { return (string)GetValue(SaveFileFolderProperty); }
            set { SetValue(SaveFileFolderProperty, value); }
        }

        public bool KioskMode
        {
            get { return (bool)GetValue(KioskModeProperty); }
            set { SetValue(KioskModeProperty, value); }
        }

        public bool OneButtonOperation
        {
            get { return (bool)GetValue(OneButtonOperationProperty); }
            set { SetValue(OneButtonOperationProperty, value); }
        }

        public WindowsMessage OneButtonMessage
        {
            get { return (WindowsMessage)GetValue(OneButtonMessageProperty); }
            set { SetValue(OneButtonMessageProperty, value); }
        }

        public PhotoCardTemplateInfo SelectedTemplateInfo
        {
            get { return (PhotoCardTemplateInfo)GetValue(SelectedTemplateInfoProperty); }
            set { SetValue(SelectedTemplateInfoProperty, value); }
        }

        public List<PhotoCardTemplateInfo> AvailableTemplates
        {
            get { return (List<PhotoCardTemplateInfo>)GetValue(AvailableTemplatesProperty); }
            set { SetValue(AvailableTemplatesProperty, value); }
        }

        private PhotoCardTemplate SelectedTemplate
        {
            get
            {
                if (this.SelectedTemplateInfo == null)
                {
                    this.selectedTemplate = null;
                }
                else if (this.selectedTemplate == null || this.selectedTemplate.GetType() != this.SelectedTemplateInfo.TemplateType)
                {
                    this.selectedTemplate = this.SelectedTemplateInfo.CreateTemplate();
                    Properties.Settings.Default.PhototCardTemplate = this.SelectedTemplateInfo.TemplateType.Name;
                    if (this.selectedTemplate != null)
                    {
                        if (this.image1.Source != null)
                        {
                            this.selectedTemplate.Images.Add(this.image1.Source);
                        }
                        if (this.image2.Source != null)
                        {
                            this.selectedTemplate.Images.Add(this.image2.Source);
                        }
                        if (this.image3.Source != null)
                        {
                            this.selectedTemplate.Images.Add(this.image3.Source);
                        }
                        if (this.image4.Source != null)
                        {
                            this.selectedTemplate.Images.Add(this.image4.Source);
                        }
                    }
                }

                return this.selectedTemplate;
            }
        }

        public PhotoBoothControlWindow()
        {
            if (Properties.Settings.Default.PrintTicket != null)
            {
                this.printerSetupTicket = Properties.Settings.Default.PrintTicket;
            }

            string saveFileFolder = Properties.Settings.Default.SaveFileFolder;
            if (string.IsNullOrWhiteSpace(saveFileFolder))
            {
                saveFileFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
            this.SaveCards = Properties.Settings.Default.SaveCards;
            this.SaveFileFolder = saveFileFolder;
            this.OneButtonOperation = Properties.Settings.Default.OneButtonOperation;
            this.KioskMode = Properties.Settings.Default.KioskMode;

            InitializeComponent();
            this.DataContext = this;

            this.LoadPreviousImages();

            this.AvailableTemplates = PhotoCardTemplateInfo.GetTemplateListing();

            this.SelectedTemplateInfo = this.AvailableTemplates.FirstOrDefault(t => t.TemplateType.Name == Properties.Settings.Default.PhototCardTemplate);

            this.OneButtonMessage = this.OneButtonMessages.FirstOrDefault(m => m.Name == Properties.Settings.Default.OneButtonMessage);
        }

        private void LoadPreviousImages()
        {
            List<string> imageFiles = new List<string>();
            AddToImageList(imageFiles, Properties.Settings.Default.ImagePath1);
            AddToImageList(imageFiles, Properties.Settings.Default.ImagePath2);
            AddToImageList(imageFiles, Properties.Settings.Default.ImagePath3);
            AddToImageList(imageFiles, Properties.Settings.Default.ImagePath4);

            if (imageFiles.Count > 0)
            {
                this.UpdateImageDisplay(imageFiles);
            }
        }

        private static void AddToImageList(List<string> imageFiles, string filename)
        {
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                imageFiles.Add(filename);
            }
        }

        private void DesignCard()
        {
            if (this.SelectedTemplate != null)
            {
                CardDesigner designer = new CardDesigner();
                designer.Owner = this;
                designer.CardTemplate = this.SelectedTemplate;
                designer.ShowDialog();
            }
        }

        private void ShowPhotoBoothWindow()
        {
            PhotoBoothWindow window = new PhotoBoothWindow()
            {
                Camera = this.camera,
                PrinterSetupTicket = this.printerSetupTicket,
                OneButtonOperation = this.OneButtonOperation,
                OneButtonMessage = this.OneButtonMessage,
                CardTemplate = this.SelectedTemplate,
                SaveCards = this.SaveCards
            };

            if(this.KioskMode)
            {
                window.WindowState = System.Windows.WindowState.Maximized;
                window.WindowStyle = System.Windows.WindowStyle.None;
                window.Topmost = true;
            }

            if (this.SaveCards && !string.IsNullOrEmpty(this.SaveFileFolder))
            {
                DirectoryInfo saveFolderInfo = new DirectoryInfo(this.SaveFileFolder);
                if (saveFolderInfo.Exists)
                {
                    window.SaveFileLocation = saveFolderInfo;
                }
            }

            window.ShowDialog();

            if (window.ClosedAbnormally)
            {
                MessageBox.Show("Photobooth closed abnormally");
                this.CloseCamera();
            }
        }

        private void OpenSaveFile_Executed(object target, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (!string.IsNullOrEmpty(this.SaveFileFolder))
            {
                dlg.SelectedPath = this.SaveFileFolder;
            }

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.SaveFileFolder = dlg.SelectedPath;
            }
        }

        private void OpenSaveFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SaveCards;
        }

        private void PhotoBooth_Executed(object target, ExecutedRoutedEventArgs e)
        {
            this.ShowPhotoBoothWindow();
        }

        private void PhotoBooth_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !this.initializing && 
                this.camera != null && this.camera.CameraReady && 
                this.SelectedTemplate != null &&
                (!this.OneButtonOperation || this.OneButtonMessage != null);
        }

        private void PrinterSetup_Executed(object target, ExecutedRoutedEventArgs e)
        {
            PrintDialog dlg = new PrintDialog();
            if (this.printerSetupTicket != null)
            {
                dlg.PrintTicket = this.printerSetupTicket;
            }
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                this.printerSetupTicket = dlg.PrintTicket;
                Properties.Settings.Default.PrintTicket = this.printerSetupTicket;
            }
        }

        private void PrinterSetup_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !this.initializing;
        }

        private void InitializeCamera_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !this.initializing;
        }

        private void InitializeCamera_Executed(object target, ExecutedRoutedEventArgs e)
        {
            this.InitializeCamera();
        }

        private void DesignCard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !this.initializing && this.SelectedTemplateInfo != null;
        }

        private void DesignCard_Executed(object target, ExecutedRoutedEventArgs e)
        {
            this.DesignCard();
        }

        private void InitializeCamera()
        {
            System.Windows.Input.Cursor originalCursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;
                this.initializing = true;
                CommandManager.InvalidateRequerySuggested();
                this.CloseCamera();

                this.camera = new PhotoBoothCamera();
                if (this.camera.Initialize())
                {
                    this.cameraSettingsGrid.DataContext = this.camera.Camera;
                }
                else
                {
                    MessageBox.Show(this.camera.InitializationLog);
                    this.CloseCamera();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Caught an exception during camera initialization: " + ex.Message);
            }
            finally
            {
                this.initializing = false;
                CommandManager.InvalidateRequerySuggested();
                this.Cursor = originalCursor;
            }
        }

        private void UpdateImageDisplay(List<string> imageFilenames)
        {
            int count = 0;

            foreach (string filename in imageFilenames)
            {
                if (!File.Exists(filename))
                {
                    continue;
                }

                Uri imageLocation = new Uri(filename, UriKind.Absolute);
                BitmapImage image = new BitmapImage(imageLocation);

                switch (count)
                {
                    case 0:
                        this.image1.Source = image;
                        this.image1.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 1:
                        this.image2.Source = image;
                        this.image2.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 2:
                        this.image3.Source = image;
                        this.image3.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 3:
                        this.image4.Source = image;
                        this.image4.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        break;
                }
                if (count >= 3)
                {
                    break;
                }
                count++;
            }
        }

        private void CloseCamera()
        {
            if (this.camera != null)
            {
                this.camera.Dispose();
                this.camera = null;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.SaveFileFolder = this.SaveFileFolder;
            Properties.Settings.Default.OneButtonOperation  = this.OneButtonOperation;
            Properties.Settings.Default.KioskMode = this.KioskMode;
            Properties.Settings.Default.SaveCards = this.SaveCards;
            if(this.OneButtonMessage != null)
            {
                Properties.Settings.Default.OneButtonMessage = this.OneButtonMessage.Name;
            }
            Properties.Settings.Default.Save();

            base.OnClosing(e);
            this.CloseCamera();
        }

        // Using a DependencyProperty as the backing store for OneButtonOperation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OneButtonOperationProperty =
            DependencyProperty.Register("OneButtonOperation", typeof(bool), typeof(PhotoBoothControlWindow), new PropertyMetadata(false));




        // Using a DependencyProperty as the backing store for OneButtonMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OneButtonMessageProperty =
            DependencyProperty.Register("OneButtonMessage", typeof(WindowsMessage), typeof(PhotoBoothControlWindow), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for KioskMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KioskModeProperty =
            DependencyProperty.Register("KioskMode", typeof(bool), typeof(PhotoBoothControlWindow), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for SaveCards.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveCardsProperty =
            DependencyProperty.Register("SaveCards", typeof(bool), typeof(PhotoBoothControlWindow), new PropertyMetadata(false));
        
        // Using a DependencyProperty as the backing store for SaveFileFolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveFileFolderProperty =
            DependencyProperty.Register("SaveFileFolder", typeof(string), typeof(PhotoBoothControlWindow), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for AvailableTemplates.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailableTemplatesProperty =
            DependencyProperty.Register("AvailableTemplates", typeof(List<PhotoCardTemplateInfo>), typeof(PhotoBoothControlWindow), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for SelectedTemplateInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTemplateInfoProperty =
            DependencyProperty.Register("SelectedTemplateInfo", typeof(PhotoCardTemplateInfo), typeof(PhotoBoothControlWindow), new PropertyMetadata(null));
    }
}
