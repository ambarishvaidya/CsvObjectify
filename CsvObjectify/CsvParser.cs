using CsvObjectify.Column;
using Microsoft.VisualBasic.FileIO;
using System.Linq.Expressions;
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
                var propertyInfo = typeof(T).GetProperty(metadata.PropertyName);
                Action<object, object> valSetter = GetPropertySetter(propertyInfo);
                dictMapping.Add(metadata.ColumnIndex.Value, new Mappings()
                {
                    PropertyName = metadata.PropertyName,
                    CellDataMethodInfo = methodInfo,
                    ColumnDefnInstance = columnDefnInstance,
                    EntityPropertyInfo = propertyInfo,
                    ValueSetter = valSetter
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

        public IEnumerable<T> ParseParallel()
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
                    Parallel.ForEach(_mappings, kvp => 
                    {
                        string data = lineData[kvp.Key];
                        //call the method in mappings to parse the data at kvpindex
                        object parsedData = kvp.Value.CellDataMethodInfo.Invoke(kvp.Value.ColumnDefnInstance, new object[] { data });
                        //from the property assign it to tObj
                        tObj.GetType().InvokeMember(kvp.Value.PropertyName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                        Type.DefaultBinder, tObj, new object[] { parsedData });
                    });
                    
                    yield return tObj;
                }
            }
        }

        public IEnumerable<T> ParseParallelMemory()
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
                    Memory<string> lineData = reader.ReadFields().AsMemory<string>();

                    T tObj = new T();
                    Parallel.ForEach(_mappings, kvp =>
                    {
                        string data = lineData.Span[kvp.Key];
                        //call the method in mappings to parse the data at kvpindex
                        object parsedData = kvp.Value.CellDataMethodInfo.Invoke(kvp.Value.ColumnDefnInstance, new object[] { data });
                        //from the property assign it to tObj
                        tObj.GetType().InvokeMember(kvp.Value.PropertyName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                        Type.DefaultBinder, tObj, new object[] { parsedData });
                    });

                    yield return tObj;
                }
            }
        }

        public IEnumerable<T> ParseParallelMemoryChat()
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
                    Memory<string> lineData = reader.ReadFields().AsMemory<string>();

                    T tObj = new T();
                    Parallel.For(0, _mappings.Count, (index, state) =>
                    {
                        var kvp = _mappings.ElementAt(index);
                        string data = lineData.Span[kvp.Key];
                        object parsedData = kvp.Value.CellDataMethodInfo.Invoke(kvp.Value.ColumnDefnInstance, new object[] { data });

                        // Use compiled expression to set property dynamically
                        //var propertyInfo = typeof(T).GetProperty(kvp.Value.PropertyName);
                        //var valueSetter = GetPropertySetter(propertyInfo);
                        kvp.Value.ValueSetter(tObj, parsedData);
                    });

                    yield return tObj;
                }
            }
        }

        private static Action<object, object> GetPropertySetter(PropertyInfo propertyInfo)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            var instanceCast = Expression.Convert(instance, propertyInfo.DeclaringType);
            var valueCast = Expression.Convert(value, propertyInfo.PropertyType);

            var call = Expression.Call(instanceCast, propertyInfo.GetSetMethod(), valueCast);

            return Expression.Lambda<Action<object, object>>(call, instance, value).Compile();
        }

    }

    internal class Mappings
    {
        public string PropertyName { get; init; }
        public MethodInfo CellDataMethodInfo { get; init; }
        public object ColumnDefnInstance { get; init; }
        public Action<object, object> ValueSetter { get; init; }
        public PropertyInfo EntityPropertyInfo { get; init; }
    }
}
