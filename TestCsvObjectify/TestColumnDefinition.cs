using CsvObjectify.Column;

namespace TestCsvObjectify
{
    internal class TestColumnDefinition
    {
        [SetUp]
        public void SetUp()
        { }

        [Test]
        public void Constructor_WithValidInputs_CreatesInstance()
        {
            ColumnDefinition<string> defn = new ColumnDefinition<string>(columnName: "columnName", str => str);            
            Assert.IsNotNull(defn);
        }

        [Test]
        public void Constructor_WithValidIndexInputs_CreatesInstance()
        {
            ColumnDefinition<string> defn = new ColumnDefinition<string>(0, str => str, "PropertyName");
            Assert.IsNotNull(defn);
        }

        [Test]
        public void Constructor_WithInValidIndexInputs_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ColumnDefinition<string>(-1, str => str, "PropertyName"));            
        }

        [TestCase("")]
        [TestCase("   ")]        
        public void Constructor_WithEmptyColumnName_ThrowsArgumentException(string columnName)
        {
            Assert.Throws<ArgumentException>( () =>  new ColumnDefinition<string>(columnName: columnName, csvToEntityProperty: str => str));            
        }

        [Test]
        public void Constructor_WithNullColumnName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ColumnDefinition<string>(columnName: null, csvToEntityProperty: str => str));
        }

        [Test]
        public void Constructor_WithNullCsvToEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ColumnDefinition<string>(columnName: "columnName", null));
        }

        [Test]
        public void Constructor_WithNoPropertyName_AssignsColumnNameAsPropertyName()
        {
            var defn = new ColumnDefinition<string>(columnName: "columnName", str => str);
            Assert.IsTrue(defn.PropertyName == "columnName");
        }

        [Test]
        public void Constructor_WithNullPropertyName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ColumnDefinition<string>(columnName: "columnName", str => str, null));            
        }

        [TestCase("")]
        [TestCase("  ")]
        public void Constructor_WithEmptyPropertyName_ThrowsArgumentException(string propName)
        {
            Assert.Throws<ArgumentException>(() => new ColumnDefinition<string>(columnName: "columnName", str => str, propName));
        }
    }
}
