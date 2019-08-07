// Copyright (c) Alium Fx. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alium.Data.CodeGeneration
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Provides a context for generating an entity ID.
    /// </summary>
    public class EntityIdGenerationContext
    {
        /// <summary>
        /// Initialises a new instance of <see cref="EntityIdGenerationContext"/>.
        /// </summary>
        /// <param name="name">The type name.</param>
        public EntityIdGenerationContext(
            StructDeclarationSyntax sourceDeclarataion,
            Type backingType,
            bool typeConverter,
            bool jsonNetConverter)
        {
            SourceDeclaration = sourceDeclarataion ?? throw new ArgumentNullException(nameof(sourceDeclarataion));
            BackingType = backingType ?? throw new ArgumentNullException(nameof(backingType));
            Name = sourceDeclarataion.Identifier.ValueText;
            TypeConverter = typeConverter;
            JsonNetConverter = jsonNetConverter;
        }

        /// <summary>
        /// Gets the backing type.
        /// </summary>
        public Type BackingType { get; }

        /// <summary>
        /// Gets whether to generate a JSON.NET converter
        /// </summary>
        public bool JsonNetConverter { get; }

        /// <summary>
        /// Gets the type name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the source declaration.
        /// </summary>
        public StructDeclarationSyntax SourceDeclaration { get; }

        /// <summary>
        /// Gets the template prefix.
        /// </summary>
        public string TemplatePrefix => BackingType.Name;

        /// <summary>
        /// Gets whether to generate a type converter
        /// </summary>
        public bool TypeConverter { get; }
    }
}
