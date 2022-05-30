using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZoomAndPan
{
    /// <summary>
    /// A class that wraps up zooming and panning of it's content.
    /// </summary>
    public class ZoomAndPanViewBox : ContentControl
    {
        #region local fields
        /// <summary>
        /// The control for creating a drag border
        /// </summary>
        private Border _dragBorder;

        /// <summary>
        /// The control for creating a drag border
        /// </summary>
        private Border _sizingBorder;

        /// <summary>
        /// The control for containing a zoom border
        /// </summary>
        private Canvas _viewportCanvas;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingModeEnum _mouseHandlingMode = MouseHandlingModeEnum.None;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point _origContentMouseDownPoint;
        #endregion

        #region constructor and overrides
        /// <summary>
        /// Static constructor to define metadata for the control (and link it to the style in Generic.xaml).
        /// </summary>
        static ZoomAndPanViewBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomAndPanViewBox), new FrameworkPropertyMetadata(typeof(ZoomAndPanViewBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _dragBorder = this.Template.FindName("PART_DraggingBorder", this) as Border;
            _sizingBorder = this.Template.FindName("PART_SizingBorder", this) as Border;
            _viewportCanvas = this.Template.FindName("PART_Content", this) as Canvas;
            SetBackground(Visual);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (ActualWidth > 0 && _viewportCanvas != null)
            {
                _sizingBorder.BorderThickness = _dragBorder.BorderThickness = new Thickness(
                   _viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Left,
                   _viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Top,
                   _viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Right,
                   _viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Bottom);
            }
        }
        #endregion

        #region Mouse Event Handlers
        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            GetZoomAndPanControl().SaveZoom();
            _mouseHandlingMode = MouseHandlingModeEnum.Panning;
            _origContentMouseDownPoint = e.GetPosition(_viewportCanvas);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                // Shift + left- or right-down initiates zooming mode.
                _mouseHandlingMode = MouseHandlingModeEnum.DragZooming;
                _dragBorder.Visibility = Visibility.Hidden;
                _sizingBorder.Visibility = Visibility.Visible;
                Canvas.SetLeft(_sizingBorder, _origContentMouseDownPoint.X);
                Canvas.SetTop(_sizingBorder, _origContentMouseDownPoint.Y);
                _sizingBorder.Width = 0;
                _sizingBorder.Height = 0;
            }
            else
            {
                // Just a plain old left-down initiates panning mode.
                _mouseHandlingMode = MouseHandlingModeEnum.Panning;
            }

            if (_mouseHandlingMode != MouseHandlingModeEnum.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                _viewportCanvas.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_mouseHandlingMode == MouseHandlingModeEnum.DragZooming)
            {
                var zoomAndPanControl = GetZoomAndPanControl();
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rect = ViewportHelpers.Clip(curContentPoint, _origContentMouseDownPoint, new Point(0, 0), 
                    new Point(_viewportCanvas.Width, _viewportCanvas.Height));
                zoomAndPanControl.AnimatedZoomTo(rect);
                _dragBorder.Visibility = Visibility.Visible;
                _sizingBorder.Visibility = Visibility.Hidden;
            }
            _mouseHandlingMode = MouseHandlingModeEnum.None;
            _viewportCanvas.ReleaseMouseCapture();
            e.Handled = true;
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_mouseHandlingMode == MouseHandlingModeEnum.Panning)
            {
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rectangleDragVector = curContentPoint - _origContentMouseDownPoint;
                //
                // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
                //
                _origContentMouseDownPoint = e.GetPosition(_viewportCanvas).Clamp();
                Canvas.SetLeft(_dragBorder, Canvas.GetLeft(_dragBorder) + rectangleDragVector.X);
                Canvas.SetTop(_dragBorder, Canvas.GetTop(_dragBorder) + rectangleDragVector.Y);
            }
            else if (_mouseHandlingMode == MouseHandlingModeEnum.DragZooming)
            {
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rect = ViewportHelpers.Clip(curContentPoint, _origContentMouseDownPoint, new Point(0, 0), new Point(_viewportCanvas.Width, _viewportCanvas.Height));
                ViewportHelpers.PositionBorderOnCanvas(_sizingBorder, rect);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Event raised with the double click command
        /// </summary>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                var zoomAndPanControl = GetZoomAndPanControl();
                zoomAndPanControl.SaveZoom();
                zoomAndPanControl.AnimatedSnapTo(e.GetPosition(_viewportCanvas));
            }
        }
        #endregion

        #region Background--Visual Brush
        /// <summary>
        /// The X coordinate of the content focus, this is the point that we are focusing on when zooming.
        /// </summary>
        public FrameworkElement Visual
        {
            get { return (FrameworkElement)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }
        public static readonly DependencyProperty VisualProperty = DependencyProperty.Register("Visual",
            typeof(FrameworkElement), typeof(ZoomAndPanViewBox), new FrameworkPropertyMetadata(null, OnVisualChanged));

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanViewBox)d;
            c.SetBackground(e.NewValue as FrameworkElement);
        }

        private void SetBackground(FrameworkElement frameworkElement)
        {
            frameworkElement = frameworkElement ?? (DataContext as ContentControl)?.Content as FrameworkElement;
            var visualBrush = new VisualBrush
            {
                Visual = frameworkElement,
                ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
                TileMode = TileMode.None,
                Stretch = Stretch.Fill
            };

            if (frameworkElement != null) frameworkElement.SizeChanged += (s, e) =>
            {
                _viewportCanvas.Height = frameworkElement.ActualHeight;
                _viewportCanvas.Width = frameworkElement.ActualWidth;
                _viewportCanvas.Background = visualBrush;
            };
        }
        #endregion

        private ZoomAndPanControl GetZoomAndPanControl()
        {
            var zoomAndPanControl = (this.DataContext as ZoomAndPanControl) ??
                                    (this.DataContext as ZoomAndPanScrollViewer)?.ZoomAndPanContent;
            if (zoomAndPanControl == null) throw new NullReferenceException("DataContext is not of type ZoomAndPanControl");
            return zoomAndPanControl;
        }
    }
}
