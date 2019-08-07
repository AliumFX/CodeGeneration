namespace Alium.CodeGeneration
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="CodeGen"/> type.
    /// </summary>
    public class CodeGenTemplateTests
    {
        [Fact]
        public async Task FromTemplate_ReturnsTemplate_AsTargetSyntax()
        {
            // Arrange
            var provider = TemplateProvider.FromAssembly(
                typeof(CodeGenStructTests).Assembly,
                "Alium.CodeGeneration");

            // Act
            var @class = await CodeGen.FromTemplate<ClassDeclarationSyntax>(provider, "TestClass");

            // Assert
            Assert.NotNull(@class);
        }

        [Fact]
        public async Task FromTemplate_ReturnsTemplate_AsTargetSyntax_WithTagReplacement()
        {
            // Arrange
            var provider = TemplateProvider.FromAssembly(
                typeof(CodeGenStructTests).Assembly,
                "Alium.CodeGeneration");

            // Act
            var @class = await CodeGen.FromTemplate<ClassDeclarationSyntax>(provider, "TemplatedClass",
                new { name = "Test" });

            // Assert
            Assert.NotNull(@class);
        }
    }
}
