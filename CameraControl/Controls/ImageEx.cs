using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CameraControl.Controls
{
    public class ImageEx : Image
    {
        public static readonly RoutedEvent SourceChangedEvent = EventManager.RegisterRoutedEvent(
            "SourceChanged", RoutingStrategy.Direct, typeof (RoutedEventHandler), typeof (ImageEx));

        static ImageEx()
        {
            Image.SourceProperty.OverrideMetadata(typeof (ImageEx), new FrameworkPropertyMetadata(SourcePropertyChanged));
        }

        public event RoutedEventHandler SourceChanged
        {
            add { AddHandler(SourceChangedEvent, value); }
            remove { RemoveHandler(SourceChangedEvent, value); }
        }

        private static void SourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Image image = obj as Image;
            if (image != null)
            {
                image.RaiseEvent(new RoutedEventArgs(SourceChangedEvent));
            }
        }
    }
}
