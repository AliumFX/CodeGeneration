// Copyright (c) Alium Fx. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alium.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    /// <summary>
    /// Provides extensions for Roslyn code syntax
    /// </summary>
    public static partial class CodeGen
    {
        public static async Task<TSyntax> FromTemplate<TSyntax>(TemplateProvider provider, string name, object templateData = null)
            where TSyntax : CSharpSyntaxNode
        {
            provider = provider ?? throw new ArgumentNullException(nameof(provider));
            name = name ?? throw new ArgumentNullException(name, nameof(name));

            string template = await provider.ReadTemplate(name, templateData);
            if (string.IsNullOrEmpty(template))
            {
                throw new ArgumentException($"The template provided by '{name}' does not represent a valid C# syntax tree.");
            }

            var tree = CSharpSyntaxTree.ParseText(template);
            var root = tree.GetRootAsync();

            if (root.Result is CompilationUnitSyntax compilationUnit)
            {
                var member = compilationUnit.Members.OfType<TSyntax>().FirstOrDefault();
                if (member is object)
                {
                    return member;
                }
            }

            throw new InvalidOperationException($"The template does not represent a {typeof(TSyntax).Name}.");
        }

        /// <summary>
        /// Generates an attribute.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <param name="arguments">The set of arguments.</param>
        /// <returns>The attribute syntax.</returns>
        public static AttributeSyntax Attribute(
            NameSyntax name,
            AttributeArgumentSyntax[] arguments = null)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));

            var attribute = SyntaxFactory.Attribute(name);

            if (arguments is object)
            {
                if (arguments.Length > 1)
                {
                    var nodes = new List<SyntaxNodeOrToken>();
                    var first = arguments.First();
                    foreach (var argument in arguments)
                    {
                        if (argument != first)
                        {
                            nodes.Add(Token(SyntaxKind.CommaToken));
                        }
                        nodes.Add(argument);
                    }

                    attribute = attribute.WithArgumentList(
                        AttributeArgumentList(
                            SeparatedList<AttributeArgumentSyntax>(
                                nodes
                            )
                        )
                    );
                }
                else
                {
                    attribute = attribute.WithArgumentList(
                        AttributeArgumentList(
                            SingletonSeparatedList<AttributeArgumentSyntax>(
                                arguments[0]
                            )
                        )
                    );
                }
            }

            return attribute;
        }

        /// <summary>
        /// Generates a list of attributes.
        /// </summary>
        /// <param name="attributes">The set of attributes.</param>
        /// <param name="joined">[Optional] Should the attributes be joined in the same set?</param>
        /// <returns></returns>
        public static AttributeListSyntax[] Attributes(
            IEnumerable<AttributeSyntax> attributes,
            bool joined = false)
        {
            if (joined)
            {
                var nodes = new List<SyntaxNodeOrToken>();
                var first = attributes.First();
                foreach (var attribute in attributes)
                {
                    if (attribute != first)
                    {
                        nodes.Add(Token(SyntaxKind.CommaToken));
                    }
                    nodes.Add(attribute);
                }

                return new[] {
                    AttributeList(
                        SeparatedList<AttributeSyntax>(nodes.ToArray())
                    )
                };
            }

            return attributes.Select(attribute =>
                AttributeList(
                    SingletonSeparatedList<AttributeSyntax>(attribute)
                )
            ).ToArray();
        }

        /// <summary>
        /// Creates a member declaration syntax that represents a field.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="type">The field type.</param>
        /// <param name="initialiser">[Optional] The field initialiser.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>The member declaration syntax.</returns>
        public static FieldDeclarationSyntax Field(
            string name,
            TypeSyntax type,
            SyntaxKind[] modifiers = null,
            ExpressionSyntax initialiser = null)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            type = type ?? throw new ArgumentNullException(nameof(type));

            var variable = VariableDeclarator(Identifier(name));
            if (initialiser is object)
            {
                variable = variable.WithInitializer(
                    EqualsValueClause(initialiser)
                );
            }

            var member = FieldDeclaration(
                VariableDeclaration(type).WithVariables(
                    SingletonSeparatedList(variable)
                )
            );

            if (modifiers is object && modifiers.Length > 0)
            {
                member = member.WithModifiers(
                    TokenList(
                        modifiers.Select(m => Token(m)).ToArray()
                    )
                );
            }

            return member;
        }

        /// <summary>
        /// Creates a name syntax for the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="namespace">[Optional] The namespace.</param>
        /// <param name="global">[Optional] Prefix the qualified name with the global prefix?</param>
        /// <returns>The name syntax</returns>
        public static NameSyntax Name(
            string name,
            string @namespace = null,
            bool global = false)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            NameSyntax ns = null;
            IdentifierNameSyntax g = null;

            if (global)
            {
                g = IdentifierName(Token(SyntaxKind.GlobalKeyword));
            }

            if (@namespace is string)
            {
                var parts = @namespace.Split('.');
                ns = parts.Aggregate<string, NameSyntax>(
                    null,
                    (current, next) => {
                        if (current is null)
                        {
                            if (g is object)
                            {
                                return AliasQualifiedName(
                                    g,
                                    IdentifierName(next));
                            }
                            return IdentifierName(next);
                        }
                        return QualifiedName(current, IdentifierName(next));
                    });
            }

            var n = IdentifierName(name);
            if (ns is object)
            {
                return QualifiedName(ns, n);
            }
            else if (g is object)
            {
                return AliasQualifiedName(g, n);
            }

            return n;
        }

        /// <summary>
        /// Creates a member declaration syntax that represents a property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="type">The property type.</param>
        /// <param name="modifiers">[Optional] The modifiers.</param>
        /// <param name="getter">[Optional] The getter.</param>
        /// <param name="setter">[Optional] The setter.</param>
        /// <returns>The property declaration syntax.</returns>
        public static PropertyDeclarationSyntax Property(
            string name, 
            TypeSyntax type,
            SyntaxKind[] modifiers = null,
            AccessorDeclarationSyntax getter = null,
            AccessorDeclarationSyntax setter = null)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            type = type ?? throw new ArgumentNullException(nameof(type));

            var member = PropertyDeclaration(type, name);

            if (modifiers is object && modifiers.Length > 0)
            {
                member = member.WithModifiers(
                    TokenList(
                        modifiers.Select(m => Token(m)).ToArray()
                    )
                );
            }

            if (getter is object || setter is object)
            {
                var accessors = new List<AccessorDeclarationSyntax>();
                if (getter is object)
                {
                    accessors.Add(getter);
                }
                if (setter is object)
                {
                    accessors.Add(setter);
                }
                member = member.WithAccessorList(
                    AccessorList(List(accessors))
                );
            }

            return member;
        }

        /// <summary>
        /// Creates a property access declaration for an auto getter
        /// </summary>
        /// <param name="modifiers">[Optional] The modifiers.</param>
        /// <returns>The accessor declaration.</returns>
        public static AccessorDeclarationSyntax AutoPropertyGetter(SyntaxKind[] modifiers = null)
        {
            var accessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            if (modifiers is object && modifiers.Length > 0)
            {
                accessor = accessor.WithModifiers(
                    TokenList(
                        modifiers.Select(m => Token(m)).ToArray()
                    )
                );
            }

            return accessor;
        }

        /// <summary>
        /// Creates a property access declaration for an auto setter
        /// </summary>
        /// <param name="modifiers">[Optional] The modifiers.</param>
        /// <returns>The accessor declaration.</returns>
        public static AccessorDeclarationSyntax AutoPropertySetter(SyntaxKind[] modifiers = null)
        {
            var accessor = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));


            if (modifiers is object && modifiers.Length > 0)
            {
                accessor = accessor.WithModifiers(
                    TokenList(
                        modifiers.Select(m => Token(m)).ToArray()
                    )
                );
            }

            return accessor;
        }

        /// <summary>
        /// Creates a literal string attribute argument.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="name">[Optional] The argument name.</param>
        /// <returns>The attribute argument syntax.</returns>
        public static AttributeArgumentSyntax StringAttributeArgument(string value, string name = null)
        {
            var argument = AttributeArgument(
                LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    Literal(value)
                )
            );

            if (name is object)
            {
                argument = argument.WithNameEquals(
                    NameEquals(
                        IdentifierName(name)
                    )
                );
            }

            return argument;
        }

        /// <summary>
        /// Creates a struct declaration syntax
        /// </summary>
        /// <param name="name">The struct name.</param>
        /// <param name="modifiers">[Optional] The modifiers.</param>
        /// <returns>The struct declaration syntax.</returns>
        public static StructDeclarationSyntax Struct(string name, SyntaxKind[] modifiers = null)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));

            var @struct = StructDeclaration(name);

            if (modifiers is object && modifiers.Length > 0)
            {
                @struct = @struct.WithModifiers(
                    TokenList(
                        modifiers.Select(m => Token(m)).ToArray()
                    )
                );
            }

            return @struct;
        }

        /// <summary>
        /// Creates a literal string attribute argument.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="name">[Optional] The argument name.</param>
        /// <returns>The attribute argument syntax.</returns>
        public static AttributeArgumentSyntax TypeOfAttributeArgument(string value, string name = null)
        {
            var argument = AttributeArgument(
                TypeOfExpression(
                    IdentifierName(value)
                )
            );

            if (name is object)
            {
                argument = argument.WithNameEquals(
                    NameEquals(
                        IdentifierName(name)
                    )
                );
            }

            return argument;
        }

        /// <summary>
        /// Unifies the given attribute list sets.
        /// </summary>
        /// <param name="members">The attribute list sets.</param>
        /// <returns>The unified list of attribute lists.</returns>
        public static SyntaxList<AttributeListSyntax> UnifyAttributesLists(
            params IEnumerable<AttributeListSyntax>[] attributeLists)
        {
            var empty = SyntaxFactory.List(Enumerable.Empty<AttributeListSyntax>());
            if (attributeLists.Length == 0)
            {
                return empty;
            }

            var master = new List<AttributeListSyntax>();
            foreach (var list in attributeLists)
            {
                master.AddRange(list);
            }

            if (master.Count == 0)
            {
                return empty;
            }
            else if (master.Count == 1)
            {
                return SyntaxFactory.SingletonList(master[0]);
            }

            return SyntaxFactory.List(master);
        }

        /// <summary>
        /// Unifies the given member sets.
        /// </summary>
        /// <param name="members">The member sets.</param>
        /// <returns>The unified list of members.</returns>
        public static SyntaxList<MemberDeclarationSyntax> UnifyMembers(
            params IEnumerable<MemberDeclarationSyntax>[] members)
        {
            var empty = SyntaxFactory.List(Enumerable.Empty<MemberDeclarationSyntax>());
            if (members.Length == 0)
            {
                return empty;
            }

            var master = new List<MemberDeclarationSyntax>();
            foreach (var list in members)
            {
                master.AddRange(list);
            }

            if (master.Count == 0)
            {
                return empty;
            }
            else if (master.Count == 1)
            {
                return SyntaxFactory.SingletonList(master[0]);
            }

            return SyntaxFactory.List(master);
        }
    }
}
