using UpsCoolWeb.Resources;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class GreaterThanAttributeTests
    {
        private GreaterThanAttribute attribute;

        public GreaterThanAttributeTests()
        {
            attribute = new GreaterThanAttribute(12.56);
        }

        #region GreaterThanAttribute(Double minimum)

        [Fact]
        public void GreaterThanAttribute_SetsMinimum()
        {
            Decimal actual = new GreaterThanAttribute(12.56).Minimum;
            Decimal expected = 12.56M;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region FormatErrorMessage(String name)

        [Fact]
        public void FormatErrorMessage_ForName()
        {
            attribute = new GreaterThanAttribute(12.56);

            String actual = attribute.FormatErrorMessage("Sum");
            String expected = Validation.For("GreaterThan", "Sum", attribute.Minimum);

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
        [InlineData("1")]
        [InlineData(12.56)]
        [InlineData(12.559)]
        public void IsValid_LowerOrEqualValue_ReturnsFalse(Object value)
        {
            Assert.False(attribute.IsValid(value));
        }

        [Theory]
        [InlineData(13)]
        [InlineData("100")]
        public void IsValid_GreaterValue(Object value)
        {
            Assert.True(attribute.IsValid(value));
        }

        [Fact]
        public void IsValid_NotDecimal_ReturnsFalse()
        {
            Assert.False(attribute.IsValid("12.60M"));
        }

        #endregion
    }
}
