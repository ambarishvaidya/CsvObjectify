using BenchmarkCsvObjectify.Entity;
using BenchmarkCsvObjectify.Helpers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CsvObjectify;

namespace BenchmarkCsvObjectify
{
    public class Program
    {
        static void Main(string[] args)
        {
            //CreateCsvs(100);
            ////new ResidentBenchmark();
            //BenchmarkRunner.Run<ResidentBenchmark>();

            //CreateCsvs(100000);
            ////new ResidentBenchmark();
            //BenchmarkRunner.Run<ResidentBenchmark>();

            CreateCsvs(1000000);
            //new ResidentBenchmark();
            BenchmarkRunner.Run<ResidentBenchmark>();            
        }

        static void CreateCsvs(int count)
        {
            //return;
            ResidentFileCreater.CreateCsvFile(count, @"D:\MyData\MyWork\_Github\CsvObjectify\BenchmarkCsvObjectify\CsvFiles\ResidentsWithHeaderDefaultDelimiter.csv");
        }
    }

    [MemoryDiagnoser]
    public class ResidentBenchmark
    {
        ICsvParser<Resident> _residentParser;
        public ResidentBenchmark()
        {
            BuildResidentParser();
            //Parse();
            //ParseWithSpan();
            //ParseWithoutSpan();
        }

        private void BuildResidentParser()
        {
            _residentParser = CsvParser<Resident>.Build(
                CsvProfile.Build(
                    Resident.GetMetadata,
                    new FileDetails()
                    {
                        FilePath = @".\CsvFiles\ResidentsWithHeaderDefaultDelimiter.csv",
                        IsFirstRowHeader = true
                    }));
        }

        [Benchmark]
        public void Parse()
        {
            List<Resident> residents = [.. _residentParser.Parse()];
            Console.WriteLine($"Total of {residents.Count} in the file.");
        }

        [Benchmark]
        public void ParseWithoutSpan()
        {
            List<Resident> residents = [.. _residentParser.ParseWithoutSpan()];
            Console.WriteLine($"Total of {residents.Count} in the file.");
        }

        [Benchmark]
        public void ParseWithSpan()
        {
            List<Resident> residents = [.. _residentParser.ParseWithSpan()];
            Console.WriteLine($"Total of {residents.Count} in the file.");
        }
    }
}