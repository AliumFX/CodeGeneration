// Copyright (c) Alium Fx. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alium.Data.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::CodeGeneration.Roslyn;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Alium.CodeGeneration;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Generates the code required to support a entity ID struct.
    /// </summary>
    public class EntityIdCodeGenerator : IRichCodeGenerator
    {
        private readonly Type _backingType;
        private readonly bool _generateTypeConverter;
        private readonly bool _generateJsonNetConverter;

        /// <summary>
        /// Initialises a new instance of <see cref="EntityIdCodeGenerator"/>.
        /// </summary>
        public EntityIdCodeGenerator(AttributeData attribute)
        {
            _backingType = GetBackingType(attribute);
            _generateTypeConverter = GetBool(attribute, nameof(EntityIdAttribute.GenerateTypeConverter), true);
            _generateJsonNetConverter = GetBool(attribute, nameof(EntityIdAttribute.GenerateJsonNetConverter), true);
        }

        /// <inheritdoc />
        public async Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var @struct = (StructDeclarationSyntax)context.ProcessingNode;
            var syntax = SyntaxFactory.SingletonList(await GetSyntaxAsync(@struct));

            var wrapped = syntax.WrapWithAncestors(@struct);

            return wrapped;
        }

        /// <inheritdoc />
        public async Task<RichGenerationResult> GenerateRichAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            return new RichGenerationResult
            {
                Members = await GenerateAsync(context, progress, cancellationToken)
            };
        }

        private Type GetBackingType(AttributeData attribute)
        {
            var backingType = attribute.NamedArguments.FirstOrDefault(
                p => p.Key == nameof(EntityIdAttribute.BackingType));

            if (backingType.Equals(default(KeyValuePair<string, TypedConstant>)))
            {
                return typeof(int);
            }

            return (Type)backingType.Value.Value;
        }

        private bool GetBool(AttributeData attribute, string name, bool @default)
        {
            var boolParameter = attribute.NamedArguments.FirstOrDefault(p => p.Key == name);

            if (boolParameter.Equals(default(KeyValuePair<string, TypedConstant>)))
            {
                return @default;
            }

            return (bool)boolParameter.Value.Value;
        }

        private async Task<MemberDeclarationSyntax> GetSyntaxAsync(StructDeclarationSyntax @struct)
        {
            var context = new EntityIdGenerationContext(
                @struct,
                _backingType,
                typeConverter: _generateTypeConverter,
                jsonNetConverter: _generateJsonNetConverter);

            EntityIdSyntaxGeneratorBase generator = null;

            if (_backingType == typeof(int))
            {
                generator = new Int32EntityIdSyntaxGenerator();
            }

            if (generator is object)
            {
                return await generator.GenerateSyntaxAsync(context);
            }

            throw new InvalidOperationException($"The backing type {_backingType} is not supported for entity ID generation.");
        }
    }
}
