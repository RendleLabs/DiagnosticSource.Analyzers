using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DiagnosticSourceUsage
{
    internal static class SyntaxNodeExtensions
    {
        public static bool IsMethodOrPropertyDeclaration(this SyntaxNode node)
            => node.IsKind(SyntaxKind.MethodDeclaration) || node.IsKind(SyntaxKind.PropertyDeclaration);
    }
}
