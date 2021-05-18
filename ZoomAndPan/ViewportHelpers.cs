using System;
using System.Windows;
using System.Windows.Controls;

namespace ZoomAndPan
{
    internal static class ViewportHelpers
    {
        /// <summary>
        /// Limits the extent of a Point to the area where X and Y are at least 0
        /// </summary>
        /// <param name="value">Point to be clamped</param>
        /// <returns></returns>
        public static Point Clamp(this Point value)
        {
            return new Point(Math.Max(value.X, 0), Math.Max(value.Y, 0));
        }

        /// <summary>
        /// Limits the extent of a Point to the area where X and Y are at least 0 and the X and 
        /// y valuses specified, returning null if Point is outside this area
        /// </summary>
        /// <param name="value">Point to be clamped</param>
        /// <param name="xMax">Maximum X value</param>
        /// <param name="yMax">Maximum Y value</param>
        /// <returns></returns>
        public static Point? FilterClamp(this Point value, double xMax, double yMax)
        {
            return (value.X < 0 || value.X > xMax || value.Y < 0 || value.Y > yMax) ? (Point?)null : value;
        }

        /// <summary>
        /// Limits the extent of a Point to the area between two points
        /// </summary>
        /// <param name="value"></param>
        /// <param name="topLeft">Point specifiying the Top Left corner</param>
        /// <param name="bottomRight">Point specifiying the Bottom Right corner</param>
        /// <returns>The Point clamped by the Top Left and Bottom Right points</returns>
        public static Point Clamp(this Point value, Point topLeft, Point bottomRight)
        {
            return new Point(
                Math.Max(Math.Min(value.X, bottomRight.X), topLeft.X),
                Math.Max(Math.Min(value.Y, bottomRight.Y), topLeft.Y));
        }

        /// <summary>
        /// Return a Rect that specificed by two points and clipped by a rectangle specified
        /// by two other points
        /// </summary>
        /// <param name="value1">First Point specifing the rectangle to be clipped</param>
        /// <param name="value2">Second Point specifing the rectangle to be clipped</param>
        /// <param name="topLeft">Point specifiying the Top Left corner of the clipping rectangle</param>
        /// <param name="bottomRight">Point specifiying the Bottom Right corner of the clipping rectangle</param>
        /// <returns>Rectangle specified by two points clipped by the other two points</returns>
        public static Rect Clip(Point value1, Point value2, Point topLeft, Point bottomRight)
        {
            var point1 = Clamp(value1, topLeft, bottomRight);
            var point2 = Clamp(value2, topLeft, bottomRight);
            var newTopLeft = new Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            var size = new Size(Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y));
            return new Rect(newTopLeft, size);
        }

        /// <summary>
        /// Moves and sized a border on a Canvas according to a Rect
        /// </summary>
        /// <param name="border">Border to be moved and sized</param>
        /// <param name="rect">Rect that specifies the size and postion of the Border on the Canvas</param>
        public static void PositionBorderOnCanvas(Border border, Rect rect)
        {
            Canvas.SetLeft(border, rect.Left);
            Canvas.SetTop(border, rect.Top);
            border.Width = rect.Width;
            border.Height = rect.Height;
        }

        public static bool IsWithinOnePercent(this double value, double testValue)
        {
            return Math.Abs(value - testValue) < .01 * testValue;
        }

        public static double FitZoom(double actualWidth, double actualHeight, double? contentWidth, double? contentHeight)
        {
            if (!contentWidth.HasValue || !contentHeight.HasValue) return 1;
            return Math.Min(actualWidth / contentWidth.Value, actualHeight / contentHeight.Value); ;
        }

        public static double FillZoom(double actualWidth, double actualHeight, double? contentWidth, double? contentHeight)
        {
            if (!contentWidth.HasValue || !contentHeight.HasValue) return 1;
            return Math.Max(actualWidth / contentWidth.Value, actualHeight / contentHeight.Value); ;
        }
    }
}
