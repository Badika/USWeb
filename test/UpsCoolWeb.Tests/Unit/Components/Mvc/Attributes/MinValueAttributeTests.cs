using UpsCoolWeb.Resources;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class MinValueAttributeTests
    {
        private MinValueAttribute attribute;

        public MinValueAttributeTests()
        {
            attribute = new MinValueAttribute(12.56);
        }

        #region MinValueAttribute(Double minimum)

        [Fact]
        public void MinValueAttribute_SetsMinimum()
        {
            Decimal actual = new MinValueAttribute(12.56).Minimum;
            Decimal expected = 12.56M;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region FormatErrorMessage(String name)

        [Fact]
        public void FormatErrorMessage_ForName()
        {
            attribute = new MinValueAttribute(12.56);

            String actual = attribute.FormatErrorMessage("Sum");
            String expected = Validation.For("MinValue", "Sum", attribute.Minimum);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region IsValid(Object value)

        [Fact]
        public void IsValid_Null()
        {
            Assert.True(attribute.IsValid(null));
        }

        [Theory]
        [InlineData(12.56)]
        [InlineData("12.561")]
        public void IsValid_GreaterOrEqualValue(Object value)
        {
            Assert.True(attribute.IsValid(value));
        }

        [Fact]
        public void IsValid_LowerValue_ReturnsFalse()
        {
            Assert.False(attribute.IsValid(12.559));
        }

        [Fact]
        public void IsValid_NotDecimalValueIsNotValid()
        {
            Assert.False(attribute.IsValid("12.56M"));
        }

        #endregion
    }
}
