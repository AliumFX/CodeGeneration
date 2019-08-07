namespace Alium.CodeGeneration
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="CodeGen"/> type.
    /// </summary>
    public class CodeGenNameTests
    {
        [Fact]
        public void Name_ValidatesArguments()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(
                "name",
                () => CodeGen.Name(null /* name */));
        }

        [Fact]
        public void Name_GeneratesNameSyntax()
        {
            // Arrange

            // Act
            var syntax = CodeGen.Name("String");

            // Assert
            Assert.NotNull(syntax);
            Assert.Equal("String", syntax.ToString());
        }

        [Fact]
        public void Name_GeneratesNameSyntax_SupportsNamespace()
        {
            // Arrange

            // Act
            var syntax = CodeGen.Name("String", "System");

            // Assert
            Assert.NotNull(syntax);
            Assert.Equal("System.String", syntax.ToString());
        }

        [Fact]
        public void Name_GeneratesNameSyntax_SupportsNestedNamespace()
        {
            // Arrange

            // Act
            var syntax = CodeGen.Name("Path", "System.IO");

            // Assert
            Assert.NotNull(syntax);
            Assert.Equal("System.IO.Path", syntax.ToString());
        }

        [Fact]
        public void Name_GeneratesNameSyntax_SupportsGlobal()
        {
            // Arrange

            // Act
            var syntax = CodeGen.Name("String", global: true);

            // Assert
            Assert.NotNull(syntax);
            Assert.Equal("global::String", syntax.ToString());
        }

        [Fact]
        public void Name_GeneratesNameSyntax_SupportsNamespace_AndGlobal()
        {
            // Arrange

            // Act
            var syntax = CodeGen.Name("String", "System", global: true);

            // Assert
            Assert.NotNull(syntax);
            Assert.Equal("global::System.String", syntax.ToString());
        }

        [Fact]
        public void Name_GeneratesNameSyntax_SupportsNestedNamespace_AndGlobal()
        {
            // Arrange

            // Act
            var syntax = CodeGen.Name("Path", "System.IO", global: true);

            // Assert
            Assert.NotNull(syntax);
            Assert.Equal("global::System.IO.Path", syntax.ToString());
        }
    }
}
