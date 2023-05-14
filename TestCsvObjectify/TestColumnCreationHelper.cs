using CsvObjectify.Column.Helper;

namespace TestCsvObjectify
{
    internal class TestColumnCreationHelper
    {
        [TestCase("", false, true )]
        [TestCase(" ", false, true)]
        [TestCase(null, true, true)]
        [TestCase("columnName", false, false)]
        public void Create_StringColumnDefnWithValidDefaultFields_PassTest(string columnName, bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateStringColumn(columnName));
            else if (throwArgumentException )
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateStringColumn(columnName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateStringColumn(columnName));
        }

        [TestCase("", "propertyName", false, true)]
        [TestCase("columnName", "", false, true)]
        [TestCase("  ", "propertyName", false, true)]
        [TestCase("columnName", "  ", false, true)]
        [TestCase("  ", "", false, true)]
        [TestCase("", "  ", false, true)]
        [TestCase("", " ", false, true)]
        [TestCase(" ", "", false, true)]
        [TestCase(null, "propertyName", true, false)]
        [TestCase("columnName", null, true, false)]
        [TestCase(null, null, true, false)]        
        [TestCase("columnName", "ppropertyName", false, false)]
        public void Create_StringColumnAndPropertyDefnWithFields_PassTest(string columnName, string propertyName, 
            bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateStringColumn(columnName, propertyName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateStringColumn(columnName, propertyName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateStringColumn(columnName, propertyName));
        }

        [TestCase(-1, "", false, true)]
        [TestCase(-1, null, false, true)]
        [TestCase(-1, "propertyName", false, true)]
        [TestCase(1, "", false, true)]
        [TestCase(1, null, true, false)]
        [TestCase(1, "propertyName", false, false)]
        public void Create_StringIndexAndPropertyDefnWithFields_PassTest(int index, string propertyName,
            bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateStringColumn(index, propertyName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateStringColumn(index, propertyName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateStringColumn(index, propertyName));
        }

        [TestCase("", false, true)]
        [TestCase(" ", false, true)]
        [TestCase(null, true, true)]
        [TestCase("columnName", false, false)]
        public void Create_IntColumnDefnWithValidDefaultFields_PassTest(string columnName, bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateIntColumn(columnName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateIntColumn(columnName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateIntColumn(columnName));
        }

        [TestCase("", "propertyName", false, true)]
        [TestCase("columnName", "", false, true)]
        [TestCase("  ", "propertyName", false, true)]
        [TestCase("columnName", "  ", false, true)]
        [TestCase("  ", "", false, true)]
        [TestCase("", "  ", false, true)]
        [TestCase("", " ", false, true)]
        [TestCase(" ", "", false, true)]
        [TestCase(null, "propertyName", true, false)]
        [TestCase("columnName", null, true, false)]
        [TestCase(null, null, true, false)]
        [TestCase("columnName", "ppropertyName", false, false)]
        public void Create_IntColumnAndPropertyDefnWithFields_PassTest(string columnName, string propertyName,
            bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateIntColumn(columnName, propertyName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateIntColumn(columnName, propertyName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateIntColumn(columnName, propertyName));
        }

        [TestCase(-1, "", false, true)]
        [TestCase(-1, null, false, true)]
        [TestCase(-1, "propertyName", false, true)]
        [TestCase(1, "", false, true)]
        [TestCase(1, null, true, false)]
        [TestCase(1, "propertyName", false, false)]
        public void Create_IntIndexAndPropertyDefnWithFields_PassTest(int index, string propertyName,
            bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateIntColumn(index, propertyName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateIntColumn(index, propertyName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateIntColumn(index, propertyName));
        }

        [TestCase("", false, true)]
        [TestCase(" ", false, true)]
        [TestCase(null, true, true)]
        [TestCase("columnName", false, false)]
        public void Create_DoubleColumnDefnWithValidDefaultFields_PassTest(string columnName, bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateDoubleColumn(columnName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateDoubleColumn(columnName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateDoubleColumn(columnName));
        }

        [TestCase("", "propertyName", false, true)]
        [TestCase("columnName", "", false, true)]
        [TestCase("  ", "propertyName", false, true)]
        [TestCase("columnName", "  ", false, true)]
        [TestCase("  ", "", false, true)]
        [TestCase("", "  ", false, true)]
        [TestCase("", " ", false, true)]
        [TestCase(" ", "", false, true)]
        [TestCase(null, "propertyName", true, false)]
        [TestCase("columnName", null, true, false)]
        [TestCase(null, null, true, false)]
        [TestCase("columnName", "ppropertyName", false, false)]
        public void Create_DoubleColumnAndPropertyDefnWithFields_PassTest(string columnName, string propertyName,
            bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateDoubleColumn(columnName, propertyName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateDoubleColumn(columnName, propertyName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateDoubleColumn(columnName, propertyName));
        }

        [TestCase(-1, "", false, true)]
        [TestCase(-1, null, false, true)]
        [TestCase(-1, "propertyName", false, true)]
        [TestCase(1, "", false, true)]
        [TestCase(1, null, true, false)]
        [TestCase(1, "propertyName", false, false)]
        public void Create_DoubleIndexAndPropertyDefnWithFields_PassTest(int index, string propertyName,
            bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateDoubleColumn(index, propertyName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateDoubleColumn(index, propertyName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateDoubleColumn(index, propertyName));
        }

        [TestCase("", false, true)]
        [TestCase(" ", false, true)]
        [TestCase(null, true, true)]
        [TestCase("columnName", false, false)]
        public void Create_FloatColumnDefnWithValidDefaultFields_PassTest(string columnName, bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateFloatColumn(columnName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateFloatColumn(columnName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateFloatColumn(columnName));
        }

        [TestCase("", "propertyName", false, true)]
        [TestCase("columnName", "", false, true)]
        [TestCase("  ", "propertyName", false, true)]
        [TestCase("columnName", "  ", false, true)]
        [TestCase("  ", "", false, true)]
        [TestCase("", "  ", false, true)]
        [TestCase("", " ", false, true)]
        [TestCase(" ", "", false, true)]
        [TestCase(null, "propertyName", true, false)]
        [TestCase("columnName", null, true, false)]
        [TestCase(null, null, true, false)]
        [TestCase("columnName", "ppropertyName", false, false)]
        public void Create_FloatColumnAndPropertyDefnWithFields_PassTest(string columnName, string propertyName,
            bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateFloatColumn(columnName, propertyName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateFloatColumn(columnName, propertyName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateFloatColumn(columnName, propertyName));
        }

        [TestCase(-1, "", false, true)]
        [TestCase(-1, null, false, true)]
        [TestCase(-1, "propertyName", false, true)]
        [TestCase(1, "", false, true)]
        [TestCase(1, null, true, false)]
        [TestCase(1, "propertyName", false, false)]
        public void Create_FloatIndexAndPropertyDefnWithFields_PassTest(int index, string propertyName,
            bool throwArgumentNullException, bool throwArgumentException)
        {
            if (throwArgumentNullException)
                Assert.Throws<ArgumentNullException>(() => ColumnDefinitionHelper.CreateFloatColumn(index, propertyName));
            else if (throwArgumentException)
                Assert.Throws<ArgumentException>(() => ColumnDefinitionHelper.CreateFloatColumn(index, propertyName));
            else
                Assert.IsNotNull(ColumnDefinitionHelper.CreateFloatColumn(index, propertyName));
        }
    }
}
