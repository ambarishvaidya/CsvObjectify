using CsvObjectify;
using CsvObjectify.Column;

namespace TestCsvObjectify
{
    internal class TestCsvProfile
    {
        string _studentWithHeaderPath;
        
        [SetUp]
        public void Setup()
        {
            _studentWithHeaderPath = @".\TestFiles\StudentWithHeader.csv";            
        }
        [Test]
        public void Build_WithEmptyCollection_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => CsvProfile.Build(Array.Empty<ColumnDefinition<string>>(), _studentWithHeaderPath, false));
        }

        [Test]
        public void Build_WithDuplicateColumnNames_ThrowsException()
        {
            Assert.Throws<InvalidDataException>(() => CsvProfile.Build(new ColumnDefinition<string>[]
            {
                new ColumnDefinition<string>(columnName:"FirstName", str => str),
                new ColumnDefinition<string>(columnName:"FirstName", str => str)
            }, _studentWithHeaderPath, true));
        }

        [TestCase("")]
        [TestCase(" ")]
        public void Build_WithEmptyFilePath_ThrowsArgumentException(string filePath)
        {
            Assert.Throws<ArgumentException>(() => CsvProfile.Build(new ColumnDefinition<string>[]
            {
                new ColumnDefinition<string>("test", str => str)
            }, filePath, false));
        }
        
        [Test]
        public void Build_WithNullFilePath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CsvProfile.Build(new ColumnDefinition<string>[]
            {
                new ColumnDefinition<string>("test", str => str)
            }, null, false));
        }

        [Test]
        public void Build_WithMissingColumns_ThrowsInvalidDataException()
        {
            Assert.Throws<InvalidDataException>(() => CsvProfile.Build(new ColumnDefinition<string>[]
            {
                new ColumnDefinition<string>("firstName", str => str)
            }, _studentWithHeaderPath, true));
        }

        [TestCase("RollNo", 0)]
        [TestCase("FirstName", 1)]
        public void Build_WithnthColumns_ReturnsIndex(string colName, int colIndex)
        {
            ColumnDefinition<string> columnDefinition = new ColumnDefinition<string>(colName, str => str);

            var profile = CsvProfile.Build(new ColumnDefinition<string>[] { columnDefinition }, _studentWithHeaderPath, true);
            Assert.That(columnDefinition.ColumnIndex.HasValue && columnDefinition.ColumnIndex.Value == colIndex);
        }

        [Test]
        public void Build_WithDuplicateIndex_ThrowsInvalidDataException()
        {
            Assert.Throws<InvalidDataException>(() => CsvProfile.Build(new ColumnMetadata[]
            {
                new ColumnDefinition<int>(0, str => int.Parse(str), "RollNo"),
                new ColumnDefinition<string>("RollNo", str => str)
            }, _studentWithHeaderPath, true));
        }

        [Test]
        public void Build_WithIsRowHeaderFalse_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => CsvProfile.Build(new ColumnMetadata[]
            {
                new ColumnDefinition<int>(0, str => int.Parse(str), "RollNo"),
                new ColumnDefinition<string>("FirstName", str => str)
            }, _studentWithHeaderPath, false));
        }
    }
}
