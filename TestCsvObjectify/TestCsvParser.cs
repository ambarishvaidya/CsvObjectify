using CsvObjectify;
using CsvObjectify.Column;

namespace TestCsvObjectify
{
    internal class TestCsvParser
    {
        ColumnMetadata[] _studentColumnMetadata, _employeeColumnMetadata;
        string _studentWithHeaderPath, _employeeWithHeaderPath;
        CsvProfile _studentProfile, _employeeProfile;

        [SetUp]
        public void Setup()
        {
            _studentWithHeaderPath = @".\TestFiles\StudentWithHeader.csv";
            _studentColumnMetadata = new ColumnMetadata[]
                {
                    new ColumnDefinition<int>("RollNo", s => int.Parse(s), "Rollnumber"),
                    new ColumnDefinition<string>("FirstName", s => s),
                    new ColumnDefinition<DateOnly>(4, s => DateOnly.ParseExact(s, "dd-MMMM-yyyy"), "DOB")
                };
            _studentProfile = CsvProfile.Build(_studentWithHeaderPath, _studentColumnMetadata, true);

            _employeeWithHeaderPath = @".\TestFiles\EmployeeWithHeader.csv";
            _employeeColumnMetadata = new ColumnMetadata[]
            {
                new ColumnDefinition<string>("First Name", s => s.Trim()),
                new ColumnDefinition<string>(1, s => s.Trim(), "Middle Name"),
                new ColumnDefinition<string>(2, s => s.Trim(), "Last Name"),
                new ColumnDefinition<int>("Age", s => int.Parse(s.Trim()), "Age"),
                new ColumnDefinition<string>("Address", s => s.Trim()),
            };
            _employeeProfile = CsvProfile.Build(_employeeWithHeaderPath, _employeeColumnMetadata, true);
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
            ICsvParser<Student> csvParser = CsvParser<Student>.Build(_studentProfile);
            Assert.IsNotNull(csvParser);
        }

        [Test]
        public void Parse_WithValidStudentInput_ReturnsData()
        {
            ICsvParser<Student> csvParser = CsvParser<Student>.Build(_studentProfile);
            Student student = csvParser.Parse().First();
            Assert.IsNotNull(student);
        }

        [Test]
        public void Parse_WithValidEmployeeInput_ReturnsData()
        {
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(_studentProfile);
            Employee employee = csvParser.Parse().First();
            Assert.IsNotNull(employee);
        }

        [Test]
        public void Parse_WithValidEmployeeInput_CheckForCompleteAddress()
        {
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(_employeeProfile);
            Employee employee = csvParser.Parse().First();
            Assert.That(employee.Address == "123 Main Street, Apt 4B, City");
        }
    }

    public class Student
    {
        public int Rollnumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DOB { get; set; }
    }

    public class Employee
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
}