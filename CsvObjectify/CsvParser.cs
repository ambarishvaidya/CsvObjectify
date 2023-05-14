using CsvObjectify.Column;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;

namespace CsvObjectify
{
    public class CsvParser<T> : ICsvParser<T> where T : class, new()
    {
        private Dictionary<int, Mappings> _mappings;
        private CsvProfile _profile;
        private CsvParser() { }
        private CsvParser(Dictionary<int, Mappings> mappings, CsvProfile csvProfile)
        {
            _mappings = mappings;
            _profile = csvProfile;
        }

        public static ICsvParser<T> Build(CsvProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException($"CsvProfile cannot be null.");

            var dictionaryMappings = BuildMappings(profile);
            return new CsvParser<T>(dictionaryMappings, profile);
        }

        private static Dictionary<int, Mappings> BuildMappings(CsvProfile profile)
        {
            var dictMapping = new Dictionary<int, Mappings>();

            var tObj = new T();
            PropertyInfo[] tProperties = tObj.GetType().GetProperties();
            var tPropertyNameTypeMap = tProperties.ToDictionary(p => p.Name, p => (p.PropertyType, p));

            foreach(var metadata in profile.ColumnMetadata)
            {
                (Type, PropertyInfo) tTypeTuple;
                if (!tPropertyNameTypeMap.TryGetValue(metadata.PropertyName, out tTypeTuple)) continue;

                Type colType = metadata.GetColumnType();
                if (tTypeTuple.Item1 != colType)
                    throw new InvalidOperationException($"Type of {metadata.ColumnIndex} with property name " +
                        $"{metadata.PropertyName} does not match to the entity type. {colType} != {tTypeTuple.Item1}");

                var typeOfColumnDefn = typeof(ColumnDefinition<>).MakeGenericType(new Type[] { colType });
                var columnDefnInstance = Activator.CreateInstance(typeOfColumnDefn, true) ;
                columnDefnInstance = metadata;
                Type instanceType = metadata.GetType();
                MethodInfo methodInfo = instanceType.GetMethod("GetCellData");

                dictMapping.Add(metadata.ColumnIndex.Value, new Mappings()
                {
                    PropertyName = metadata.PropertyName,
                    CellDataMethodInfo = methodInfo,
                    ColumnDefnInstance = columnDefnInstance
                });
            }

            return dictMapping;
        }        

        public IEnumerable<T> Parse()
        {
            bool ignoreFirstLine = _profile.IsFirstRowHeader;
            using (TextFieldParser reader = new TextFieldParser(_profile.FilePath))
            {
                reader.Delimiters = new string[] { _profile.Delimiter };
                reader.HasFieldsEnclosedInQuotes = true;

                if (ignoreFirstLine)                
                    reader.ReadLine();                

                while (!reader.EndOfData)
                {   
                    string[] lineData = reader.ReadFields();

                    T tObj = new T();
                    foreach (var kvp in _mappings)
                    {
                        string data = lineData[kvp.Key];
                        //call the method in mappings to parse the data at kvpindex
                        object parsedData = kvp.Value.CellDataMethodInfo.Invoke(kvp.Value.ColumnDefnInstance, new object[] { data });
                        //from the property assign it to tObj
                        tObj.GetType().InvokeMember(kvp.Value.PropertyName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                        Type.DefaultBinder, tObj, new object[] { parsedData });
                    }

                    yield return tObj;
                }
            }
        }
    }

    internal class Mappings
    {
        public string PropertyName { get; init; }
        public MethodInfo CellDataMethodInfo { get; init; }
        public  object ColumnDefnInstance { get; init; }
    }
}
