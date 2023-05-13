using CsvObjectify.Validation;

namespace TestCsvObjectify
{
    internal class TestValidation
    {
        [Test]
        public void IsEmpty_WithValidInputs_ReturnsTrue()
        {
            Assert.IsTrue("".IsEmpty());
        }

        [TestCase(" ")]
        [TestCase(" a")]
        public void IsEmpty_WithInValidInputs_ReturnsTrue(string input)
        {
            Assert.IsFalse(input.IsEmpty());
        }

        [Test]
        public void IsNull_WithValidInputs_ReturnsTrue()
        {
            string nullStr = null;
            Assert.IsTrue(nullStr.IsNull());
        }
    }
}
