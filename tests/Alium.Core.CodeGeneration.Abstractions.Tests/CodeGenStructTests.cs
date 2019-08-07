namespace Alium.CodeGeneration
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="CodeGen"/> type.
    /// </summary>
    public class CodeGenStructTests
    {
        [Fact]
        public void Struct_ValidatesArguments()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(
                "name",
                () => CodeGen.Struct(null /* name */));
        }

        [Fact]
        public void Struct_GeneratesStructSyntax()
        {
            // Arrange

            // Act
            var @struct = CodeGen.Struct("Value");

            // Assert
            Assert.NotNull(@struct);
            Assert.Equal("structValue{}", @struct.ToString());
        }

        [Fact]
        public void Struct_GeneratesStructSyntax_WithModifiers()
        {
            // Arrange
            var modifiers = new[]
            {
                SyntaxKind.PublicKeyword
            };

            // Act
            var @struct = CodeGen.Struct("Value", modifiers);

            // Assert
            Assert.NotNull(@struct);
            Assert.Equal("publicstructValue{}", @struct.ToString());
        }
    }
}
