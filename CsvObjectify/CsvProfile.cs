using CsvObjectify.Column;
using CsvObjectify.Validation;

namespace CsvObjectify
{
    public class FileDetails
    {
        public string FilePath { get; set; }
        public string Delimiter { get; set; } = ",";
        public bool IsFirstRowHeader { get; set; }
    }

    public class CsvProfile
    {
        private CsvProfile() { }
        private CsvProfile(ColumnMetadata[] colMetadata, FileDetails fileDetails)
        {   
            ColumnMetadata = colMetadata;
            FileDetails = fileDetails;
        }

        public ColumnMetadata[] ColumnMetadata { get; init; }
        public FileDetails FileDetails { get; init; }        

        public static CsvProfile Build(ColumnMetadata[] colMetadata, FileDetails fileDetails)
        {
            fileDetails.FilePath = fileDetails.FilePath?.Trim();

            Validate.RaiseExceptionIfStringIsEmptyOrNull(fileDetails.FilePath, nameof(fileDetails.FilePath));
            UpdateMetaDataWithColumnIndxes(colMetadata, fileDetails);
            ValidateInput(colMetadata);            

            return new CsvProfile(colMetadata, fileDetails);            
        }

        private static void UpdateMetaDataWithColumnIndxes(ColumnMetadata[] colMetadata, FileDetails fileDetails)
        {
            if (!colMetadata.Any(defn => defn.ColumnIndex == null))            
                return;
            
            if (!fileDetails.IsFirstRowHeader)
                throw new InvalidOperationException($"Column Index missing. Assign Column Index to all ColumnDefinition or " +
                    $"provide csv with header and {nameof(fileDetails.IsFirstRowHeader)} variable set to true.");

            string headerLine = GetHeader(fileDetails.FilePath);
            if (headerLine.IsEmpty()) throw new InvalidDataException($"Header line is empty in {fileDetails.FilePath}.");

            int index = 0;
            Dictionary<string, int> strings = new Dictionary<string, int>();
            foreach (var str in headerLine.Split(fileDetails.Delimiter).Select(s => s.Trim()))
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
