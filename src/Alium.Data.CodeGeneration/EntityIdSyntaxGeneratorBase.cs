// Copyright (c) Alium Fx. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alium.Data.CodeGeneration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Alium.CodeGeneration;

    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using static Alium.CodeGeneration.CodeGen;

    /// <summary>
    /// Provides a base implementation of an entity ID syntax generator.
    /// </summary>
    public abstract class EntityIdSyntaxGeneratorBase
    {
        /// <summary>
        /// Initialises a new instance of <see cref="EntityIdSyntaxGeneratorBase"/>.
        /// </summary>
        public EntityIdSyntaxGeneratorBase()
        {
            DataTemplateProvider = TemplateProvider.FromAssembly(
                typeof(EntityIdSyntaxGeneratorBase).Assembly,
                "Alium.Data.CodeGeneration");
        }

        /// <summary>
        /// Generates the required syntax representing a backed entity ID
        /// </summary>
        /// <param name="context">The entitity ID generation context.</param>
        /// <returns>The struct declaration syntax.</returns>
        public abstract Task<StructDeclarationSyntax> GenerateSyntaxAsync(EntityIdGenerationContext context);

        /// <summary>
        /// Gets the data template provider.
        /// </summary>
        protected TemplateProvider DataTemplateProvider { get; }

        protected async Task<StructDeclarationSyntax> GenerateStructAsync(EntityIdGenerationContext context)
        {
            // MA - Generate the majority of the syntax from a template.
            var @struct = await FromTemplate<StructDeclarationSyntax>(
                DataTemplateProvider,
                $"{context.TemplatePrefix}EntityIdStruct",
                new
                {
                    name = context.Name
                });

            // MA - Merge attributes and members based on configuration.
            return @struct
                .WithAttributeLists(
                    UnifyAttributesLists(@struct.AttributeLists, GenerateAttributes(context))
                )
                .WithMembers(
                    UnifyMembers(@struct.Members, await GenerateMembersAsync(context))
                );
        }

        protected SyntaxList<AttributeListSyntax> GenerateAttributes(EntityIdGenerationContext context)
        {
            var attributes = new List<AttributeSyntax>();

            if (context.TypeConverter)
            {
                attributes.Add(
                    // TypeConverter(typeof({name}TypeConverter))
                    Attribute(
                        Name("TypeConverter", "System.ComponentModel"),
                        new[]
                        {
                            TypeOfAttributeArgument($"{context.Name}TypeConverter")
                        }
                    )
                );
            }

            if (context.JsonNetConverter)
            {
                attributes.Add(
                    // JsonConverter(typeof({name}JsonConverter))
                    Attribute(
                        Name("JsonConverter", "Newtonsoft.Json"),
                        new[]
                        {
                            TypeOfAttributeArgument($"{context.Name}JsonConverter")
                        }
                    )
                );
            }

            return List(Attributes(attributes));
        }

        protected async Task<SyntaxList<MemberDeclarationSyntax>> GenerateMembersAsync(EntityIdGenerationContext context)
        {
            var members = new List<MemberDeclarationSyntax>();
            var templateData = new
            {
                name = context.Name
            };

            if (context.TypeConverter)
            {
                var typeConverter = await CodeGen.FromTemplate<ClassDeclarationSyntax>(
                    DataTemplateProvider,
                    $"{context.TemplatePrefix}EntityIdStructTypeConverter",
                    templateData);

                members.Add(typeConverter);
            }

            if (context.JsonNetConverter)
            {
                var jsonConverter = await CodeGen.FromTemplate<ClassDeclarationSyntax>(
                    DataTemplateProvider,
                    $"{context.TemplatePrefix}EntityIdStructJsonConverter",
                    templateData);

                members.Add(jsonConverter);
            }

            return List(members);
        }
    }
}
