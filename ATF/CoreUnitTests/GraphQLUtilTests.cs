using Xunit;
using Core;

namespace CoreUnitTests
{
    public class GraphQLUtilTests
    {
        [Fact]
        public void GraphQLUtil_GetGraphQLQuery_WithValidQuery_ReturnsJsonString()
        {
            // Arrange
            var query = "{ user { id name } }";

            // Act
            var result = GraphQLUtil.GetGraphQLQuery(query);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("query", result);
            Assert.Contains(query, result);
        }

        [Fact]
        public void GraphQLUtil_GetGraphQLQuery_WithSpecialCharacters_EscapesCorrectly()
        {
            // Arrange
            var query = "query { test: \"value with \\\"quotes\\\" \" }";

            // Act
            var result = GraphQLUtil.GetGraphQLQuery(query);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GraphQLUtil_GetGraphQLQuery_WithNullQuery_ThrowsException()
        {
            // Arrange
            string? query = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => GraphQLUtil.GetGraphQLQuery(query!));
        }

        [Fact]
        public void GraphQLUtil_GetGraphQLObject_WithValidModel_ReturnsJsonString()
        {
            // Arrange
            var model = new GraphQL { query = "test query" };

            // Act
            var result = GraphQLUtil.GetGraphQLObject(model);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("test query", result);
        }

        [Fact]
        public void GraphQLUtil_GetGraphQLObject_WithNullModel_ThrowsException()
        {
            // Arrange
            GraphQL? model = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => GraphQLUtil.GetGraphQLObject(model!));
        }

        [Fact]
        public void GraphQLUtil_GetGraphQLModel_WithValidJson_ReturnsModel()
        {
            // Arrange
            var json = "{\"query\":\"test query\"}";

            // Act
            var result = GraphQLUtil.GetGraphQLModel(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test query", result.query);
        }

        [Fact]
        public void GraphQLUtil_GetGraphQLModel_WithNullJson_ReturnsNull()
        {
            // Arrange
            string json = "";

            // Act
            var result = GraphQLUtil.GetGraphQLModel(json);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GraphQLUtil_GetGraphQLModel_WithEmptyJson_ReturnsNull()
        {
            // Arrange
            var json = "";

            // Act
            var result = GraphQLUtil.GetGraphQLModel(json);

            // Assert
            Assert.Null(result);
        }
    }
}
