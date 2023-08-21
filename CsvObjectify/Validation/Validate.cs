using System.Reflection.Metadata.Ecma335;

namespace CsvObjectify.Validation
{
    public static class Validate
    {
        public static void RaiseExceptionIfStringIsEmptyOrNull(string data, string inputName)
        {
            RaiseArgumentExceptionIfStringIsNull(data, inputName);
            RaiseArgumentExceptionIfStringIsEmpty(data, inputName);
        }
        public static void RaiseArgumentExceptionIfStringIsEmpty(string data, string inputName)
        {
            if (data.IsEmpty())
            {
                CsvParserLog.Error?.Invoke($"{inputName} cannot be empty.");
                throw new ArgumentException($"{inputName} cannot be empty.");
            }
        }
        public static void RaiseArgumentExceptionIfStringIsNull(string data, string inputName)
        {
            if (data.IsNull())
            {
                CsvParserLog.Error?.Invoke($"{inputName} cannot be null.");
                throw new ArgumentNullException($"{inputName} cannot be null.");
            }
        }

        public static void RaiseArgumentExceptionForInvalidIndex(int index)
        {
            if (index < 0)
            {
                CsvParserLog.Error?.Invoke("Column Index cannot be < 0.");
                throw new ArgumentException($"Column Index cannot be < 0.");
            }
        }
    }
}
