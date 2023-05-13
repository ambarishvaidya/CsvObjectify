using CsvObjectify;
using CsvObjectify.Column;

namespace TestCsvObjectify
{
    internal class TestCsvParser
    {
        ColumnMetadata[] _columnMetadata;
        string _studentWithHeaderPath;
        CsvProfile _profile;

        [SetUp]
        public void Setup()
        {
            _studentWithHeaderPath = @".\TestFiles\StudentWithHeader.csv";
            _columnMetadata = new ColumnMetadata[]
                {
                    new ColumnDefinition<int>("RollNo", s => int.Parse(s), "Rollnumber"),
                    new ColumnDefinition<string>("FirstName", s => s),
                    new ColumnDefinition<DateOnly>(4, s => DateOnly.ParseExact(s, "dd-MMMM-yyyy"), "DOB")
                };
            _profile = CsvProfile.Build(_studentWithHeaderPath, _columnMetadata, true);
        }

        [Test]
        public void Build_WithIncorrectTypeParameters_ThrowsInvalidOperationException()
        {
            _studentWithHeaderPath = @".\TestFiles\StudentWithHeader.csv";
            var columnMetadata = new ColumnMetadata[]
                {
                    new ColumnDefinition<string>("RollNo", s => s, "Rollnumber"),                    
                };
            var profile = CsvProfile.Build(_studentWithHeaderPath, columnMetadata, true);
            Assert.Throws<InvalidOperationException>(() => CsvParser<Student>.Build(profile));            
        }

        [Test]
        public void Build_WithValidInput_ReturnsICsvParser()
        {                        
            ICsvParser<Student> csvParser = CsvParser<Student>.Build(_profile);
            Assert.IsNotNull(csvParser);
        }

        [Test]
        public void Parse_WithValidInput_ReturnsData()
        {
            ICsvParser<Student> csvParser = CsvParser<Student>.Build(_profile);
            Student student = csvParser.Parse().First();
            Assert.IsNotNull(student);
        }
    }

    public class Student
    {
        public int Rollnumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DOB { get; set; }
    }
}