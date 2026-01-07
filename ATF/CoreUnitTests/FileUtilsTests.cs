using Xunit;
using Core.FileIO;
using System.IO;

namespace CoreUnitTests
{
    public class FileUtilsTests
    {
        [Fact]
        public void FileUtils_GetRepoDirectory_ReturnsValidPath()
        {
            // Arrange
            // Act
            var repoDir = FileUtils.GetRepoDirectory();

            // Assert
            Assert.NotNull(repoDir);
            Assert.NotEmpty(repoDir);
            Assert.True(Directory.Exists(repoDir), $"Repo directory should exist: {repoDir}");
        }

        [Fact]
        public void FileUtils_CurrentDirectory_IsNotEmpty()
        {
            // Arrange
            // Act
            var currentDir = FileUtils.currentDirectory;

            // Assert
            Assert.NotNull(currentDir);
            Assert.NotEmpty(currentDir);
        }

        [Theory]
        [InlineData("/valid/path/file.txt")]
        [InlineData("/data/config.json")]
        [InlineData("/test/config.xml")]
        public void FileUtils_FileCheck_WithValidPaths_ExecutesSuccessfully(string fileName)
        {
            // Arrange
            // Act
            var result = FileUtils.FileCheck(fileName);

            // Assert
            Assert.IsType<bool>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FileUtils_FileCheck_WithInvalidPaths_ReturnsFalse(string? fileName)
        {
            // Arrange
            // Act
            var result = FileUtils.FileCheck(fileName ?? "");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void FileUtils_DoesDirectoryExist_WithValidDirectory_ReturnsBoolean()
        {
            // Arrange
            var directory = Directory.GetCurrentDirectory();

            // Act
            var result = FileUtils.DoesDirectoryExist(directory);

            // Assert
            Assert.IsType<bool>(result);
            Assert.True(result); // Current directory should exist
        }

        [Fact]
        public void FileUtils_GetImagesProjectDirectory_ReturnsValidPath()
        {
            // Arrange
            // Act
            var result = FileUtils.GetImagesProjectDirectory();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
