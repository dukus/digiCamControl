using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZoomAndPan
{
    public partial class ZoomAndPanControl
    {
        private void ZoomAndPanControl_EventHandlers_OnApplyTemplate()
        {
            _partDragZoomBorder = this.Template.FindName("PART_DragZoomBorder", this) as Border;
            _partDragZoomCanvas = this.Template.FindName("PART_DragZoomCanvas", this) as Canvas;
        }

        /// <summary>
        /// The control for creating a zoom border
        /// </summary>
        private Border _partDragZoomBorder;

        /// <summary>
        /// The control for containing a zoom border
        /// </summary>
        private Canvas _partDragZoomCanvas;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingModeEnum _mouseHandlingMode = MouseHandlingModeEnum.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point _origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point _origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton _mouseButtonDown;

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            SaveZoom();
            _content.Focus();
            Keyboard.Focus(_content);

            _mouseButtonDown = e.ChangedButton;
            _origZoomAndPanControlMouseDownPoint = e.GetPosition(this);
            _origContentMouseDownPoint = e.GetPosition(_content);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                _mouseHandlingMode = MouseHandlingModeEnum.Zooming;
            }
            else if (_mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                _mouseHandlingMode = MouseHandlingModeEnum.Panning;
            }

            if (_mouseHandlingMode != MouseHandlingModeEnum.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                this.CaptureMouse();
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (_mouseHandlingMode != MouseHandlingModeEnum.None)
            {
                if (_mouseHandlingMode == MouseHandlingModeEnum.Zooming)
                {
                    if (_mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn(_origContentMouseDownPoint);
                    }
                    else if (_mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut(_origContentMouseDownPoint);
                    }
                }
                else if (_mouseHandlingMode == MouseHandlingModeEnum.DragZooming)
                {
                    var finalContentMousePoint = e.GetPosition(_content);
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    ApplyDragZoomRect(finalContentMousePoint);
                }

                this.ReleaseMouseCapture();
                _mouseHandlingMode = MouseHandlingModeEnum.None;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var oldContentMousePoint = MousePosition;
            var curContentMousePoint = e.GetPosition(_content);
            MousePosition = curContentMousePoint.FilterClamp(_content.ActualWidth - 1, _content.ActualHeight - 1);
            OnPropertyChanged(new DependencyPropertyChangedEventArgs(MousePositionProperty, oldContentMousePoint,
                curContentMousePoint));

            if (_mouseHandlingMode == MouseHandlingModeEnum.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                var dragOffset = curContentMousePoint - _origContentMouseDownPoint;

                this.ContentOffsetX -= dragOffset.X;
                this.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (_mouseHandlingMode == MouseHandlingModeEnum.Zooming)
            {
                var curZoomAndPanControlMousePoint = e.GetPosition(this);
                var dragOffset = curZoomAndPanControlMousePoint - _origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (_mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                     Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    //
                    _mouseHandlingMode = MouseHandlingModeEnum.DragZooming;
                    InitDragZoomRect(_origContentMouseDownPoint, curContentMousePoint);
                }
            }
            else if (_mouseHandlingMode == MouseHandlingModeEnum.DragZooming)
            {
                //
                // When in drag zooming mode continously update the position of the rectangle
                // that the user is dragging out.
                //
                curContentMousePoint = e.GetPosition(this);
                SetDragZoomRect(_origZoomAndPanControlMouseDownPoint, curContentMousePoint);
            }
        }

        /// <summary>
        /// Event raised on mouse wheel moved in the ZoomAndPanControl.
        /// </summary>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            DelayedSaveZoom750Miliseconds();
            e.Handled = true;

            if (e.Delta > 0)
                ZoomIn(e.GetPosition(_content));
            else if (e.Delta < 0)
                ZoomOut(e.GetPosition(_content));
        }

        /// <summary>
        /// Event raised with the double click command
        /// </summary>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                SaveZoom();
                this.AnimatedSnapTo(e.GetPosition(_content));
            }
        }

        #region private Zoom methods

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            this.ZoomAboutPoint(this.InternalViewportZoom * 0.90909090909, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            this.ZoomAboutPoint(this.InternalViewportZoom * 1.1, contentZoomCenter);
        }

        /// <summary>
        /// Initialise the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            _partDragZoomCanvas.Visibility = Visibility.Visible;
            _partDragZoomBorder.Opacity = 1;
            SetDragZoomRect(pt1, pt2);
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            //
            var rect = ViewportHelpers.Clip(pt1, pt2, new Point(0, 0),
                new Point(_partDragZoomCanvas.ActualWidth, _partDragZoomCanvas.ActualHeight));
            ViewportHelpers.PositionBorderOnCanvas(_partDragZoomBorder, rect);
        }

        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect(Point finalContentMousePoint)
        {
            var rect = ViewportHelpers.Clip(finalContentMousePoint, _origContentMouseDownPoint, new Point(0, 0),
                new Point(_partDragZoomCanvas.ActualWidth, _partDragZoomCanvas.ActualHeight));
            this.AnimatedZoomTo(rect);
            // new Rect(contentX, contentY, contentWidth, contentHeight));
            FadeOutDragZoomRect();
        }

        //
        // Fade out the drag zoom rectangle.
        //
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(_partDragZoomBorder, OpacityProperty, 0.0, 0.1,
                delegate { _partDragZoomCanvas.Visibility = Visibility.Collapsed; }, UseAnimations);
        }

        #endregion

        #region Commands

        /// <summary>
        ///     Command to implement the zoom to fill 
        /// </summary>
        public ICommand FillCommand => _fillCommand ?? (_fillCommand = new RelayCommand(() =>
        {
            SaveZoom();
            AnimatedZoomToCentered(FillZoomValue);
            RaiseCanExecuteChanged();
        }, () => !InternalViewportZoom.IsWithinOnePercent(FillZoomValue) && FillZoomValue >= MinimumZoomClamped));

        private RelayCommand _fillCommand;

        /// <summary>
        ///     Command to implement the zoom to fit 
        /// </summary>
        public ICommand FitCommand => _fitCommand ?? (_fitCommand = new RelayCommand(() =>
        {
            SaveZoom();
            AnimatedZoomTo(FitZoomValue);
            RaiseCanExecuteChanged();
        }, () => !InternalViewportZoom.IsWithinOnePercent(FitZoomValue) && FitZoomValue >= MinimumZoomClamped));

        private RelayCommand _fitCommand;

        /// <summary>
        ///     Command to implement the zoom to a percentage where 100 (100%) is the default and 
        ///     shows the image at a zoom where 1 pixel is 1 pixel. Other percentages specified
        ///     with the command parameter. 50 (i.e. 50%) would display 4 times as much of the image
        /// </summary>
        public ICommand ZoomPercentCommand
            => _zoomPercentCommand ?? (_zoomPercentCommand = new RelayCommand<double>(value =>
            {
                SaveZoom();
                var adjustedValue = value == 0 ? 1 : value / 100;
                AnimatedZoomTo(adjustedValue);
                RaiseCanExecuteChanged();
            }, value =>
            {
                var adjustedValue = value == 0 ? 1 : value / 100;
                return !InternalViewportZoom.IsWithinOnePercent(adjustedValue) && adjustedValue >= MinimumZoomClamped;
            }));


        // Math.Abs(InternalViewportZoom - ((value == 0) ? 1.0 : value / 100)) > .01 * InternalViewportZoom 

        private RelayCommand<double> _zoomPercentCommand;

        /// <summary>
        ///     Command to implement the zoom ratio where 1 is is the the specified minimum. 2 make the image twices the size,
        ///     and is the default. Other values are specified with the CommandParameter. 
        /// </summary>
        public ICommand ZoomRatioFromMinimumCommand
            => _zoomRatioFromMinimumCommand ?? (_zoomRatioFromMinimumCommand = new RelayCommand<double>(value =>
            {
                SaveZoom();
                var adjustedValue = (value == 0 ? 2 : value) * MinimumZoomClamped;
                AnimatedZoomTo(adjustedValue);
                RaiseCanExecuteChanged();
            }, value =>
            {
                var adjustedValue = (value == 0 ? 2 : value) * MinimumZoomClamped;
                return !InternalViewportZoom.IsWithinOnePercent(adjustedValue) && adjustedValue >= MinimumZoomClamped;
            }));

        private RelayCommand<double> _zoomRatioFromMinimumCommand;


        /// <summary>
        ///     Command to implement the zoom out by 110% 
        /// </summary>
        public ICommand ZoomOutCommand => _zoomOutCommand ?? (_zoomOutCommand = new RelayCommand(() =>
             {
                 DelayedSaveZoom1500Miliseconds();
                 ZoomOut(new Point(ContentZoomFocusX, ContentZoomFocusY));
             }, () => InternalViewportZoom > MinimumZoomClamped));
        private RelayCommand _zoomOutCommand;

        /// <summary>
        ///     Command to implement the zoom in by 91% 
        /// </summary>
        public ICommand ZoomInCommand => _zoomInCommand ?? (_zoomInCommand = new RelayCommand(() =>
            {
                DelayedSaveZoom1500Miliseconds();
                ZoomIn(new Point(ContentZoomFocusX, ContentZoomFocusY));
            }, () => InternalViewportZoom < MaximumZoom));
        private RelayCommand _zoomInCommand;

        private void RaiseCanExecuteChanged()
        {
            _zoomPercentCommand?.RaiseCanExecuteChanged();
            _zoomOutCommand?.RaiseCanExecuteChanged();
            _zoomInCommand?.RaiseCanExecuteChanged();
            _fitCommand?.RaiseCanExecuteChanged();
            _fillCommand?.RaiseCanExecuteChanged();
        }
        #endregion

        /// <summary>
        /// When content is renewed, set event to set the initial position as specified
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (oldContent != null)
                ((FrameworkElement)oldContent).SizeChanged -= SetZoomAndPanInitialPosition;
            ((FrameworkElement)newContent).SizeChanged += SetZoomAndPanInitialPosition;
        }

        /// <summary>
        /// When content is renewed, set the initial position as specified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetZoomAndPanInitialPosition(object sender, SizeChangedEventArgs e)
        {
            switch (ZoomAndPanInitialPosition)
            {
                case ZoomAndPanInitialPositionEnum.Default:
                    break;
                case ZoomAndPanInitialPositionEnum.FitScreen:
                    InternalViewportZoom = FitZoomValue;
                    break;
                case ZoomAndPanInitialPositionEnum.FillScreen:
                    InternalViewportZoom = FillZoomValue;
                    ContentOffsetX = (_content.ActualWidth - ViewportWidth / InternalViewportZoom) / 2;
                    ContentOffsetY = (_content.ActualHeight - ViewportHeight / InternalViewportZoom) / 2;
                    break;
                case ZoomAndPanInitialPositionEnum.OneHundredPercentCentered:
                    InternalViewportZoom = 1.0;
                    ContentOffsetX = (_content.ActualWidth - ViewportWidth) / 2;
                    ContentOffsetY = (_content.ActualHeight - ViewportHeight) / 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}