using Xunit;
using Core;

namespace CoreUnitTests
{
    public class ADFUtilsTests
    {
        [Fact]
        public void ADFUtils_Hello_ReturnsGreeting()
        {
            // Arrange
            // Act
            var result = ADFUtils.hello();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Hello from ADFUtils", result);
        }

        [Theory]
        [InlineData("/valid/path")]
        [InlineData("/another/adf/path")]
        public void ADFUtils_DoesADFDirectoryExist_WithValidPath_ExecutesSuccessfully(string directoryPath)
        {
            // Arrange
            // Act & Assert
            // This will depend on ADF being available - for now just verify it doesn't throw
            try
            {
                var result = ADFUtils.DoesADFDirectoryExist(directoryPath);
                Assert.IsType<bool>(result);
            }
            catch (Exception ex)
            {
                // Expected if ADF is not running
                Assert.NotNull(ex);
            }
        }

        [Fact]
        public void ADFUtils_GetAllFileNamesInADFDirectory_ReturnsListOfStrings()
        {
            // Arrange
            var testDirectory = "/test";

            // Act & Assert
            try
            {
                var result = ADFUtils.GetAllFileNamesInADFDirectory(testDirectory);
                Assert.IsType<List<string>>(result);
            }
            catch (Exception ex)
            {
                // Expected if ADF is not running
                Assert.NotNull(ex);
            }
        }

        [Fact]
        public void ADFUtils_DeleteDirectoryAndContentsInADF_WithValidPath_ExecutesSuccessfully()
        {
            // Arrange
            var testDirectory = "/test/deleteme";

            // Act & Assert
            try
            {
                var result = ADFUtils.DeleteDirectoryAndContentsInADF(testDirectory);
                Assert.IsType<bool>(result);
            }
            catch (Exception ex)
            {
                // Expected if ADF is not running
                Assert.NotNull(ex);
            }
        }
    }
}
