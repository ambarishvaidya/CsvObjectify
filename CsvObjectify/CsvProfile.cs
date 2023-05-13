using CsvObjectify.Column;
using CsvObjectify.Validation;

namespace CsvObjectify
{
    public class CsvProfile
    {
        private CsvProfile() { }
        private CsvProfile(string filePath, ColumnMetadata[] colMetadata, bool isFirstRowHeader)
        {
            FilePath = filePath;
            ColumnMetadata = colMetadata;
            IsFirstRowHeader = isFirstRowHeader;
        }

        public ColumnMetadata[] ColumnMetadata { get; init; }
        public string FilePath { get; init; }
        public bool IsFirstRowHeader { get; init; }

        public static CsvProfile Build(string filePath, ColumnMetadata[] colMetadata, bool isFirstRowHeader)
        {
            filePath = filePath?.Trim();

            Validate.RaiseExceptionIfStringIsEmptyOrNull(filePath, nameof(filePath));
            UpdateMetaDataWithColumnIndxes(filePath, colMetadata, isFirstRowHeader);
            ValidateInput(colMetadata);            

            return new CsvProfile(filePath, colMetadata, isFirstRowHeader);            
        }

        private static void UpdateMetaDataWithColumnIndxes(string filepath, ColumnMetadata[] colMetadata, bool isFirstRowHeader)
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
            foreach (var str in headerLine.Split(",").Select(s => s.Trim()))
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
