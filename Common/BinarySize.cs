namespace Common
{
    public static class BinarySize
    {
        private const int Kilo = 1024;

        public static class InBytes
        {
            public const long KB = Kilo;
            public const long MB = KB * Kilo;
            public const long GB = MB * Kilo;
        }
    }
}
