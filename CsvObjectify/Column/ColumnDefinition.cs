using CsvObjectify.Validation;
using System;

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
            CsvParserLog.Info?.Invoke(LogColumnDefinition());
        }

        public ColumnDefinition(int columnIndex, Func<string, T> csvToEntityProperty, string entityPropertyName)            
        {
            Validate.RaiseArgumentExceptionForInvalidIndex(columnIndex);
            ColumnIndex = columnIndex;
            CsvToEntityProperty = csvToEntityProperty;
            PropertyName = entityPropertyName?.Trim();

            ValidateInput();
            CsvParserLog.Info?.Invoke(LogColumnDefinition());
        }

        private void ValidateInput()
        {
            Validate.RaiseExceptionIfStringIsEmptyOrNull(PropertyName, nameof(PropertyName));
            if (CsvToEntityProperty == null)
            {
                CsvParserLog.Error?.Invoke("CsvToEntityProperty is a mandatory field and cannot be null.");
                throw new ArgumentNullException($"CsvToEntityProperty is a mandatory field and cannot be null.");
            }
        }
                
        private Func<string, T> CsvToEntityProperty { get; init; }

        public T GetCellData(string s) => CsvToEntityProperty(s);
        public override Type GetColumnType()
        {
            return typeof(T);
        }
    }
}
