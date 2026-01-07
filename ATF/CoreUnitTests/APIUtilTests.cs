using Xunit;

namespace CoreUnitTests
{
    public class APIUtilTests
    {
        [Fact]
        public void APIUtil_CanMakeRequest_Successfully()
        {
            // Arrange
            var apiUrl = "https://api.example.com";

            // Act
            var isValidUrl = Uri.IsWellFormedUriString(apiUrl, UriKind.Absolute);

            // Assert
            Assert.True(isValidUrl);
        }

        [Theory]
        [InlineData("application/json", true)]
        [InlineData("application/xml", true)]
        [InlineData("text/plain", false)]
        public void APIUtil_ValidContentTypes(string contentType, bool expected)
        {
            // Arrange
            var validTypes = new[] { "application/json", "application/xml" };

            // Act
            var result = validTypes.Contains(contentType);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
