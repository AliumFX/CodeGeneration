namespace Alium.Data.CodeGeneration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="Int32EntityIdSyntaxGenerator"/> type.
    /// </summary>
    public class Int32EntityIdSyntaxGeneratorTests
    {
        [Fact]
        public async Task GenerateStructAsync_GeneratesStruct()
        {
            // Arrange
            var context = new EntityIdGenerationContext(
                SyntaxFactory.StructDeclaration("EntityId"),
                typeConverter: false,
                jsonNetConverter: false);

            // Act
            var @struct = await Int32EntityIdSyntaxGenerator.GenerateStructAsync(context);

            // Assert
            Assert.NotNull(@struct);
            Assert.Equal("EntityId", @struct.Identifier.ValueText);

            AssertHasModifier(@struct, SyntaxKind.ReadOnlyKeyword);
            AssertHasAttribute(@struct, "System.Diagnostics.DebuggerDisplay",
                attribute =>
                {
                    Assert.Equal("(\"{DebuggerToString(),nq}\")", attribute.ArgumentList.ToString());
                }
            );
            AssertDoesNotHaveAttribute(@struct, "System.ComponentModel.TypeConverter");
            AssertDoesNotHaveAttribute(@struct, "Newtonsoft.Json.JsonConverter");
        }

        [Fact]
        public async Task GenerateStructAsync_GeneratesStruct_WithTypeConverter()
        {
            // Arrange
            var context = new EntityIdGenerationContext(
                SyntaxFactory.StructDeclaration("EntityId"),
                typeConverter: true,
                jsonNetConverter: false);

            // Act
            var @struct = await Int32EntityIdSyntaxGenerator.GenerateStructAsync(context);

            // Assert
            Assert.NotNull(@struct);
            Assert.Equal("EntityId", @struct.Identifier.ValueText);

            AssertHasModifier(@struct, SyntaxKind.ReadOnlyKeyword);
            AssertHasAttribute(@struct, "System.Diagnostics.DebuggerDisplay",
                attribute =>
                {
                    Assert.Equal("(\"{DebuggerToString(),nq}\")", attribute.ArgumentList.ToString());
                }
            );
            AssertHasAttribute(@struct, "System.ComponentModel.TypeConverter",
                attribute =>
                {
                    Assert.Equal("(typeof(EntityIdTypeConverter))", attribute.ArgumentList.ToString());
                }
            );
            AssertDoesNotHaveAttribute(@struct, "Newtonsoft.Json.JsonConverter");
        }

        [Fact]
        public async Task GenerateStructAsync_GeneratesStruct_WithJsonConverter()
        {
            // Arrange
            var context = new EntityIdGenerationContext(
                SyntaxFactory.StructDeclaration("EntityId"),
                typeConverter: false,
                jsonNetConverter: true);

            // Act
            var @struct = await Int32EntityIdSyntaxGenerator.GenerateStructAsync(context);

            // Assert
            Assert.NotNull(@struct);
            Assert.Equal("EntityId", @struct.Identifier.ValueText);

            AssertHasModifier(@struct, SyntaxKind.ReadOnlyKeyword);
            AssertHasAttribute(@struct, "System.Diagnostics.DebuggerDisplay",
                attribute =>
                {
                    Assert.Equal("(\"{DebuggerToString(),nq}\")", attribute.ArgumentList.ToString());
                }
            );
            AssertDoesNotHaveAttribute(@struct, "System.ComponentModel.TypeConverter");
            AssertHasAttribute(@struct, "Newtonsoft.Json.JsonConverter",
                attribute =>
                {
                    Assert.Equal("(typeof(EntityIdJsonConverter))", attribute.ArgumentList.ToString());
                }
            );
        }

        [Fact]
         public async Task GenerateStructAsync_GeneratesStruct_WithEmptyField()
        {
            // Arrange
            var context = new EntityIdGenerationContext(
                SyntaxFactory.StructDeclaration("EntityId"),
                typeConverter: false,
                jsonNetConverter: false);

            // Act
            var @struct = await Int32EntityIdSyntaxGenerator.GenerateStructAsync(context);

            // Assert
            AssertHasField(
                @struct,
                name: "Empty",
                type: "EntityId",
                (member, variable) =>
                {
                    AssertHasModifier(member, SyntaxKind.PublicKeyword);
                    AssertHasModifier(member, SyntaxKind.StaticKeyword);
                    AssertHasModifier(member, SyntaxKind.ReadOnlyKeyword);

                    Assert.NotNull(variable.Initializer);
                    Assert.Equal("= new EntityId()", variable.Initializer.ToString());
                });
        }

        [Fact]
        public async Task GenerateStructAsync_GeneratesStruct_WithHasValueProperty()
        {
            // Arrange
            var context = new EntityIdGenerationContext(
                SyntaxFactory.StructDeclaration("EntityId"),
                typeConverter: false,
                jsonNetConverter: false);

            // Act
            var @struct = await Int32EntityIdSyntaxGenerator.GenerateStructAsync(context);

            // Assert
            AssertHasProperty(
                @struct,
                name: "HasValue",
                type: "bool",
                (member) =>
                {
                    AssertHasModifier(member, SyntaxKind.PublicKeyword);

                    AssertHasAccessor(member, SyntaxKind.GetAccessorDeclaration);
                    AssertDoesNotHaveAccessor(member, SyntaxKind.SetAccessorDeclaration);
                });
        }

        [Fact]
        public async Task GenerateStructAsync_GeneratesStruct_WithValueProperty()
        {
            // Arrange
            var context = new EntityIdGenerationContext(
                SyntaxFactory.StructDeclaration("EntityId"),
                typeConverter: false,
                jsonNetConverter: false);

            // Act
            var @struct = await Int32EntityIdSyntaxGenerator.GenerateStructAsync(context);

            // Assert
            AssertHasProperty(
                @struct,
                name: "Value",
                type: "int",
                (member) =>
                {
                    AssertHasModifier(member, SyntaxKind.PublicKeyword);

                    AssertHasAccessor(member, SyntaxKind.GetAccessorDeclaration);
                    AssertDoesNotHaveAccessor(member, SyntaxKind.SetAccessorDeclaration);
                });
        }

        private void AssertHasModifier(BaseTypeDeclarationSyntax type, SyntaxKind modifier)
        {
            Assert.Contains(
                type.Modifiers,
                t => t.Kind() == modifier
            );
        }

        private void AssertHasModifier(BaseFieldDeclarationSyntax member, SyntaxKind modifier)
        {
            Assert.Contains(
                member.Modifiers,
                t => t.Kind() == modifier
            );
        }

        private void AssertHasModifier(BasePropertyDeclarationSyntax member, SyntaxKind modifier)
        {
            Assert.Contains(
                member.Modifiers,
                t => t.Kind() == modifier
            );
        }

        private void AssertHasAttribute(StructDeclarationSyntax @struct, string fullName, Action<AttributeSyntax> asserts = null)
        {
            Assert.Contains(
                @struct.AttributeLists,
                als => als.Attributes.Any(
                    attribute =>
                    {
                        if (attribute.Name.ToString() == fullName)
                        {
                            asserts?.DynamicInvoke(attribute);
                            return true;
                        }
                        return false;
                    }
                )
            );
        }

        private void AssertDoesNotHaveAttribute(StructDeclarationSyntax @struct, string fullName)
        {
            Assert.DoesNotContain(
                @struct.AttributeLists,
                als => als.Attributes.Any(
                    attribute => (attribute.Name.ToString() == fullName)
                )
            );
        }

        private void AssertHasField(
            StructDeclarationSyntax @struct, 
            string name, 
            string type, 
            Action<FieldDeclarationSyntax, VariableDeclaratorSyntax> asserts = null)
        {
            Assert.Contains(
                @struct.Members.OfType<FieldDeclarationSyntax>(),
                fds =>
                {
                    var variable = fds.ChildNodes().OfType<VariableDeclarationSyntax>()
                        .FirstOrDefault(vds => vds.Variables.Any(vd =>
                        {
                            if (vd.Identifier.ValueText == name && vds.Type.ToString() == type)
                            {
                                asserts.DynamicInvoke(fds, vd);
                                return true;
                            }

                            return false;
                        }));

                    return (variable is object);
                });
        }

        private void AssertHasProperty(
            StructDeclarationSyntax @struct,
            string name,
            string type,
            Action<PropertyDeclarationSyntax> asserts = null)
                {
                    Assert.Contains(
                        @struct.Members.OfType<PropertyDeclarationSyntax>(),
                        pds =>
                        {
                            if (pds.Identifier.ValueText == name && pds.Type.ToString() == type)
                            {
                                asserts.DynamicInvoke(pds);

                                return true;
                            }

                            return false;
                        });
                }

        private void AssertHasAccessor(BasePropertyDeclarationSyntax property, SyntaxKind kind)
        {
            Assert.NotNull(property.AccessorList);
            Assert.Contains(
                property.AccessorList.Accessors,
                ads => ads.Kind() == kind);
        }

        private void AssertDoesNotHaveAccessor(BasePropertyDeclarationSyntax property, SyntaxKind kind)
        {
            if (property.AccessorList is object)
            {
                Assert.DoesNotContain(
                    property.AccessorList.Accessors,
                    ads => ads.Kind() == kind);
            }
        }
    }
}
