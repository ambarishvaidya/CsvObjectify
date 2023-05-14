namespace CsvObjectify.Column.Helper
{
    public static class ColumnDefinitionHelper
    {
        public static ColumnDefinition<string> CreateStringColumn(string columnName)
        {
            return CreateStringColumn(columnName, columnName);
        }
        public static ColumnDefinition<string> CreateStringColumn(string columnName, string propertyName)
        {
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(columnName, nameof(columnName));
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(propertyName, nameof(propertyName));
            return new ColumnDefinition<string>(columnName, s => s.Trim(), propertyName);
        }
        public static ColumnDefinition<string> CreateStringColumn(int index, string propertyName)
        {
            if (index < 0) throw new ArgumentException($"Csv index cannot be < 0.");
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(propertyName, nameof(propertyName));
            return new ColumnDefinition<string>(index, s => s.Trim(), propertyName);
        }

        public static ColumnDefinition<int> CreateIntColumn(string columnName)
        {
            return CreateIntColumn(columnName, columnName);
        }
        public static ColumnDefinition<int> CreateIntColumn(string columnName, string propertyName)
        {
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(columnName, nameof(columnName));
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(propertyName, nameof(propertyName));
            return new ColumnDefinition<int>(columnName, s => int.Parse(s.Trim()), propertyName);
        }
        public static ColumnDefinition<int> CreateIntColumn(int index, string propertyName)
        {
            if (index < 0) throw new ArgumentException($"Csv index cannot be < 0.");
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(propertyName, nameof(propertyName));
            return new ColumnDefinition<int>(index, s => int.Parse(s.Trim()), propertyName);
        }

        public static ColumnDefinition<double> CreateDoubleColumn(string columnName)
        {
            return CreateDoubleColumn(columnName, columnName);
        }
        public static ColumnDefinition<double> CreateDoubleColumn(string columnName, string propertyName)
        {
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(columnName, nameof(columnName));
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(propertyName, nameof(propertyName));
            return new ColumnDefinition<double>(columnName, s => double.Parse(s.Trim()), propertyName);
        }
        public static ColumnDefinition<double> CreateDoubleColumn(int index, string propertyName)
        {
            if (index < 0) throw new ArgumentException($"Csv index cannot be < 0.");
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(propertyName, nameof(propertyName));
            return new ColumnDefinition<double>(index, s => double.Parse(s.Trim()), propertyName);
        }

        public static ColumnDefinition<float> CreateFloatColumn(string columnName)
        {
            return CreateFloatColumn(columnName, columnName);
        }
        public static ColumnDefinition<float> CreateFloatColumn(string columnName, string propertyName)
        {
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(columnName, nameof(columnName));
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(propertyName, nameof(propertyName));
            return new ColumnDefinition<float>(columnName, s => float.Parse(s.Trim()), propertyName);
        }
        public static ColumnDefinition<float> CreateFloatColumn(int index, string propertyName)
        {
            if (index < 0) throw new ArgumentException($"Csv index cannot be < 0.");
            Validation.Validate.RaiseExceptionIfStringIsEmptyOrNull(propertyName, nameof(propertyName));
            return new ColumnDefinition<float>(index, s => float.Parse(s.Trim()), propertyName);
        }
    }
}
