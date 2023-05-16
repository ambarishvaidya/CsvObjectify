using CsvObjectify;
using CsvObjectify.Column;
using CsvObjectify.Column.Helper;

namespace Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            new Program();
        }

        public Program()
        {
            ICsvParser<Resident> residentParser = CsvParser<Resident>.Build
                (
                    CsvProfile.Build(new CsvObjectify.Column.ColumnMetadata[]
                    {
                        ColumnDefinitionHelper.CreateStringColumn("First Name", "FirstName"),
                        new ColumnDefinition<string>("Middle Name", s => s.Trim(), "MiddleName"),
                        new ColumnDefinition<string>(2, s => s.Trim(), "LastName"),
                        new ColumnDefinition<DateTime>(4, s => DateTime.Parse(s), "DateOfBirth"),
                        new ColumnDefinition<char>("Disability", s => ParseDisablity(s), "Disability")
                    },
                    @"E:\MyData\MyWork\_Github\CsvObjectify\BenchmarkCsvObjectify\CsvFiles\ResidentsWithHeaderDefaultDelimiter.csv", true)
                );

            List<Resident> residents = new List<Resident>();
            foreach (var item in residentParser.Parse())
                residents.Add(item);

            foreach(var item in residents)
                Console.WriteLine(item.ToString());

            Console.ReadLine();

        }

        public char ParseDisablity(string b)
        {
            return bool.Parse(b) ? 'Y' : 'F';
        }
    }

    internal class Resident
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Married { get; set; }
        public int Children { get; set; }
        public string Address { get; set; }
        public string Employment { get; set; }
        public char Disability { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} {DateOfBirth} - {Disability}";
        }
    }    
}