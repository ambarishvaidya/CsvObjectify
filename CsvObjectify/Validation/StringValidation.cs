namespace CsvObjectify.Validation
{
    public static class StringExtension
    {
        public static bool IsEmpty(this string value) => value.Equals(string.Empty);
        public static bool IsNull(this string value) => value == null;
    }
}
