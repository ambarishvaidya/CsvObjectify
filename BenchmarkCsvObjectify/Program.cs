﻿using BenchmarkCsvObjectify.Entity;
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
            CreateCsvs(100000);
            //new ResidentBenchmark();
            BenchmarkRunner.Run<ResidentBenchmark>();
        }

        static void CreateCsvs(int count)
        {
            //return;
            ResidentFileCreater.CreateCsvFile(count, @"E:\MyData\MyWork\_Github\CsvObjectify\BenchmarkCsvObjectify\CsvFiles\ResidentsWithHeaderDefaultDelimiter.csv");
        }
    }

    [MemoryDiagnoser]
    public class ResidentBenchmark
    {
        ICsvParser<Resident> _residentParser;
        public ResidentBenchmark()
        {
            BuildResidentParser();
            //ReadResidentSequential();
            //ReadResidentParallel();
            //ReadResidentMemoryParallel();
            ReadResidentChatParallel();
        }

        //[Benchmark]
        public void ReadResidentChatParallel()
        {
            List<Resident> residents = new List<Resident>();
            foreach (var item in _residentParser.ParseParallelMemoryChat())
                residents.Add(item);
            Console.WriteLine($"Total of {residents.Count} in the file.");
        }

        //[Benchmark]
        public void ReadResidentMemoryParallel()
        {
            List<Resident> residents = new List<Resident>();
            foreach (var item in _residentParser.ParseParallelMemory())
                residents.Add(item);
            Console.WriteLine($"Total of {residents.Count} in the file.");
        }

        private void BuildResidentParser()
        {
            _residentParser = CsvParser<Resident>.Build(
                CsvProfile.Build(
                    @".\CsvFiles\ResidentsWithHeaderDefaultDelimiter.csv",
                    Resident.GetMetadata,
                    true));
        }

        [Benchmark]
        public void ReadResidentSequential()
        {
            List<Resident> residents = new List<Resident>();
            foreach (var item in _residentParser.Parse())
                residents.Add(item);
            Console.WriteLine($"Total of {residents.Count} in the file.");
        }

        [Benchmark]
        public void ReadResidentParallel()
        {
            List<Resident> residents = new List<Resident>();
            foreach (var item in _residentParser.Parse())
                residents.Add(item);
            Console.WriteLine($"Total of {residents.Count} in the file.");
        }
    }
}