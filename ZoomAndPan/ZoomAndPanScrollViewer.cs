using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZoomAndPan
{
    public class ZoomAndPanScrollViewer : ScrollViewer
    {
        // private ZoomAndPanControl _zoomAndPanControl;

        #region constructor and overrides
        /// <summary>
        /// Static constructor to define metadata for the control (and link it to the style in Generic.xaml).
        /// </summary>
        static ZoomAndPanScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomAndPanScrollViewer), new FrameworkPropertyMetadata(typeof(ZoomAndPanScrollViewer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ZoomAndPanContent = this.Template.FindName("PART_ZoomAndPanControl", this) as ZoomAndPanControl;
            OnPropertyChanged(new DependencyPropertyChangedEventArgs(ZoomAndPanContentProperty, null, ZoomAndPanContent));
            RefreshProperties();
        }
        #endregion

        /// <summary>
        /// Get/set the maximum value for 'ViewportZoom'.
        /// </summary>
        public ZoomAndPanControl ZoomAndPanContent
        {
            get { return (ZoomAndPanControl)GetValue(ZoomAndPanContentProperty); }
            set { SetValue(ZoomAndPanContentProperty, value); }
        }
        public static readonly DependencyProperty ZoomAndPanContentProperty = DependencyProperty.Register("ZoomAndPanContent",
            typeof(ZoomAndPanControl), typeof(ZoomAndPanScrollViewer), new FrameworkPropertyMetadata(null));

        #region DependencyProperties

        /// <summary>
        /// Get/set the maximum value for 'ViewportZoom'.
        /// </summary>
        public MinimumZoomTypeEnum MinimumZoomType
        {
            get { return (MinimumZoomTypeEnum)GetValue(MinimumZoomTypeProperty); }
            set { SetValue(MinimumZoomTypeProperty, value); }
        }
        public static readonly DependencyProperty MinimumZoomTypeProperty = DependencyProperty.Register("MinimumZoomType",
            typeof(MinimumZoomTypeEnum), typeof(ZoomAndPanScrollViewer), new FrameworkPropertyMetadata(MinimumZoomTypeEnum.MinimumZoom));

        /// <summary>
        /// Get/set the MinimumZoom value for 'ViewportZoom'.
        /// </summary>
        public Point? MousePosition
        {
            get { return (Point?)GetValue(MousePositionProperty); }
            set { SetValue(MousePositionProperty, value); }
        }
        public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition",
            typeof(Point?), typeof(ZoomAndPanScrollViewer), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Disables animations if set to false
        /// </summary>
        public bool UseAnimations
        {
            get { return (bool)GetValue(UseAnimationsProperty); }
            set { SetValue(UseAnimationsProperty, value); }
        }
        public static readonly DependencyProperty UseAnimationsProperty = DependencyProperty.Register("UseAnimations",
            typeof(bool), typeof(ZoomAndPanScrollViewer), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get/set the current scale (or zoom factor) of the content.
        /// </summary>
        public double ViewportZoom
        {
            get { return (double)GetValue(ViewportZoomProperty); }
            set { SetValue(ViewportZoomProperty, value); }
        }
        public static readonly DependencyProperty ViewportZoomProperty = DependencyProperty.Register("ViewportZoom",
            typeof(double), typeof(ZoomAndPanScrollViewer), new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// The duration of the animations (in seconds) started by calling AnimatedZoomTo and the other animation methods.
        /// </summary>
        public ZoomAndPanInitialPositionEnum ZoomAndPanInitialPosition
        {
            get { return (ZoomAndPanInitialPositionEnum)GetValue(ZoomAndPanInitialPositionProperty); }
            set { SetValue(ZoomAndPanInitialPositionProperty, value); }
        }
        public static readonly DependencyProperty ZoomAndPanInitialPositionProperty = DependencyProperty.Register("ZoomAndPanInitialPosition",
            typeof(ZoomAndPanInitialPositionEnum), typeof(ZoomAndPanScrollViewer), new FrameworkPropertyMetadata(ZoomAndPanInitialPositionEnum.Default));
        #endregion

        #region Commands
        /// <summary>
        ///     Command to implement the zoom to fill 
        /// </summary>
        public ICommand FillCommand => _fillCommand ?? (_fillCommand =
                new RelayCommand(
                    () => ZoomAndPanContent.FillCommand.Execute(null),
                    () => ZoomAndPanContent?.FillCommand.CanExecute(null) ?? true));
        private RelayCommand _fillCommand;

        /// <summary>
        ///     Command to implement the zoom to fit 
        /// </summary>
        public ICommand FitCommand => _fitCommand ?? (_fitCommand =
                new RelayCommand(
                    () => ZoomAndPanContent.FitCommand.Execute(null),
                    () => ZoomAndPanContent?.FitCommand.CanExecute(null) ?? true));
        private RelayCommand _fitCommand;

        /// <summary>
        ///     Command to implement the zoom to 100% 
        /// </summary>
        public ICommand ZoomPercentCommand => _zoomPercentCommand ?? (_zoomPercentCommand =
                new RelayCommand<double>(
                    value => ZoomAndPanContent.ZoomPercentCommand.Execute(value),
                    value => ZoomAndPanContent?.ZoomPercentCommand.CanExecute(value) ?? true));
         private RelayCommand<double> _zoomPercentCommand;

        /// <summary>
        ///     Command to implement the zoom to 100% 
        /// </summary>
        public ICommand ZoomRatioFromMinimumCommand => _zoomRatioFromMinimumCommand ?? (_zoomRatioFromMinimumCommand =
                new RelayCommand<double>(
                    value => ZoomAndPanContent.ZoomRatioFromMinimumCommand.Execute(value),
                    value => ZoomAndPanContent?.ZoomRatioFromMinimumCommand.CanExecute(value) ?? true));
        private RelayCommand<double> _zoomRatioFromMinimumCommand;

        /// <summary>
        ///     Command to implement the zoom out by 110% 
        /// </summary>
        public ICommand ZoomOutCommand => _zoomOutCommand ?? (_zoomOutCommand =
                new RelayCommand(
                    () => ZoomAndPanContent.ZoomOutCommand.Execute(null),
                    () => ZoomAndPanContent?.ZoomOutCommand.CanExecute(null) ?? true));
        private RelayCommand _zoomOutCommand;

        /// <summary>
        ///     Command to implement the zoom in by 91% 
        /// </summary>
        public ICommand ZoomInCommand => _zoomInCommand ?? (_zoomInCommand =
                new RelayCommand(
                    () => ZoomAndPanContent.ZoomInCommand.Execute(null),
                    () => ZoomAndPanContent?.ZoomInCommand.CanExecute(null) ?? true));
        private RelayCommand _zoomInCommand;

        /// <summary>
        ///     Command to implement Undo 
        /// </summary>
        public ICommand UndoZoomCommand => _undoZoomCommand ?? (_undoZoomCommand =
                new RelayCommand(
                    () => ZoomAndPanContent.UndoZoomCommand.Execute(null),
                    () => ZoomAndPanContent?.UndoZoomCommand.CanExecute(null) ?? true));
        private RelayCommand _undoZoomCommand;

        /// <summary>
        ///     Command to implement Redo 
        /// </summary>
        public ICommand RedoZoomCommand => _redoZoomCommand ?? (_redoZoomCommand =
                new RelayCommand(
                    () => ZoomAndPanContent.RedoZoomCommand.Execute(null),
                    () => ZoomAndPanContent?.RedoZoomCommand.CanExecute(null) ?? true));
        private RelayCommand _redoZoomCommand;
        #endregion

        private void RefreshProperties()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FillCommand)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FitCommand)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomPercentCommand)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomRatioFromMinimumCommand)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomInCommand)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ZoomOutCommand)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UndoZoomCommand)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RedoZoomCommand)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
