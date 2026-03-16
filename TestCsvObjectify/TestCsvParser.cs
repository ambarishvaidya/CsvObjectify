using CsvObjectify;
using CsvObjectify.Column;
using CsvObjectify.Column.Helper;

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
            _studentProfile = CsvProfile.Build(_studentColumnMetadata, 
                new FileDetails() {
                    FilePath = _studentWithHeaderPath, IsFirstRowHeader = true 
                });

            _employeeWithHeaderPath = @".\TestFiles\EmployeeWithHeader.csv";
            _employeeColumnMetadata = new ColumnMetadata[]
            {
                new ColumnDefinition<string>("First Name", s => s.Trim()),
                new ColumnDefinition<string>(1, s => s.Trim(), "Middle Name"),
                new ColumnDefinition<string>(2, s => s.Trim(), "Last Name"),
                new ColumnDefinition<int>("Age", s => int.Parse(s.Trim()), "Age"),
                new ColumnDefinition<string>("Address", s => s.Trim()),
            };
            _employeeProfile = CsvProfile.Build(_employeeColumnMetadata,
                                                new FileDetails() { FilePath = _employeeWithHeaderPath, IsFirstRowHeader = true });
        }

        [Test]
        public void Build_WithIncorrectTypeParameters_ThrowsInvalidOperationException()
        {
            _studentWithHeaderPath = @".\TestFiles\StudentWithHeader.csv";
            var columnMetadata = new ColumnMetadata[]
                {
                    new ColumnDefinition<string>("RollNo", s => s, "Rollnumber"),
                };
            var profile = CsvProfile.Build(columnMetadata, new FileDetails() { FilePath = _studentWithHeaderPath, IsFirstRowHeader = true });
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
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata, new FileDetails()
            {
                FilePath = employeeWithHeaderPath,
                IsFirstRowHeader = false,
                Delimiter = '#'
            });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            Employee employee = csvParser.Parse().First();
            Assert.That(employee.Address == "123 Main Street, Apt 4B, City");
        }

        [TestCase("Michaela", "", "Thompson", 23, @"345 Pinecrest Avenue, ""Floor 3, Suite 10"", Hamlet")]
        [TestCase("John", "", "Doe", 20, @"123 Main Street, Apt 4B, City")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough")]
        [TestCase("John", "", "O'Connor", 24, @"789 Main Street, Apt 1, City")]
        public void Parse_WithValidEmployeeInputDifferentValidatorNoHeader_ConfirmEmployeeExists(
            string firstname, string middlename, string lastname, int age, string address)
        {
            Employee record = new Employee()
            {
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                Age = age,
                Address = address
            };

            var employeeWithHeaderPath = @".\TestFiles\EmployeeWithoutHeaderHashDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                new ColumnDefinition<string>(0, s => s.Trim(),"FirstName"),
                new ColumnDefinition<string>(1, s => s.Trim(), "MiddleName"),
                new ColumnDefinition<string>(2, s => s.Trim(), "LastName"),
                new ColumnDefinition<int>(3, s => int.Parse(s.Trim()), "Age"),
                new ColumnDefinition<string>(4, s => s.Trim(), "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeWithHeaderPath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach (Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            Assert.IsTrue(hasEmployee);
        }

        [TestCase("Michaela", "", "Thompson", 23, @"345 Pinecrest Avenue, ""Floor 3, Suite 10"", Hamlet")]
        [TestCase("John", "", "Doe", 20, @"123 Main Street, Apt 4B, City")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough")]
        [TestCase("John", "", "O'Connor", 24, @"789 Main Street, Apt 1, City")]
        public void Parse_WithValidEmployeeInputDifferentValidatorNoHeaderUsingHelpers_ConfirmEmployeeExists(
            string firstname, string middlename, string lastname, int age, string address)
        {
            Employee record = new Employee()
            {
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                Age = age,
                Address = address
            };

            var employeeWithHeaderPath = @".\TestFiles\EmployeeWithoutHeaderHashDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };
            var employeeProfile = CsvProfile.Build(
                employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeWithHeaderPath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach (Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            Assert.IsTrue(hasEmployee);
        }

        [TestCase("Michaela", "", "Thompson", 23, @"345 Pinecrest Avenue, ""Floor 3, Suite 10"", Hamlet")]
        [TestCase("John", "", "Doe", 20, @"123 Main Street, Apt 4B, City")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough")]
        [TestCase("John", "", "O'Connor", 24, @"789 Main Street, Apt 1, City")]
        public void Parse_WithValidEmployeeWithEmptyLinesInFile_ConfirmEmployeeExists(
            string firstname, string middlename, string lastname, int age, string address)
        {
            Employee record = new Employee()
            {
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                Age = age,
                Address = address
            };

            var employeeWithHeaderPath = @".\TestFiles\EmployeeWithoutHeaderHashDelimiterWithEmptyLines.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeWithHeaderPath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach (Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            Assert.IsTrue(hasEmployee);
        }

        [TestCase("Michaela", "", "Thompson", 23, @"345 Pinecrest Avenue, ""Floor 3, Suite 10"", Hamlet")]
        [TestCase("John", "", "Doe", 20, @"123 Main Street, Apt 4B, City")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough", true)]
        [TestCase("John", "", "O'Connor", 24, @"789 Main Street, Apt 1, City", true)]
        public void Parse_WithValidEmployeeWithMissingFieldsInFile_ConfirmEmployeeExists(
            string firstname, string middlename, string lastname, int age, string address, bool willBeIgnored = false)
        {
            Employee record = new Employee()
            {
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                Age = age,
                Address = address
            };

            var employeeWithHeaderPath = @".\TestFiles\EmployeeWithoutHeaderHashDelimiterMissingRecord.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeWithHeaderPath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach (Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            if(willBeIgnored)
                Assert.IsTrue(!hasEmployee);
            else
                Assert.IsTrue(hasEmployee);
        }

        [TestCase("John", "", "Doe", 20, "123 Main Street, Apt 4B, City")]
        [TestCase("Jane", "", "Smith", 22, "456 Elm Avenue, Unit 8, Town")]
        [TestCase("Michael", "Allen", "Johnson", 21, "789 Oak Lane, Suite 12, Village")]
        [TestCase("Sarah", "", "Williams", 19, "321 Pine Road, Building 5, County")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough")]
        public void Parse_WithNoTrailingDelimiterHashDelimited_ParsesLastFieldCorrectly(
            string firstname, string middlename, string lastname, int age, string address)
        {
            Employee record = new Employee()
            {
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                Age = age,
                Address = address
            };

            var employeeFilePath = @".\TestFiles\EmployeeNoTrailingDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeFilePath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach (Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            Assert.IsTrue(hasEmployee, $"Employee {firstname} {lastname} with address '{address}' should be found");
        }

        [TestCase("John", "", "Doe", 20, "123 Main Street, Apt 4B, City")]
        [TestCase("Jane", "", "Smith", 22, "456 Elm Avenue, Unit 8, Town")]
        [TestCase("Michael", "Allen", "Johnson", 21, "789 Oak Lane, Suite 12, Village")]
        [TestCase("Sarah", "", "Williams", 19, "321 Pine Road, Building 5, County")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough")]
        public void Parse_WithNoTrailingDelimiterCommaDelimited_ParsesLastFieldCorrectly(
            string firstname, string middlename, string lastname, int age, string address)
        {
            Employee record = new Employee()
            {
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                Age = age,
                Address = address
            };

            var employeeFilePath = @".\TestFiles\EmployeeWithHeaderNoTrailingDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn("First Name", "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn("Middle Name", "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn("Last Name", "LastName"),
                ColumnDefinitionHelper.CreateIntColumn("Age", "Age"),
                ColumnDefinitionHelper.CreateStringColumn("Address", "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeFilePath,
                    IsFirstRowHeader = true,
                    Delimiter = ','
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach (Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            Assert.IsTrue(hasEmployee, $"Employee {firstname} {lastname} with address '{address}' should be found");
        }

        [Test]
        public void Parse_WithNoTrailingDelimiter_ParsesAllRecordsCorrectly()
        {
            var employeeFilePath = @".\TestFiles\EmployeeNoTrailingDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeFilePath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);

            List<Employee> employees = csvParser.Parse().ToList();

            Assert.That(employees.Count, Is.EqualTo(5), "Should parse all 5 records");
            Assert.That(employees[0].Address, Is.EqualTo("123 Main Street, Apt 4B, City"));
            Assert.That(employees[4].Address, Is.EqualTo(@"987 Mulberry Court, ""Apt 3C"", Borough"));
        }

        [Test]
        public void Parse_WithNoTrailingDelimiterAndQuotedLastField_ParsesCorrectly()
        {
            var employeeFilePath = @".\TestFiles\EmployeeNoTrailingDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeFilePath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);

            var employees = csvParser.Parse().ToList();
            var michaelRecord = employees.FirstOrDefault(e => e.FirstName == "Michael");

            Assert.IsNotNull(michaelRecord, "Michael's record should be found");
            Assert.That(michaelRecord.Address, Is.EqualTo("789 Oak Lane, Suite 12, Village"), "Quoted last field should be unescaped correctly");
        }

        [TestCase("John", "", "Doe", 20, "123 Main Street, Apt 4B, City")]
        [TestCase("Jane", "", "Smith", 22, "456 Elm Avenue, Unit 8, Town")]
        [TestCase("Michael", "Allen", "Johnson", 21, "789 Oak Lane, Suite 12, Village")]
        [TestCase("Sarah", "", "Williams", 19, "321 Pine Road, Building 5, County")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough")]
        public void Parse_WithTrailingDelimiterHashDelimited_ParsesLastFieldCorrectly(
            string firstname, string middlename, string lastname, int age, string address)
        {
            Employee record = new Employee()
            {
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                Age = age,
                Address = address
            };

            var employeeFilePath = @".\TestFiles\EmployeeWithTrailingDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeFilePath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach (Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            Assert.IsTrue(hasEmployee, $"Employee {firstname} {lastname} with address '{address}' should be found");
        }

        [TestCase("John", "", "Doe", 20, "123 Main Street, Apt 4B, City")]
        [TestCase("Jane", "", "Smith", 22, "456 Elm Avenue, Unit 8, Town")]
        [TestCase("Michael", "Allen", "Johnson", 21, "789 Oak Lane, Suite 12, Village")]
        [TestCase("Sarah", "", "Williams", 19, "321 Pine Road, Building 5, County")]
        [TestCase("Robert", "James", "Davis", 23, @"987 Mulberry Court, ""Apt 3C"", Borough")]
        public void Parse_WithTrailingDelimiterCommaDelimited_ParsesLastFieldCorrectly(
            string firstname, string middlename, string lastname, int age, string address)
        {
            Employee record = new Employee()
            {
                FirstName = firstname,
                MiddleName = middlename,
                LastName = lastname,
                Age = age,
                Address = address
            };

            var employeeFilePath = @".\TestFiles\EmployeeWithHeaderWithTrailingDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn("First Name", "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn("Middle Name", "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn("Last Name", "LastName"),
                ColumnDefinitionHelper.CreateIntColumn("Age", "Age"),
                ColumnDefinitionHelper.CreateStringColumn("Address", "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeFilePath,
                    IsFirstRowHeader = true,
                    Delimiter = ','
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);
            bool hasEmployee = false;
            foreach (Employee employee in csvParser.Parse())
            {
                if (employee.Equals(record))
                {
                    hasEmployee = true;
                    break;
                }
            }
            Assert.IsTrue(hasEmployee, $"Employee {firstname} {lastname} with address '{address}' should be found");
        }

        [Test]
        public void Parse_WithTrailingDelimiter_ParsesAllRecordsCorrectly()
        {
            var employeeFilePath = @".\TestFiles\EmployeeWithTrailingDelimiter.csv";
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };
            var employeeProfile = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = employeeFilePath,
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            ICsvParser<Employee> csvParser = CsvParser<Employee>.Build(employeeProfile);

            List<Employee> employees = csvParser.Parse().ToList();

            Assert.That(employees.Count, Is.EqualTo(5), "Should parse all 5 records");
            Assert.That(employees[0].Address, Is.EqualTo("123 Main Street, Apt 4B, City"));
            Assert.That(employees[4].Address, Is.EqualTo(@"987 Mulberry Court, ""Apt 3C"", Borough"));
        }

        [Test]
        public void Parse_ComparingTrailingVsNoTrailingDelimiter_ProducesSameResults()
        {
            var employeeColumnMetadata = new ColumnMetadata[]
            {
                ColumnDefinitionHelper.CreateStringColumn(0, "FirstName"),
                ColumnDefinitionHelper.CreateStringColumn(1, "MiddleName"),
                ColumnDefinitionHelper.CreateStringColumn(2, "LastName"),
                ColumnDefinitionHelper.CreateIntColumn(3, "Age"),
                ColumnDefinitionHelper.CreateStringColumn(4, "Address"),
            };

            var profileWithTrailing = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = @".\TestFiles\EmployeeWithTrailingDelimiter.csv",
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });
            var profileNoTrailing = CsvProfile.Build(employeeColumnMetadata,
                new FileDetails()
                {
                    FilePath = @".\TestFiles\EmployeeNoTrailingDelimiter.csv",
                    IsFirstRowHeader = false,
                    Delimiter = '#'
                });

            ICsvParser<Employee> parserWithTrailing = CsvParser<Employee>.Build(profileWithTrailing);
            ICsvParser<Employee> parserNoTrailing = CsvParser<Employee>.Build(profileNoTrailing);

            var employeesWithTrailing = parserWithTrailing.Parse().ToList();
            var employeesNoTrailing = parserNoTrailing.Parse().ToList();

            Assert.That(employeesWithTrailing.Count, Is.EqualTo(employeesNoTrailing.Count), "Both should parse same number of records");

            for (int i = 0; i < employeesWithTrailing.Count; i++)
            {
                Assert.That(employeesWithTrailing[i], Is.EqualTo(employeesNoTrailing[i]), 
                    $"Record {i} should be identical regardless of trailing delimiter");
            }
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