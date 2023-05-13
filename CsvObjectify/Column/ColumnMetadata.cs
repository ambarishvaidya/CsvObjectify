namespace CsvObjectify.Column
{
    public abstract class ColumnMetadata
    {
        public abstract Type GetColumnType();
        public string ColumnName { get; init; }
        public string PropertyName { get; init; }
        public int? ColumnIndex { get; set; }
    }
}
