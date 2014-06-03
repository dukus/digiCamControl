using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PhotoBooth
{
    /// <summary>
    /// Interaction logic for PhotoBoothWindow.xaml
    /// </summary>
    public partial class PhotoBoothWindow : Window
    {
        // User messages.
        // TODO: localize
        private static string RegularStartPrompt = "Ready";
        private static string OneButtonStartPrompt = "Press the button to start.";
        private static string TakingPicturesPrompt = "Now taking {0} pictures. Please look at the camera.";
        private static string PrintingPrompt = "Printing your photos. Please wait.";
        private static string SmileMessage = "Smile";
        private const int CaptureDelay = 5; // in seconds
        // TODO: make this configurable or monitor the printer queue
        private const int PrintingDelay = 10000; // in milliseconds

        private int timerTick;
        private double initialStatusFontSize;
        private DateTime startTime;

        // TODO: move this to XAML
        private DoubleAnimation fontSizeAnimation;
        private PhotoBoothCamera camera;
        private int saveFileCount = 0;
        private static string SaveCardFormat = "PhotoBoothCard{0}.jpg";
        private ScreenSaverInhibitor screenSaverInhibitor;
        private PhotoCardTemplate cardTemplate;
        private int requiredImagedCount = 0;

        /// <summary>
        /// Gets and set the windows message that will be used to initiate operation when in
        /// <see cref="OneButtonOperation"/>.
        /// </summary>
        public WindowsMessage OneButtonMessage { get; set; }

        /// <summary>
        /// Gets and sets whether card images should be saved.
        /// </summary>
        public bool SaveCards { get; set; }
        
        /// <summary>
        /// Gets or sets the location that card images should be stored.
        /// </summary>
        public DirectoryInfo SaveFileLocation { get; set; }

        /// <summary>
        /// The printer ticket to use when printing cards.
        /// </summary>
        public PrintTicket PrinterSetupTicket {get; set;}

        /// <summary>
        /// Gets whether this windows closed abnormally such as when the camera is disconnected.
        /// </summary>
        public bool ClosedAbnormally
        {
            get;
            private set;
        }

        public bool OneButtonOperation
        {
            get { return (bool)GetValue(OneButtonOperationProperty); }
            set { SetValue(OneButtonOperationProperty, value); }
        }

        public bool TakingPictures
        {
            get { return (bool)GetValue(TakingPicturesProperty); }
            private set { SetValue(TakingPicturesProperty, value); }
        }

        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        public int GridRows
        {
            get { return (int)GetValue(GridRowsProperty); }
            set { SetValue(GridRowsProperty, value); }
        }

        public int GridColumns
        {
            get { return (int)GetValue(GridColumnsProperty); }
            set { SetValue(GridColumnsProperty, value); }
        }

        public PhotoCardTemplate CardTemplate
        {
            get { return this.cardTemplate; }

            set
            {
                this.cardTemplate = value;
                this.OnCardTemplateChanged();                
            }
        }

        /// <summary>
        /// Gets and set the camera to be used by this instance
        /// </summary>
        public PhotoBoothCamera Camera
        {
            get { return this.camera; }
            set
            {
                if (this.camera != null)
                {
                    this.camera.CaptureComplete -= this.camera_CaptureComplete;
                    this.camera.CameraDisconnected -= camera_CameraDisconnected;
                }
                this.camera = value;
                if (this.camera != null)
                {
                    this.camera.CaptureComplete += this.camera_CaptureComplete;
                    this.camera.CameraDisconnected += camera_CameraDisconnected;
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public PhotoBoothWindow()
        {
            InitializeComponent();

            this.initialStatusFontSize = this.statusText.FontSize;
            this.fontSizeAnimation = new DoubleAnimation(this.initialStatusFontSize / 4, TimeSpan.FromSeconds(1.0));
            this.fontSizeAnimation.Completed += fontSizeAnimation_Completed;

            this.screenSaverInhibitor = new ScreenSaverInhibitor(this.Dispatcher);
            this.DataContext = this;

            this.Loaded += PhotoBoothWindow_Loaded;
        }

        private void PhotoBoothWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetStartPrompt();
        }

        private void DoPhotoBoothCountdown()
        {
            string message;
            if (this.timerTick > 0)
            {
                message = string.Format("{0}", this.timerTick);
            }
            else
            {
                message = SmileMessage;
            }
            this.statusText.Text = message;
            this.statusText.BeginAnimation(TextBlock.FontSizeProperty, this.fontSizeAnimation);
        }

        private void ResetStatusFontSize()
        {
            this.statusText.BeginAnimation(TextBlock.FontSizeProperty, null);
            this.statusText.FontSize = this.initialStatusFontSize;
        }

        private void fontSizeAnimation_Completed(object sender, EventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                // The calling thread owns the dispatcher, and hence the UI element
                this.ResetStatusFontSize();
            }
            else
            {
                // Invokation required
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(this.ResetStatusFontSize));
            }

            if (--this.timerTick >= 0)
            {
                this.DoPhotoBoothCountdown();
            }
            else
            {
                this.TakePictureSet();
            }
        }

        private void Close_Executed(object target, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !this.TakingPictures && (this.Camera == null || this.Camera.CameraReady);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.screenSaverInhibitor.Dispose();
            this.screenSaverInhibitor = null;
            base.OnClosed(e);
        }

        private void TakePictureSet_Executed(object target, ExecutedRoutedEventArgs e)
        {
            if (this.Camera != null)
            {
                this.TakePhotoBoothPictures();
            }
        }

        private void TakePictureSet_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CanTakePictureSet();
        }

        private bool CanTakePictureSet()
        {
            return !this.TakingPictures && this.Camera != null && this.Camera.CameraReady;
        }

        private void camera_CaptureComplete(object sender, EventArgs e)
        {
            PhotoBoothCamera camera = sender as PhotoBoothCamera;
            if (camera != null)
            {
                if (this.Dispatcher.CheckAccess())
                {
                    // The calling thread owns the dispatcher, and hence the UI element
                    this.UpdateImageDisplay();
                }
                else
                {
                    // Invokation required
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(this.UpdateImageDisplay));
                }
            }
        }

        private void camera_CameraDisconnected(object sender, EventArgs e)
        {
            PhotoBoothCamera eCamera = sender as PhotoBoothCamera;
            if (eCamera != null)
            {
                this.ClosedAbnormally = true;
                if (this.Dispatcher.CheckAccess())
                {
                    // The calling thread owns the dispatcher, and hence the UI element
                    this.Close();
                }
                else
                {
                    // Invokation required
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(this.Close));
                }
            }
        }

        private void OnCardTemplateChanged()
        {
            int neccessaryImages = 0;
            if (this.CardTemplate != null) 
            {
                neccessaryImages = this.CardTemplate.ImagesRequired;
                while(this.CardTemplate.ImagesRequired < this.CardTemplate.Images.Count)
                {
                    this.CardTemplate.Images.RemoveAt(neccessaryImages);
                }
            }

            if (this.requiredImagedCount != neccessaryImages)
            {
                this.requiredImagedCount = neccessaryImages;
                double squareRoot = Math.Sqrt(this.requiredImagedCount);
                int newRowCount = Convert.ToInt32(Math.Ceiling(squareRoot));
                int newColCount = Convert.ToInt32(Math.Round(squareRoot, 0));

                this.GridRows = newRowCount;
                this.GridColumns = newColCount;
            }

            if (this.CardTemplate != null)
            {
                this.photoDisplay.ItemsSource = this.CardTemplate.Images;
            }
        }

        private void UpdateImageDisplay()
        {
            this.UpdateImageDisplay(this.Camera.CaptureFiles);

            if (this.Camera.CaptureFiles.Count >= 1 && !string.IsNullOrEmpty(this.Camera.CaptureFiles[0]))
            {
                Properties.Settings.Default.ImagePath1 = this.Camera.CaptureFiles[0];
            }
            if (this.Camera.CaptureFiles.Count >= 2 && !string.IsNullOrEmpty(this.Camera.CaptureFiles[1]))
            {
                Properties.Settings.Default.ImagePath2 = this.Camera.CaptureFiles[1];
            }
            if (this.Camera.CaptureFiles.Count >= 3 && !string.IsNullOrEmpty(this.Camera.CaptureFiles[2]))
            {
                Properties.Settings.Default.ImagePath3 = this.Camera.CaptureFiles[2];
            }
            if (this.Camera.CaptureFiles.Count >= 4 && !string.IsNullOrEmpty(this.Camera.CaptureFiles[3]))
            {
                Properties.Settings.Default.ImagePath4 = this.Camera.CaptureFiles[3];
            }

            if (this.Camera.CaptureFiles.Count >= this.CardTemplate.ImagesRequired)
            {
                this.SessionComplete();
            }
        }

        private void SessionComplete()
        {
            this.TakingPictures = false;

            if (this.OneButtonOperation && this.CanPrint())
            {
                this.PrintCard();
            }
            else
            {
                this.SetStartPrompt();
                this.DisplayCardView();
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdateImageDisplay(List<string> imageFilenames)
        {
            if (this.CardTemplate == null)
            {
                return;
            }

            int count = 0;
            BitmapImage image;
            this.statusText.Visibility = System.Windows.Visibility.Hidden;
            this.photoContainer.Visibility = System.Windows.Visibility.Visible;
            foreach (string filename in imageFilenames)
            {
                image = this.GetImage(filename);
                if (this.CardTemplate.Images.Count >= (count + 1))
                {
                    this.CardTemplate.Images[count] = image;
                }
                else
                {
                    this.CardTemplate.Images.Add(image);
                }
                count++;
            }
        }

        private BitmapImage GetImage(string filename)
        {
            Uri imageLocation = new Uri(filename, UriKind.Absolute);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = imageLocation;
            image.EndInit();

            return image;
        }

        private void TakePhotoBoothPictures()
        {
            this.TakingPictures = true;
            this.StatusText = string.Format(TakingPicturesPrompt, this.CardTemplate.ImagesRequired);
            if (this.CardTemplate != null)
            {
                this.CardTemplate.Images.Clear();
            }

            this.photoContainer.Visibility = System.Windows.Visibility.Hidden;
            this.statusText.Visibility = System.Windows.Visibility.Visible;
            this.startTime = DateTime.Now;
            this.timerTick = CaptureDelay;
            this.DoPhotoBoothCountdown();
        }

        private void TakePictureSet()
        {
            if (this.Camera != null && this.CardTemplate != null)
            {
                this.Camera.ImageCount = this.CardTemplate.ImagesRequired;
                this.Camera.BeginTakePictureSet();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Camera = null;
        }

        private void Print_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CanPrint();
        }

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.PrintCard();
        }

        private bool CanPrint()
        {
            return !this.TakingPictures &&
                this.CardTemplate != null &&
                this.CardTemplate.CanPrint &&
                this.photoContainer.Visibility == System.Windows.Visibility.Visible;
        }

        private PhotoCard CreateCardView()
        {
            PhotoCard cardView = null;
            if (this.CardTemplate != null)
            {
                cardView = this.CardTemplate.CreateCard();
            }

            return cardView;
        }

        private void DisplayCardView()
        {
            PhotoCard card = this.CreateCardView();
            if (card != null)
            {
                card.ShowDialog();
            }
        }

        private void PrintCard()
        {
            Debug.Assert(this.CardTemplate != null);
            Debug.Assert(this.CardTemplate.CanPrint);

            this.StatusText = PrintingPrompt;

            PrintDialog printDlg = new PrintDialog();
            if (this.PrinterSetupTicket != null)
            {
                printDlg.PrintTicket = this.PrinterSetupTicket;
            }

            double width = printDlg.PrintableAreaWidth;
            double height = printDlg.PrintableAreaHeight;

            PhotoCard cardView = this.CreateCardView();
            cardView.Width = width;
            cardView.Height = height;
            cardView.Show();

            if (this.SaveCards && this.SaveFileLocation.Exists)
            {
                string fileName = GetNextCardFileName();
                Byte[] jpeg = cardView.GetJpgImage(2.0, 95, Constants.ScreenDPI);
                System.IO.File.WriteAllBytes(fileName, jpeg);
            }

            UIElement content = cardView.RootVisual;
            FixedDocument document = DocumentUtility.CreateFixedDocument(width, height, content);
            printDlg.PrintDocument(document.DocumentPaginator, "Photo Booth");
            cardView.Close();

            // Wait so photo capture doesn't get too far ahead of the printer.
            System.Threading.Thread.Sleep(PrintingDelay);
            this.SetStartPrompt();
        }

        private void SetStartPrompt()
        {
            if (this.OneButtonOperation)
            {
                this.StatusText = OneButtonStartPrompt;
            }
            else
            {
                this.StatusText = RegularStartPrompt;
            }
        }

        private string GetNextCardFileName()
        {
            FileInfo fileInfo;
            string fileName;

            do
            {
                fileName = System.IO.Path.Combine(this.SaveFileLocation.FullName,
                    string.Format(SaveCardFormat, this.saveFileCount++));

                fileInfo = new FileInfo(fileName);
            } while (fileInfo.Exists);

            return fileName;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource hwnd = (HwndSource)PresentationSource.FromVisual(this);
            if (hwnd != null)
            {
                hwnd.AddHook(new HwndSourceHook(WndProc));
            }
        }

        protected IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (this.OneButtonOperation && this.OneButtonMessage != null && this.OneButtonMessage.Matches(msg, wParam, lParam) && this.CanTakePictureSet())
            {
                this.TakePhotoBoothPictures();
            }

            return IntPtr.Zero;
        }

        // Using a DependencyProperty as the backing store for StatusText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string), typeof(PhotoBoothWindow), new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for OneButtonOperation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OneButtonOperationProperty =
            DependencyProperty.Register("OneButtonOperation", typeof(bool), typeof(PhotoBoothWindow), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for TakingPictures.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TakingPicturesProperty =
            DependencyProperty.Register("TakingPictures", typeof(bool), typeof(PhotoBoothWindow), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for GridRows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridRowsProperty =
            DependencyProperty.Register("GridRows", typeof(int), typeof(PhotoBoothWindow), new PropertyMetadata(0));

        // Using a DependencyProperty as the backing store for GridColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridColumnsProperty =
            DependencyProperty.Register("GridColumns", typeof(int), typeof(PhotoBoothWindow), new PropertyMetadata(0));
    }
}
