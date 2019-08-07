// Copyright (c) Alium Fx. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alium.Data.CodeGeneration
{
    using System;
    using System.Diagnostics;
    using global::CodeGeneration.Roslyn;

    /// <summary>
    /// Marks a target struct type as an entity ID for code generation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    [CodeGenerationAttribute("Alium.Data.CodeGeneration.EntityIdCodeGenerator,Alium.Data.CodeGeneration")]
    [Conditional("CodeGeneration")]
    public sealed class EntityIdAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of <see cref="EntityIdAttribute"/>.
        /// </summary>
        public EntityIdAttribute()
        {
            BackingType = typeof(int);
        }

        /// <summary>
        /// Gets the backing type.
        /// </summary>
        public Type BackingType { get; set; }

        /// <summary>
        /// Gets whether to genearate a JSON.NET converter.
        /// </summary>
        public bool GenerateJsonNetConverter { get; set; }

        /// <summary>
        /// Gets whether to generate a type converter.
        /// </summary>
        public bool GenerateTypeConverter { get; set; }
    }
}
