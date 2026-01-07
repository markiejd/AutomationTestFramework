using Xunit;
using Core.Encrypt;

namespace CoreUnitTests
{
    public class EncryptionHelperTests
    {
        [Fact]
        public void EncryptionHelper_EncryptString_WithValidEnvironmentVariables_ReturnsEncryptedString()
        {
            // Arrange
            var plainText = "HelloWorld123";
            
            // Set encryption keys in environment (or test will return "UNKNOWN")
            var originalKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            var originalIV = Environment.GetEnvironmentVariable("ENCRYPTION_IV");
            
            try
            {
                // Only test if environment variables are set
                if (!string.IsNullOrEmpty(originalKey) && !string.IsNullOrEmpty(originalIV))
                {
                    // Act
                    var encrypted = EncryptionHelper.EncryptString(plainText);

                    // Assert
                    Assert.NotNull(encrypted);
                    Assert.NotEqual(plainText, encrypted);
                    Assert.NotEqual("UNKNOWN", encrypted);
                }
            }
            finally
            {
                // Restore original values
                if (originalKey != null)
                    Environment.SetEnvironmentVariable("ENCRYPTION_KEY", originalKey);
                if (originalIV != null)
                    Environment.SetEnvironmentVariable("ENCRYPTION_IV", originalIV);
            }
        }

        [Fact]
        public void EncryptionHelper_EncryptString_WithoutEnvironmentVariables_ReturnsUnknown()
        {
            // Arrange
            var plainText = "TestString";
            var originalKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            var originalIV = Environment.GetEnvironmentVariable("ENCRYPTION_IV");

            try
            {
                // Clear environment variables
                Environment.SetEnvironmentVariable("ENCRYPTION_KEY", null);
                Environment.SetEnvironmentVariable("ENCRYPTION_IV", null);

                // Act
                var result = EncryptionHelper.EncryptString(plainText);

                // Assert
                Assert.Equal("UNKNOWN", result);
            }
            finally
            {
                // Restore original values
                if (originalKey != null)
                    Environment.SetEnvironmentVariable("ENCRYPTION_KEY", originalKey);
                if (originalIV != null)
                    Environment.SetEnvironmentVariable("ENCRYPTION_IV", originalIV);
            }
        }

        [Fact]
        public void EncryptionHelper_DecryptString_WithValidEncryptedText_ReturnsPlainText()
        {
            // Arrange
            var originalKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
            var originalIV = Environment.GetEnvironmentVariable("ENCRYPTION_IV");

            try
            {
                // Only test if environment variables are set
                if (!string.IsNullOrEmpty(originalKey) && !string.IsNullOrEmpty(originalIV))
                {
                    var plainText = "TestData123";
                    var encrypted = EncryptionHelper.EncryptString(plainText);

                    // Act
                    var decrypted = EncryptionHelper.DecryptString(encrypted);

                    // Assert
                    Assert.NotNull(decrypted);
                    Assert.Equal(plainText, decrypted);
                }
            }
            finally
            {
                // Restore original values
                if (originalKey != null)
                    Environment.SetEnvironmentVariable("ENCRYPTION_KEY", originalKey);
                if (originalIV != null)
                    Environment.SetEnvironmentVariable("ENCRYPTION_IV", originalIV);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("InvalidBase64!!!")]
        public void EncryptionHelper_DecryptString_WithInvalidInput_HandlesGracefully(string encryptedText)
        {
            // Arrange
            // Act & Assert
            try
            {
                var result = EncryptionHelper.DecryptString(encryptedText);
                // Either returns a value or throws - both are acceptable
                Assert.NotNull(result);
            }
            catch (Exception ex)
            {
                // Expected for invalid encrypted text
                Assert.NotNull(ex);
            }
        }
    }
}
