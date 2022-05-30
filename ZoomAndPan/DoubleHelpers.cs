namespace ZoomAndPan
{
    internal static class DoubleHelpers
    {
        public static double ToRealNumber(this double value, double defaultValue = 0)
        {
            return (double.IsInfinity(value) || double.IsNaN(value)) ? defaultValue : value;
        }
    }
}
