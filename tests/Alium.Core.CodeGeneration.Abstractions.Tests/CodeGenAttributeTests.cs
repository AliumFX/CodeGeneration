namespace Alium.CodeGeneration
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="CodeGen"/> type.
    /// </summary>
    public class CodeGenAttributeTests
    {
        [Fact]
        public void Attribute_ValidatesArguments()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(
                "name",
                () => CodeGen.Attribute(null /* name */));
        }

        [Fact]
        public void Attribute_GeneratesAttributeSyntax()
        {
            // Arrange
            var name = CodeGen.Name("Attribute");

            // Act
            var attribute = CodeGen.Attribute(name);

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Attribute", attribute.ToString());
        }

        [Fact]
        public void Attribute_GeneratesAttributeSyntax_WithSingleArgument()
        {
            // Arrange
            var name = CodeGen.Name("Attribute");
            var argument = CodeGen.StringAttributeArgument("Hello World");

            // Act
            var attribute = CodeGen.Attribute(name, new[] { argument });

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Attribute(\"Hello World\")", attribute.ToString());
        }

        [Fact]
        public void Attribute_GeneratesAttributeSyntax_WithMultipleArgument()
        {
            // Arrange
            var name = CodeGen.Name("Attribute");
            var argument1 = CodeGen.StringAttributeArgument("Hello");
            var argument2 = CodeGen.StringAttributeArgument("World");

            // Act
            var attribute = CodeGen.Attribute(name, new[] { argument1, argument2 });

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Attribute(\"Hello\",\"World\")", attribute.ToString());
        }

        [Fact]
        public void Attributes_GeneratesAttributeListSyntax_WithSingleAttribute()
        {
            // Arrange
            var attribute = CodeGen.Attribute(CodeGen.Name("Attribute"));

            // Act
            var attributes = CodeGen.Attributes(new[] { attribute });
            var @class = SyntaxFactory.ClassDeclaration("Test")
                .WithAttributeLists(SyntaxFactory.List(attributes));

            // Assert
            Assert.NotNull(attributes);
            Assert.Equal("[Attribute]classTest{}", @class.ToString());
        }

        [Fact]
        public void Attributes_GeneratesAttributeListSyntax_WithMultipleAttributes()
        {
            // Arrange
            var attribute1 = CodeGen.Attribute(CodeGen.Name("Attribute"));
            var attribute2 = CodeGen.Attribute(CodeGen.Name("Attribute"));

            // Act
            var attributes = CodeGen.Attributes(new[] { attribute1, attribute2 });
            var @class = SyntaxFactory.ClassDeclaration("Test")
                .WithAttributeLists(SyntaxFactory.List(attributes));

            // Assert
            Assert.NotNull(attributes);
            Assert.Equal("[Attribute][Attribute]classTest{}", @class.ToString());
        }

        [Fact]
        public void Attributes_GeneratesAttributeListSyntax_WithMultipleAttributes_Joined()
        {
            // Arrange
            var attribute1 = CodeGen.Attribute(CodeGen.Name("Attribute"));
            var attribute2 = CodeGen.Attribute(CodeGen.Name("Attribute"));

            // Act
            var attributes = CodeGen.Attributes(new[] { attribute1, attribute2 }, joined: true);
            var @class = SyntaxFactory.ClassDeclaration("Test")
                .WithAttributeLists(SyntaxFactory.List(attributes));

            // Assert
            Assert.NotNull(attributes);
            Assert.Equal("[Attribute,Attribute]classTest{}", @class.ToString());
        }

        #region Arguments
        [Fact]
        public void StringAttributeArgument_GeneratesAttributeArgument()
        {
            // Arrange

            // Act
            var argument = CodeGen.StringAttributeArgument("Hello World");

            // Assert
            Assert.NotNull(argument);
            Assert.Equal("\"Hello World\"", argument.ToString());
        }

        [Fact]
        public void StringAttributeArgument_GeneratesAttributeArgument_WithName()
        {
            // Arrange

            // Act
            var argument = CodeGen.StringAttributeArgument("Hello World", "Message");

            // Assert
            Assert.NotNull(argument);
            Assert.Equal("Message=\"Hello World\"", argument.ToString());
        }

        [Fact]
        public void TypeOfAttributeArgument_GeneratesAttributeArgument()
        {
            // Arrange

            // Act
            var argument = CodeGen.TypeOfAttributeArgument("System.String");

            // Assert
            Assert.NotNull(argument);
            Assert.Equal("typeof(System.String)", argument.ToString());
        }

        [Fact]
        public void TypeOfAttributeArgument_GeneratesAttributeArgument_WithName()
        {
            // Arrange

            // Act
            var argument = CodeGen.TypeOfAttributeArgument("System.String", "Provider");

            // Assert
            Assert.NotNull(argument);
            Assert.Equal("Provider=typeof(System.String)", argument.ToString());
        }
        #endregion
    }
}
