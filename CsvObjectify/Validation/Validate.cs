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
            if(data.IsEmpty())
                throw new ArgumentException($"{inputName} cannot be empty.");
        }
        public static void RaiseArgumentExceptionIfStringIsNull(string data, string inputName)
        {
            if (data.IsNull())
                throw new ArgumentNullException($"{inputName} cannot be null.");
        }
    }
}
