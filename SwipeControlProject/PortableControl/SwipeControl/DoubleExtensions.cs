namespace SwipeControl
{
    public static class DoubleExtensions
    {
        public static bool IsBetween(this double value, double from, double to)
        {
            return value > from && value < to;
        }
    }
}