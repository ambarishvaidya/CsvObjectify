namespace CsvObjectify
{
    public interface ICsvParser<T> where T : class, new()
    {
        IEnumerable<T> Parse();
        IEnumerable<T> ParseParallel();
        IEnumerable<T> ParseParallelMemory();
        IEnumerable<T> ParseParallelMemoryChat();
    }
}
