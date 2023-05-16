using CsvObjectify.Validation;

namespace CsvObjectify.Column
{
    public class ColumnDefinition<T> : ColumnMetadata
    {
        internal ColumnDefinition() { }
        public ColumnDefinition(string columnName, Func<string, T> csvToEntityProperty)
            : this(columnName, csvToEntityProperty, columnName)
        { }

        public ColumnDefinition(string columnName, Func<string, T> csvToEntityProperty, string entityPropertyName)
        { 
            ColumnName = columnName?.Trim();
            CsvToEntityProperty = csvToEntityProperty;
            PropertyName = entityPropertyName?.Trim();

            Validate.RaiseExceptionIfStringIsEmptyOrNull(ColumnName, nameof(ColumnName));            

            ValidateInput();
        }

        public ColumnDefinition(int columnIndex, Func<string, T> csvToEntityProperty, string entityPropertyName)            
        {
            if (columnIndex < 0)
                throw new ArgumentException($"Column index cannot be less than 0.");
            ColumnIndex = columnIndex;
            CsvToEntityProperty = csvToEntityProperty;
            PropertyName = entityPropertyName?.Trim();

            ValidateInput();
        }

        private void ValidateInput()
        {
            Validate.RaiseExceptionIfStringIsEmptyOrNull(PropertyName, nameof(PropertyName));
            if (CsvToEntityProperty == null)
                throw new ArgumentNullException($"CsvToEntityProperty is a mandatory field and cannot be null.");
        }
                
        private Func<string, T> CsvToEntityProperty { get; init; }

        public T GetCellData(string s) => CsvToEntityProperty(s);
        public override Type GetColumnType()
        {
            return typeof(T);
        }
    }
}
