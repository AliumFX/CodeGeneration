// Copyright (c) Alium Fx. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alium.Data.CodeGeneration
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Generates the syntax for an Int32-backed Entity ID.
    /// </summary>
    public class Int32EntityIdSyntaxGenerator : EntityIdSyntaxGeneratorBase
    {
        /// <inheritdoc />
        public override Task<StructDeclarationSyntax> GenerateSyntaxAsync(EntityIdGenerationContext context)
            => GenerateStructAsync(context);
    }
}
