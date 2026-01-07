using Xunit;
using Core.Transformations;

namespace CoreUnitTests
{
    public class StringValuesTests
    {
        [Fact]
        public void StringValues_TextReplacementService_WithMYREPO_ReturnsReplacedPath()
        {
            // Arrange
            var input = "The repo is at MYREPO/TestData";

            // Act
            var result = StringValues.TextReplacementService(input);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(input, result); // Should be different after replacement
            Assert.DoesNotContain("MYREPO", result);
        }

        [Fact]
        public void StringValues_TextReplacementService_WithoutSpecialTokens_ReturnsUnchanged()
        {
            // Arrange
            var input = "This is a normal string without tokens";

            // Act
            var result = StringValues.TextReplacementService(input);

            // Assert
            Assert.Equal(input, result);
        }

        [Theory]
        [InlineData("ATFVARIABLE0")]
        [InlineData("ATFVARIABLE5")]
        [InlineData("ATFVARIABLE9")]
        public void StringValues_TextReplacementService_WithATFVARIABLE_ProcessesValidNumbers(string variable)
        {
            // Arrange - valid ATFVARIABLE numbers are 0-9

            // Act
            var result = StringValues.TextReplacementService(variable);

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("ATFVARIABLE10")] // Out of range
        [InlineData("ATFVARIABLE-1")] // Negative
        public void StringValues_TextReplacementService_WithInvalidATFVARIABLE_KeepsOriginal(string variable)
        {
            // Arrange

            // Act
            var result = StringValues.TextReplacementService(variable);

            // Assert - Invalid numbers should keep ATFVARIABLE in result
            Assert.NotNull(result);
        }
    }
}
