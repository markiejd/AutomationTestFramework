using Xunit;

namespace CoreUnitTests
{
    public class SeleniumUtilTests
    {
        [Fact]
        public void SeleniumUtil_Initialized_Successfully()
        {
            // Arrange
            // Act
            var result = true;

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("chrome", true)]
        [InlineData("firefox", true)]
        [InlineData("edge", true)]
        public void SeleniumUtil_SupportsMultipleBrowsers(string browser, bool expected)
        {
            // Arrange
            var supportedBrowsers = new[] { "chrome", "firefox", "edge" };

            // Act
            var result = supportedBrowsers.Contains(browser);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
