namespace Alium.CodeGeneration
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="CodeGen"/> type.
    /// </summary>
    public class CodeGenFieldTests
    {
        [Fact]
        public void Field_ValidatesArguments()
        {
            // Arrange
            string name = "field";
            var type = CodeGen.Name("Type");

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(
                "name",
                () => CodeGen.Field(null /* name */, type));

            Assert.Throws<ArgumentNullException>(
                "type",
                () => CodeGen.Field(name, null /* type */));
        }

        [Fact]
        public void Field_GeneratesField()
        {
            // Arrange
            string name = "_name";
            var type = CodeGen.Name("String", "System");

            // Act
            var field = CodeGen.Field(name, type);

            // Assert
            Assert.Equal("System.String_name;", field.ToString());
        }

        [Fact]
        public void Field_GeneratesField_WithInitialiser()
        {
            // Arrange
            string name = "_name";
            var type = CodeGen.Name("String", "System");
            var init = SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal("Hello World"));

            // Act
            var field = CodeGen.Field(
                name, 
                type,
                initialiser: init);

            // Assert
            Assert.Equal("System.String_name=\"Hello World\";", field.ToString());
        }

        [Fact]
        public void Field_GeneratesField_WithModifiers()
        {
            // Arrange
            string name = "_name";
            var type = CodeGen.Name("String", "System");
            var modifiers = new[] { SyntaxKind.ReadOnlyKeyword };

            // Act
            var field = CodeGen.Field(name, type, modifiers: modifiers);

            // Assert
            Assert.Equal("readonlySystem.String_name;", field.ToString());
        }
    }
}
