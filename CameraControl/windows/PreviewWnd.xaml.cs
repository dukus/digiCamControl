using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraControl.Core;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for PreviewWnd.xaml
    /// </summary>
    public partial class PreviewWnd
    {
        public PreviewWnd()
        {
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
        }

        private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var AssociatedObject = this;
            // Retrieve the coordinate of the mouse position in relation to the supplied image.
            Point point = e.GetPosition(AssociatedObject);

            // Use RenderTargetBitmap to get the visual, in case the image has been transformed.
            var renderTargetBitmap = new RenderTargetBitmap((int) AssociatedObject.ActualWidth,
                (int) AssociatedObject.ActualHeight,
                96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(AssociatedObject);

            // Make sure that the point is within the dimensions of the image.
            if ((point.X <= renderTargetBitmap.PixelWidth) && (point.Y <= renderTargetBitmap.PixelHeight))
            {
                // Create a cropped image at the supplied point coordinates.
                var croppedBitmap = new CroppedBitmap(renderTargetBitmap,
                    new Int32Rect((int) point.X, (int) point.Y, 1, 1));

                // Copy the sampled pixel to a byte array.
                var pixels = new byte[4];
                croppedBitmap.CopyPixels(pixels, 4, 0);

                // Assign the sampled color to a SolidColorBrush and return as conversion.
                var SelectedColor = Color.FromArgb(255, pixels[2], pixels[1], pixels[0]);
                TextBox.Text = "#" + SelectedColor.ToString().Substring(3);
                Label.Background = new SolidColorBrush(SelectedColor);
            }
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (TextBox.Text != null) Clipboard.SetText(TextBox.Text);
            }
            catch (Exception)
            {
                
            }
        }
    }
}
