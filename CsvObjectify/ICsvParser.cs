namespace CsvObjectify
{
    public interface ICsvParser<T> where T : class, new()
    {
        IEnumerable<T> Parse();        
    }
}
