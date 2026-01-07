using Xunit;
using Core.Transformations;

namespace CoreUnitTests
{
    public class DateValuesTests
    {
        [Fact]
        public void DateValues_ReturnNowDateAsString_ReturnsValidDate()
        {
            // Arrange
            // Act
            var result = DateValues.ReturnNowDateAsString();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void DateValues_GetDateTime_ReturnsValidDateTime()
        {
            // Arrange
            // Act
            var result = DateValues.GetDateTime();

            // Assert
            Assert.NotNull(result);
            Assert.True(result > DateTime.MinValue);
        }

        [Theory]
        [InlineData("101")]
        [InlineData("102")]
        [InlineData("103")]
        public void DateValues_ReturnNowDateAsString_WithValidFormats_ReturnsFormattedDate(string format)
        {
            // Arrange
            // Act
            var result = DateValues.ReturnNowDateAsString(format);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void DateValues_GetDateTimeFromParse_WithValidDateString_ReturnsDateTime()
        {
            // Arrange
            var validDate = "2024-01-15";

            // Act
            var result = DateValues.GetDateTimeFromParse(validDate);

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("not-a-date")]
        [InlineData("")]
        [InlineData("32-13-2024")]
        public void DateValues_GetDateTimeFromParse_WithInvalidDate_ReturnsNull(string invalidDate)
        {
            // Arrange
            // Act
            var result = DateValues.GetDateTimeFromParse(invalidDate);

            // Assert
            Assert.Null(result);
        }
    }
}
