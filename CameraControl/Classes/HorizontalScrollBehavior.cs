using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CameraControl.Classes
{
    public class HorizontalScrollBehavior : Behavior<ItemsControl>
    {
        /// <summary>
        /// A reference to the internal ScrollViewer.
        /// </summary>
        private ScrollViewer ScrollViewer { get; set; }

        /// <summary>
        /// By default, scrolling down on the wheel translates to right, and up to left.
        /// Set this to true to invert that translation.
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// The ScrollViewer is not available in the visual tree until the control is loaded.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= OnLoaded;

            ScrollViewer = VisualTreeHelpers.FindVisualChild<ScrollViewer>(AssociatedObject);

            if (ScrollViewer != null)
            {
                ScrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (ScrollViewer != null)
            {
                ScrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var newOffset = !IsInverted ?
                ScrollViewer.HorizontalOffset + e.Delta/100 :
                ScrollViewer.HorizontalOffset - e.Delta/100;

            ScrollViewer.ScrollToHorizontalOffset(newOffset);
        }
    }
}
