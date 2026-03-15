using CsvObjectify.Column;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;
using System.Text;

namespace CsvObjectify
{
    public class CsvParser<T> : ICsvParser<T> where T : class, new()
    {
        private SortedDictionary<int, Mappings> _mappings;
        private CsvProfile _profile;
        private CsvParser() { }
        private CsvParser(SortedDictionary<int, Mappings> mappings, CsvProfile csvProfile)
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

        private static SortedDictionary<int, Mappings> BuildMappings(CsvProfile profile)
        {
            var dictMapping = new SortedDictionary<int, Mappings>();

            var tObj = new T();
            PropertyInfo[] tProperties = tObj.GetType().GetProperties();
            var tPropertyNameTypeMap = tProperties.ToDictionary(p => p.Name, p => (p.PropertyType, p));

            foreach (var metadata in profile.ColumnMetadata)
            {
                (Type, PropertyInfo) tTypeTuple;
                if (!tPropertyNameTypeMap.TryGetValue(metadata.PropertyName, out tTypeTuple)) continue;

                Type colType = metadata.GetColumnType();
                if (tTypeTuple.Item1 != colType)
                    throw new InvalidOperationException($"Type of {metadata.ColumnIndex} with property name " +
                        $"{metadata.PropertyName} does not match to the entity type. {colType} != {tTypeTuple.Item1}");

                var typeOfColumnDefn = typeof(ColumnDefinition<>).MakeGenericType(new Type[] { colType });
                var columnDefnInstance = Activator.CreateInstance(typeOfColumnDefn, true);
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
            bool ignoreFirstLine = _profile.FileDetails.IsFirstRowHeader;
            using (StreamReader reader = new StreamReader(_profile.FileDetails.FilePath))
            {
                if (ignoreFirstLine)
                    reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    ReadOnlySpan<char> lineData = reader.ReadLine().AsSpan();
                    if (lineData.Length < 1) continue;
                    T tObj = new T();

                    try
                    {
                        int sliceFrom = 0;
                        int counter = 0;
                        int sliceTo = -1;
                        int indexCounter = -1;
                        foreach (var kvp in _mappings)
                        {
                            sliceTo = kvp.Key;
                            sliceFrom = kvp.Key - 1;

                            bool quotedData = false;

                            while (indexCounter != sliceFrom)
                            {
                                if (lineData[counter] == '"')
                                {
                                    quotedData = !quotedData;
                                }
                                if (lineData[counter] == _profile.FileDetails.Delimiter && !quotedData)
                                    indexCounter++;
                                counter++;
                            }
                            sliceFrom = counter - 1;

                            quotedData = false;
                            while (indexCounter != sliceTo)
                            {
                                if (lineData[counter] == '"')
                                {
                                    quotedData = !quotedData;
                                }
                                if (lineData[counter] == _profile.FileDetails.Delimiter && !quotedData)
                                    indexCounter++;
                                counter++;
                            }
                            sliceTo = counter - 1;

                            var dataspan = sliceFrom < 0 ? lineData.Slice(0, sliceTo) : lineData.Slice(sliceFrom + 1, sliceTo - sliceFrom - 1);

                            //call the method in mappings to parse the data at kvpindex
                            object parsedData = kvp.Value.CellDataMethodInfo.Invoke(kvp.Value.ColumnDefnInstance, [Unescape(dataspan)]);
                            //from the property assign it to tObj
                            tObj.GetType().InvokeMember(kvp.Value.PropertyName,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                            Type.DefaultBinder, tObj, new object[] { parsedData });
                        }
                    }
                    catch
                    {
                        string logMessage = $"Missing or incorrect items for condifured columns in row with data : {lineData.ToString()}";
                    }
                    
                    yield return tObj;

                }
            }
        }

        private string Unescape(ReadOnlySpan<char> data)
        {
            if (data.Length > 1 && data[0] == '"' && data[^1] == '"')
                data = data.Slice(1, data.Length - 2);

            StringBuilder stringBuilder = new StringBuilder();
            for(int counter = 0; counter < data.Length; counter++)
            {
                if (data[counter] == '"' && counter + 1 < data.Length && data[counter + 1] == '"')
                {
                    stringBuilder.Append('"');
                    counter++;
                }
                else
                {
                    stringBuilder.Append(data[counter]);
                }
            }
            return stringBuilder.ToString();
        }
    }

    internal class Mappings
    {
        public string PropertyName { get; init; }
        public MethodInfo CellDataMethodInfo { get; init; }
        public object ColumnDefnInstance { get; init; }
    }
}
