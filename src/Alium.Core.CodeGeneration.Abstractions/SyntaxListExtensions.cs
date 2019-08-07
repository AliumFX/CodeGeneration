// Copyright (c) Alium Fx. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Alium.CodeGeneration
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Provides extensions for the <see cref="SyntaxList{TNode}"/> type.
    /// </summary>
    public static class SyntaxListExtensions
    {
        /// <summary>
        /// Wraps the given syntax with the ancestors nodes provided.
        /// </summary>
        /// <param name=""></param>
        /// <param name="syntax"></param>
        /// <returns></returns>
        public static SyntaxList<MemberDeclarationSyntax> WrapWithAncestors(
            this SyntaxList<MemberDeclarationSyntax> target,
            CSharpSyntaxNode source)
            => source.Ancestors().Aggregate(target, WrapInAncestor);

        private static SyntaxList<MemberDeclarationSyntax> WrapInAncestor(SyntaxList<MemberDeclarationSyntax> generatedMembers, SyntaxNode ancestor)
        {
            switch (ancestor)
            {
                case NamespaceDeclarationSyntax ancestorNamespace:
                    generatedMembers = SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        CopyAsAncestor(ancestorNamespace)
                            .WithMembers(generatedMembers));
                    break;
                case ClassDeclarationSyntax nestingClass:
                    generatedMembers = SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        CopyAsAncestor(nestingClass)
                            .WithMembers(generatedMembers));
                    break;
                case StructDeclarationSyntax nestingStruct:
                    generatedMembers = SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        CopyAsAncestor(nestingStruct)
                            .WithMembers(generatedMembers));
                    break;
            }
            return generatedMembers;
        }

        private static NamespaceDeclarationSyntax CopyAsAncestor(NamespaceDeclarationSyntax syntax) 
            => SyntaxFactory.NamespaceDeclaration(syntax.Name.WithoutTrivia())
                .WithExterns(SyntaxFactory.List(syntax.Externs.Select(x => x.WithoutTrivia())))
                .WithUsings(SyntaxFactory.List(syntax.Usings.Select(x => x.WithoutTrivia())));

        private static ClassDeclarationSyntax CopyAsAncestor(ClassDeclarationSyntax syntax)
            => SyntaxFactory.ClassDeclaration(syntax.Identifier.WithoutTrivia())
                .WithModifiers(SyntaxFactory.TokenList(syntax.Modifiers.Select(x => x.WithoutTrivia())))
                .WithTypeParameterList(syntax.TypeParameterList);

        private static StructDeclarationSyntax CopyAsAncestor(StructDeclarationSyntax syntax) 
            => SyntaxFactory.StructDeclaration(syntax.Identifier.WithoutTrivia())
                .WithModifiers(SyntaxFactory.TokenList(syntax.Modifiers.Select(x => x.WithoutTrivia())))
                .WithTypeParameterList(syntax.TypeParameterList);
    }
}
