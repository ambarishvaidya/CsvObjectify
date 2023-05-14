using System.Collections;

namespace CsvObjectify
{
    public interface ICsvParser<T> where T : class, new()
    {
        IEnumerable<T> Parse();
        IEnumerable<T> ParseParallel();
    }
}
