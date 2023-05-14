using CsvObjectify;
using CsvObjectify.Column;
using NUnit.Framework;

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

        [Test]
        public void Parse_WithValidEmployeeInputDifferentValidatorNoHeader_CheckForCompleteAddress()
        {
            var employeeWithHeaderPath = @".\TestFiles\EmployeeWithoutHeaderHashDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                new ColumnDefinition<string>(0, s => s.Trim(),"FirstName"),
                new ColumnDefinition<string>(1, s => s.Trim(), "MiddleName"),
                new ColumnDefinition<string>(2, s => s.Trim(), "LastName"),
                new ColumnDefinition<int>(3, s => int.Parse(s.Trim()), "Age"),
                new ColumnDefinition<string>(4, s => s.Trim(), "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeWithHeaderPath, employeeColumnMetadata, false, "#");
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            Employee employee = csvParser.Parse().First();
            Assert.That(employee.Address == "123 Main Street, Apt 4B, City");
        }

        [TestCase("Michaela","", "Thompson", 23, @"345 Pinecrest Avenue, ""Floor 3, Suite 10"", Hamlet")]
        [TestCase("John", "", "Doe", 20, @"123 Main Street, Apt 4B, City")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough")]
        [TestCase("John", "", "O'Connor", 24, @"789 Main Street, Apt 1, City")]
        public void Parse_WithValidEmployeeInputDifferentValidatorNoHeader_ConfirmEmployeeExists(
            string firstname, string middlename, string lastname, int age, string address )
        {
            Employee record = new Employee() {
                FirstName = firstname,
                MiddleName = middlename, 
                LastName = lastname, 
                Age = age, 
                Address = address };

            var employeeWithHeaderPath = @".\TestFiles\EmployeeWithoutHeaderHashDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                new ColumnDefinition<string>(0, s => s.Trim(),"FirstName"),
                new ColumnDefinition<string>(1, s => s.Trim(), "MiddleName"),
                new ColumnDefinition<string>(2, s => s.Trim(), "LastName"),
                new ColumnDefinition<int>(3, s => int.Parse(s.Trim()), "Age"),
                new ColumnDefinition<string>(4, s => s.Trim(), "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeWithHeaderPath, employeeColumnMetadata, false, "#");
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach(Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            Assert.IsTrue(hasEmployee);
        }
    }

    public class Student
    {
        public int Rollnumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DOB { get; set; }
    }

    public class Employee : IEquatable<Employee>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Employee);
        }

        public bool Equals(Employee other)
        {
            return other is not null &&
                   FirstName == other.FirstName &&
                   MiddleName == other.MiddleName &&
                   LastName == other.LastName &&
                   Age == other.Age &&
                   Address == other.Address;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, MiddleName, LastName, Age, Address);
        }

        public static bool operator ==(Employee left, Employee right)
        {
            return EqualityComparer<Employee>.Default.Equals(left, right);
        }

        public static bool operator !=(Employee left, Employee right)
        {
            return !(left == right);
        }
    }    
}