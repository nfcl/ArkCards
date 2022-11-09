namespace Tool
{
    public static class Math
    {
        public static int max(int i1, int i2)
        {
            return i1 > i2 ? i1 : i2;
        }

        public static int min(int i1, int i2)
        {
            return i1 < i2 ? i1 : i2;
        }

        public static int abs(int item)
        {
            return item > 0 ? item : -item;
        }
    }
}