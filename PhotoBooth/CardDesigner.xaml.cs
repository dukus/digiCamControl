using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoBooth
{
    /// <summary>
    /// Interaction logic for CardDesigner.xaml
    /// </summary>
    public partial class CardDesigner : Window
    {
        private PhotoCardTemplate cardTemplate;
        private UserControl designerControl;

        public PhotoCardTemplate CardTemplate
        {
            get { return this.cardTemplate; }
            set
            {
                if (this.cardTemplate != value)
                {
                    this.cardTemplate = value;
                    this.OnCardTemplateChange();
                }
            }
        }

        public CardDesigner()
        {
            InitializeComponent();
        }

        private void ShowPreview()
        {
            if (this.CardTemplate != null)
            {
                for (int i = this.CardTemplate.Images.Count + 1; i <= this.CardTemplate.ImagesRequired; i++)
                {
                    this.CardTemplate.Images.Add(this.GenerateImage(string.Format("{0}", i)));
                }

                PhotoCard card = this.CardTemplate.CreateCard();
                card.Owner = this.Owner;
                card.SizeToContent = SizeToContent.Width;
                card.Width /= 2;
                card.ShowDialog();
            }
        }

        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowCard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowCard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ShowPreview();
        }

        private void OnCardTemplateChange()
        {
            if (this.layoutGrid != null)
            {
                if (this.designerControl != null)
                {
                    this.layoutGrid.Children.Remove(this.designerControl);
                    this.designerControl = null;
                }

                if (this.CardTemplate != null)
                {
                    this.designerControl = this.CardTemplate.CreateDesigner();
                    if (this.designerControl != null)
                    {
                        this.designerControl.SetValue(Grid.RowProperty, 0);     //row 1
                        this.designerControl.SetValue(Grid.ColumnProperty, 0); //column 1
                        this.layoutGrid.Children.Add(this.designerControl);
                        this.designerControl.DataContext = this.CardTemplate;
                    }
                }
            }

            if (this.CardTemplate != null)
            {
                // TODO: localize
                this.Title = "Card Designer: " + this.CardTemplate.DisplayName;
            }

            this.DataContext = this.CardTemplate;
        }

        private ImageSource GenerateImage(string content)
        {
            int width = 400;
            int height = 300;
            int margin = 10;
            int fontSize = 32;
            FormattedText text = new FormattedText(content,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(this.FontFamily, FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                    fontSize,
                    this.Foreground);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            using(drawingContext)
            {
                Pen pen = new Pen(Brushes.Red, 2);
                Rect rect = new Rect(margin, margin, width - 2 * margin, height - 2 * margin);
                drawingContext.DrawRoundedRectangle(Brushes.Gray, pen, rect, margin, margin);
                drawingContext.DrawText(text, new Point(width / 2, (height - fontSize) / 2));
                drawingContext.Close();
            }
            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            return bmp;
        }
    }
}
