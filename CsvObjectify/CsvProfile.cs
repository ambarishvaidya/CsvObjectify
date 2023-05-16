using CsvObjectify.Column;
using CsvObjectify.Validation;

namespace CsvObjectify
{
    public class CsvProfile
    {
        private CsvProfile() { }
        private CsvProfile(ColumnMetadata[] colMetadata, string filePath, bool isFirstRowHeader, string delimiter)
        {
            FilePath = filePath;
            ColumnMetadata = colMetadata;
            IsFirstRowHeader = isFirstRowHeader;
            Delimiter = delimiter;
        }

        public ColumnMetadata[] ColumnMetadata { get; init; }
        public string FilePath { get; init; }
        public bool IsFirstRowHeader { get; init; }
        public string Delimiter { get; init; }

        public static CsvProfile Build(ColumnMetadata[] colMetadata, string filePath, bool isFirstRowHeader, string delimiter = ",")
        {
            filePath = filePath?.Trim();

            Validate.RaiseExceptionIfStringIsEmptyOrNull(filePath, nameof(filePath));
            UpdateMetaDataWithColumnIndxes(colMetadata, filePath, isFirstRowHeader, delimiter);
            ValidateInput(colMetadata);            

            return new CsvProfile(colMetadata, filePath, isFirstRowHeader, delimiter);            
        }

        private static void UpdateMetaDataWithColumnIndxes(ColumnMetadata[] colMetadata, string filepath, bool isFirstRowHeader, string delimiter)
        {
            if (!colMetadata.Any(defn => defn.ColumnIndex == null))            
                return;
            
            if (!isFirstRowHeader)
                throw new InvalidOperationException($"Column Index missing. Assign Column Index to all ColumnDefinition or " +
                    $"provide csv with header and {nameof(isFirstRowHeader)} variable set to true.");

            string headerLine = GetHeader(filepath);
            if (headerLine.IsEmpty()) throw new InvalidDataException($"Header line is empty in {filepath}.");

            int index = 0;
            Dictionary<string, int> strings = new Dictionary<string, int>();
            foreach (var str in headerLine.Split(delimiter).Select(s => s.Trim()))
            {
                if (strings.ContainsKey(str))
                    throw new InvalidDataException($"Duplicates data [{str}]in Header.");
                strings.Add(str, index++);
            }

            foreach (var defn in colMetadata.Where(indx => indx.ColumnIndex == null))
            {
                if (strings.TryGetValue(defn.ColumnName, out index))
                    defn.ColumnIndex = index;
            }

            var missingColumnNames = colMetadata.Where(defn => defn.ColumnIndex == null).Select(n => n.ColumnName);
            if (missingColumnNames.Any())
                throw new InvalidDataException($"Column names not present in Header. Misssing names [{string.Join(",", missingColumnNames)}]");
        }

        private static string GetHeader(string filepath)
        {
            return File.ReadLines(filepath).First().Trim();
        }

        private static void ValidateInput(ColumnMetadata[] colMetadata)
        {
            if (!colMetadata.Any()) throw new ArgumentException($"Column defintion is mandatory");

            if(colMetadata.Length != colMetadata.Select(n => n.ColumnIndex).Distinct().Count())
                throw new InvalidDataException($"Duplicate Column Indexes.");
        }
    }
}
