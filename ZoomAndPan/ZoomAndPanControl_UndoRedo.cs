using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZoomAndPan
{
    public partial class ZoomAndPanControl
    {
        private readonly Stack<UndoRedoStackItem> _undoStack = new Stack<UndoRedoStackItem>();
        private readonly Stack<UndoRedoStackItem> _redoStack = new Stack<UndoRedoStackItem>();
        private UndoRedoStackItem _viewportZoomCache;

        /// <summary> 
        ///     Record the previous zoom level, so that we can return to it.
        /// </summary>
        public void SaveZoom()
        {
            _viewportZoomCache = CreateUndoRedoStackItem();
            if (_undoStack.Any() && _viewportZoomCache.Equals(_undoStack.Peek())) return;
            _undoStack.Push(_viewportZoomCache);
            _redoStack.Clear();
            _undoZoomCommand?.RaiseCanExecuteChanged();
            _redoZoomCommand?.RaiseCanExecuteChanged();
        }

        /// <summary> 
        ///  Record the last saved zoom level, so that we can return to it if no activity for 750 milliseconds
        /// </summary>
        public void DelayedSaveZoom750Miliseconds()
        {
            if (_timer750Miliseconds?.Running != true) _viewportZoomCache = CreateUndoRedoStackItem();
            (_timer750Miliseconds ?? (_timer750Miliseconds = new KeepAliveTimer(TimeSpan.FromMilliseconds(740), () =>
            {
                if (_undoStack.Any() && _viewportZoomCache.Equals(_undoStack.Peek())) return;
                _undoStack.Push(_viewportZoomCache);
                _redoStack.Clear();
                _undoZoomCommand?.RaiseCanExecuteChanged();
                _redoZoomCommand?.RaiseCanExecuteChanged();
            }))).Nudge();
        }
        private KeepAliveTimer _timer750Miliseconds;


        /// <summary> 
        ///  Record the last saved zoom level, so that we can return to it if no activity for 1550 milliseconds
        /// </summary>
        public void DelayedSaveZoom1500Miliseconds()
        {
            if (!_timer1500Miliseconds?.Running != true) _viewportZoomCache = CreateUndoRedoStackItem();
            (_timer1500Miliseconds ?? (_timer1500Miliseconds = new KeepAliveTimer(TimeSpan.FromMilliseconds(1500), () =>
            {
                if (_undoStack.Any() && _viewportZoomCache.Equals(_undoStack.Peek())) return;
                _undoStack.Push(_viewportZoomCache);
                _redoStack.Clear();
                _undoZoomCommand?.RaiseCanExecuteChanged();
                _redoZoomCommand?.RaiseCanExecuteChanged();
            }))).Nudge();
        }
        private KeepAliveTimer _timer1500Miliseconds;

        private UndoRedoStackItem CreateUndoRedoStackItem()
        {
            return new UndoRedoStackItem(this.ContentOffsetX, this.ContentOffsetY,
                this.ContentViewportWidth, this.ContentViewportHeight, InternalViewportZoom);
        }

        /// <summary>
        ///     Jump back to the previous zoom level, saving current zoom to Redo Stack.
        /// </summary>
        private void UndoZoom()
        {
            _viewportZoomCache = CreateUndoRedoStackItem();
            if (!_undoStack.Any() || !_viewportZoomCache.Equals(_undoStack.Peek()))
                _redoStack.Push(_viewportZoomCache);
            _viewportZoomCache = _undoStack.Pop();
            this.AnimatedZoomTo(_viewportZoomCache.Zoom, _viewportZoomCache.Rect);
            SetScrollViewerFocus();
            _undoZoomCommand?.RaiseCanExecuteChanged();
            _redoZoomCommand?.RaiseCanExecuteChanged();
        }

        /// <summary>
        ///     Jump back to the most recent zoom level saved on redo stack.
        /// </summary>
        private void RedoZoom()
        {
            _viewportZoomCache = CreateUndoRedoStackItem();
            if (!_redoStack.Any() || !_viewportZoomCache.Equals(_redoStack.Peek()))
                _undoStack.Push(_viewportZoomCache);
            _viewportZoomCache = _redoStack.Pop();
            this.AnimatedZoomTo(_viewportZoomCache.Zoom, _viewportZoomCache.Rect);
            SetScrollViewerFocus();
            _undoZoomCommand?.RaiseCanExecuteChanged();
            _redoZoomCommand?.RaiseCanExecuteChanged();
        }

        private bool CanUndoZoom => _undoStack.Any();
        private bool CanRedoZoom => _redoStack.Any();

        /// <summary>
        ///     Command to implement Undo 
        /// </summary>
        public ICommand UndoZoomCommand => _undoZoomCommand ?? (_undoZoomCommand =
            new RelayCommand(UndoZoom, () => CanUndoZoom));
        private RelayCommand _undoZoomCommand;

        /// <summary>
        ///     Command to implement Redo 
        /// </summary>
        public ICommand RedoZoomCommand => _redoZoomCommand ?? (_redoZoomCommand =
             new RelayCommand(RedoZoom, () => CanRedoZoom));
        private RelayCommand _redoZoomCommand;

        private class UndoRedoStackItem
        {
            public UndoRedoStackItem(Rect rect, double zoom)
            {
                Rect = rect;
                Zoom = zoom;
            }

            public UndoRedoStackItem(double offsetX, double offsetY, double width, double height, double zoom)
            {
                Rect = new Rect(offsetX, offsetY, width, height);
                Zoom = zoom;
            }

            public Rect Rect { get; }
            public double Zoom { get; }

            public override string ToString()
            {
                return $"Rectangle {{{Rect.X},{Rect.X}}}, Zoom {Zoom}";
            }

            public bool Equals(UndoRedoStackItem obj)
            {
                return Zoom.IsWithinOnePercent(obj.Zoom) && Rect.Equals(obj.Rect);
            }
        }

        private void SetScrollViewerFocus()
        {
            var scrollViewer = _content.FindParentControl<ScrollViewer>();
            if (scrollViewer != null)
            {
                Keyboard.Focus(scrollViewer);
                scrollViewer.Focus();
            }
        }
    }
}
