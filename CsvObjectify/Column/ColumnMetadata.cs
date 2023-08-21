namespace CsvObjectify.Column
{
    public abstract class ColumnMetadata
    {
        public abstract Type GetColumnType();
        public string ColumnName { get; init; }
        public string PropertyName { get; init; }
        public int? ColumnIndex { get; internal set; }

        internal string LogColumnDefinition()
        {
            return $"ColumnDefinition of {GetColumnType().ToString()} created with csv " +
                $"{(ColumnIndex.HasValue ? ("Index " + ColumnIndex) : ("column name " + ColumnName))} " +
                $"mapped to property {PropertyName}";
        }

        internal string LogColumnDefinitionUsage()
        {
            return $"ColumnDefinition - Type : {GetColumnType().ToString()} | " +
                $"{(ColumnIndex.HasValue ? ("Index : " + ColumnIndex) : ("Column Name : " + ColumnName))} | " +
                $"Entity Property : {PropertyName}";
        }
    }
}
