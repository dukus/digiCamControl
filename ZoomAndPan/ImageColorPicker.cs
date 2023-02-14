using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZoomAndPan
{
    /// <summary>
    /// Image element with the ability to pick out a pixel color value.
    /// </summary>
    /// <remarks>
    /// <see cref="ImageColorPicker"/> element adorns the <see cref="Image"/>
    /// it's derived from with the facility to pick the image pixel color value at the position 
    /// specified by the selector visual.
    /// </remarks>
    public class ImageColorPicker : Image
    {
        #region dependency properties
        /// <summary>
        /// SelectedColor property backing ReadOnly DependencyProperty.
        /// </summary>
        private static readonly DependencyPropertyKey SelectedColorPropertyKey = DependencyProperty.RegisterReadOnly("SelectedColor", typeof(Color),
            typeof(ImageColorPicker), new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        public Color SelectedColor => (Color)GetValue(SelectedColorPropertyKey.DependencyProperty);

        /// <summary>
        /// Selector property backing DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SelectorProperty = DependencyProperty.Register("Selector", typeof(Drawing),
            typeof(ImageColorPicker), new FrameworkPropertyMetadata(new GeometryDrawing(Brushes.White, new Pen(Brushes.Black, 1),
                new EllipseGeometry(new Point(), 3, 3))
            {
                Pen = new Pen(new SolidColorBrush(Color.FromArgb(212, 0, 0, 0)), 1),
                Brush = new SolidColorBrush(Color.FromArgb(80, 255, 255, 255))
            }, FrameworkPropertyMetadataOptions.AffectsRender));
        public Drawing Selector
        {
            get { return (Drawing)GetValue(SelectorProperty); }
            set { SetValue(SelectorProperty, value); }
        }

        /// <summary>
        /// Scaling for the TargetImage
        /// </summary>
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double),
            typeof(ImageColorPicker), new PropertyMetadata(0.0, (d, e) => (d as Image)?.InvalidateVisual()));
        public double Scale { get { return (double)GetValue(ScaleProperty); } set { SetValue(ScaleProperty, value); } }
        #endregion

        #region overridden methods
        /// <summary>
        /// Renders the contents of an <see cref="T:System.Windows.Controls.Image"/> and 
        /// the SelectorDrawing.
        /// </summary>
        /// <param name="drawingContext">An instance of <see cref="T:System.Windows.Media.DrawingContext"/> 
        /// used to render the control.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (ActualWidth == 0 || ActualHeight == 0)
                return;

            // Render the SelectorDrawing
            drawingContext.PushTransform(new TranslateTransform(Position.X, Position.Y));
            if (Scale > 0) drawingContext.PushTransform(new ScaleTransform(1 / Scale, 1 / Scale));
            drawingContext.DrawDrawing(Selector);
            drawingContext.Pop();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, 
        /// using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            _cachedTargetBitmap = null; // TargetBitmap cache isn't valid anymore.
                                        // Adjust the selector position proportionally to size change.
            if (sizeInfo.PreviousSize.Width > 0 && sizeInfo.PreviousSize.Height > 0)
                Position = new Point(Position.X * sizeInfo.NewSize.Width / sizeInfo.PreviousSize.Width
                    , Position.Y * sizeInfo.NewSize.Height / sizeInfo.PreviousSize.Height);
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this 
        /// <see cref="T:System.Windows.FrameworkElement"/> has been updated. 
        /// The specific dependency property that changed is reported in the arguments parameter. 
        /// Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, 
        /// as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Source")
            {
                _cachedTargetBitmap = null; // TargetBitmap cache isn't valid anymore.
                Position = new Point(); // Move the selector to the top-left corner.
            }
            base.OnPropertyChanged(e);
        }
        #endregion

        #region Position
        /// <summary>
        /// Gets or sets the Selector Position.
        /// </summary>
        /// <value>The position.</value>
        Point Position
        {
            get { return _position; }
            set
            {
                var newPos = new Point(Clamp(value.X, 0, ActualWidth), Clamp(value.Y, 0, ActualHeight));
                if (_position != newPos && Source != null)
                {
                    _position = newPos;
                    Color color = GetColor(_position.X, _position.Y);
                    if (color == SelectedColor) InvalidateVisual();
                    SetValue(SelectedColorPropertyKey, color);
                }
            }
        }
        Point _position;

        /// <summary>
        /// Sets the <paramref name="pt"/> as the new position if the point falls 
        /// into the element bounds.
        /// </summary>
        /// <param name="pt">The point.</param>
        void SetPositionIfInBounds(Point pt)
        {
            if (pt.X >= 0 && pt.X <= ActualWidth && pt.Y >= 0 && pt.Y <= ActualHeight)
                Position = pt;
        }
        #endregion Position

        #region TargetBitmap
        RenderTargetBitmap _cachedTargetBitmap;
        /// <summary>
        /// Gets the target bitmap for the DrawingImage image Source.
        /// </summary>
        /// <value>The target bitmap.</value>
        RenderTargetBitmap TargetBitmap
        {
            get
            {
                if (_cachedTargetBitmap == null)
                {
                    var drawingImage = Source as DrawingImage;
                    if (drawingImage != null)
                    {
                        DrawingVisual drawingVisual = new DrawingVisual();
                        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                        {
                            drawingContext.DrawDrawing(drawingImage.Drawing);
                        }

                        // Scale the DrawingVisual.
                        Rect dvRect = drawingVisual.ContentBounds;
                        drawingVisual.Transform = new ScaleTransform(ActualWidth / dvRect.Width
                            , ActualHeight / dvRect.Height);

                        _cachedTargetBitmap = new RenderTargetBitmap((int)ActualWidth
                            , (int)ActualHeight, 96, 96, PixelFormats.Pbgra32);
                        _cachedTargetBitmap.Render(drawingVisual);
                    }
                }
                return _cachedTargetBitmap;
            }
        }
        #endregion TargetBitmap

        #region Mouse handling
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            SetPositionIfInBounds(e.GetPosition(this));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            SetPositionIfInBounds(e.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
                SetPositionIfInBounds(e.GetPosition(this));
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePoint = e.GetPosition(this);
                Position = new Point(mousePoint.X, mousePoint.Y);
            }
        }
        #endregion Mouse handling

        /// <summary>
        /// Picks the color at the position specified.
        /// </summary>
        /// <param name="x">The x coordinate in WPF pixels.</param>
        /// <param name="y">The y coordinate in WPF pixels.</param>
        /// <returns>The image pixel color at x,y position.</returns>
        /// <remarks>
        /// Input coordinates are scaled according to the underlying image resolution,
        /// so this method doesn't expect exceptions thrown by the 
        /// <see cref="M:System.Windows.Media.Imaging.BitmapSource.CopyPixels"/> method.
        /// <para>Color can be picked not only from the 
        /// <see cref="T:System.Windows.Media.Imaging.BitmapSource"/>, but also from the
        /// <see cref="T:System.Windows.Media.DrawingImage"/>.</para>
        /// </remarks>
        Color GetColor(double x, double y)
        {
            if (Source == null) throw new InvalidOperationException("Image Source not set");

            BitmapSource bitmapSource = Source as BitmapSource;
            if (bitmapSource != null)
            { // Get color from bitmap pixel.
              // Convert coopdinates from WPF pixels to Bitmap pixels and restrict them by the Bitmap bounds.
                x *= bitmapSource.PixelWidth / ActualWidth;
                if ((int)x > bitmapSource.PixelWidth - 1)
                    x = bitmapSource.PixelWidth - 1;
                else if (x < 0)
                    x = 0;
                y *= bitmapSource.PixelHeight / ActualHeight;
                if ((int)y > bitmapSource.PixelHeight - 1)
                    y = bitmapSource.PixelHeight - 1;
                else if (y < 0)
                    y = 0;

                // Lee Brimelow approach (http://thewpfblog.com/?p=62).
                //byte[] pixels = new byte[4];
                //CroppedBitmap cb = new CroppedBitmap(bitmapSource, new Int32Rect((int)x, (int)y, 1, 1));
                //cb.CopyPixels(pixels, 4, 0);
                //return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);

                // Alternative approach
                if (bitmapSource.Format == PixelFormats.Indexed4)
                {
                    byte[] pixels = new byte[1];
                    int stride = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 3) / 4;
                    bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), pixels, stride, 0);

                    Debug.Assert(bitmapSource.Palette != null, "bitmapSource.Palette != null");
                    Debug.Assert(bitmapSource.Palette.Colors.Count == 16, "bitmapSource.Palette.Colors.Count == 16");
                    return bitmapSource.Palette.Colors[pixels[0] >> 4];
                }
                else if (bitmapSource.Format == PixelFormats.Indexed8)
                {
                    byte[] pixels = new byte[1];
                    int stride = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 7) / 8;
                    bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), pixels, stride, 0);

                    Debug.Assert(bitmapSource.Palette != null, "bitmapSource.Palette != null");
                    Debug.Assert(bitmapSource.Palette.Colors.Count == 256, "bitmapSource.Palette.Colors.Count == 256");
                    return bitmapSource.Palette.Colors[pixels[0]];
                }
                else
                {
                    byte[] pixels = new byte[4];
                    int stride = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 7) / 8;
                    bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), pixels, stride, 0);

                    return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                }
                // TODO There are other PixelFormats which processing should be added if desired.
            }

            DrawingImage drawingImage = Source as DrawingImage;
            if (drawingImage != null)
            { // Get color from drawing pixel.
                RenderTargetBitmap targetBitmap = TargetBitmap;
                Debug.Assert(targetBitmap != null, "targetBitmap != null");

                // Convert coopdinates from WPF pixels to Bitmap pixels and restrict them by the Bitmap bounds.
                x *= targetBitmap.PixelWidth / ActualWidth;
                if ((int)x > targetBitmap.PixelWidth - 1)
                    x = targetBitmap.PixelWidth - 1;
                else if (x < 0)
                    x = 0;
                y *= targetBitmap.PixelHeight / ActualHeight;
                if ((int)y > targetBitmap.PixelHeight - 1)
                    y = targetBitmap.PixelHeight - 1;
                else if (y < 0)
                    y = 0;

                // TargetBitmap is always in PixelFormats.Pbgra32 format.
                // Pbgra32 is a sRGB format with 32 bits per pixel (BPP). Each channel (blue, green, red, and alpha)
                // is allocated 8 bits per pixel (BPP). Each color channel is pre-multiplied by the alpha value. 
                byte[] pixels = new byte[4];
                int stride = (targetBitmap.PixelWidth * targetBitmap.Format.BitsPerPixel + 7) / 8;
                targetBitmap.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), pixels, stride, 0);
                return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
            }
            throw new InvalidOperationException("Unsupported Image Source Type");
        }

        private static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
        }
    }
}
