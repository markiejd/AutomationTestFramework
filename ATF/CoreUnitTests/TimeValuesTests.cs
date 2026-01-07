using Xunit;
using Core.Transformations;

namespace CoreUnitTests
{
    public class TimeValuesTests
    {
        [Fact]
        public void TimeValues_ReturnNowTimeAsString_ReturnsValidTimeFormat()
        {
            // Arrange
            // Act
            var result = TimeValues.ReturnNowTimeAsString();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("HH:mm")]
        [InlineData("HH:mm:ss")]
        [InlineData("h:mm tt")]
        public void TimeValues_ReturnNowTimeAsString_WithValidFormats_ReturnsFormattedTime(string format)
        {
            // Arrange
            // Act
            var result = TimeValues.ReturnNowTimeAsString(format);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TimeValues_IsTimeIsh_WithSameTime_ReturnsTrue()
        {
            // Arrange
            var currentTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                // get the current time PLUS 1 minute
            var laterTime = DateTime.Now.AddMinutes(1).ToString("dd/MM/yyyy HH:mm");
            var range = 5; // minutes

            // Act
            var result = TimeValues.IsTimeIsh(currentTime, laterTime, range);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TimeValues_IsTimeIsh_WithTimesOutOfRange_ReturnsFalse()
        {
            // Arrange
            var time1 = "15/01/2024 10:00";
            var time2 = "15/01/2024 12:00";
            var range = 1; // minute - very small range

            // Act
            var result = TimeValues.IsTimeIsh(time1, time2, range);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("invalid/time")]
        [InlineData("")]
        [InlineData("25:00:00")]
        public void TimeValues_IsTimeIsh_WithInvalidTimeFormat_ReturnsFalse(string invalidTime)
        {
            // Arrange
            var validTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            // Act
            var result = TimeValues.IsTimeIsh(invalidTime, validTime, 5);

            // Assert
            Assert.False(result);
        }
    }
}
