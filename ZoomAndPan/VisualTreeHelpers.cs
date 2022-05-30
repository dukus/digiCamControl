using System.Windows;
using System.Windows.Media;

namespace ZoomAndPan
{
    public static class VisualTreeHelpers
    {
        /// <summary>
        /// Find first paretn of type T in VisualTree.
        /// </summary>
        public static T FindParentControl<T>(this DependencyObject control) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(control);
            while (parent != null && !(parent is T))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as T;
        }

        /// <summary>
        /// Find first child of type T in VisualTree.
        /// </summary>
        public static T FindChildControl<T>(this DependencyObject control) where T : DependencyObject
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (var i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                return (child is T)
                    ? (T)child : FindChildControl<T>(child);
            }
            return null;
        }
    }
}
