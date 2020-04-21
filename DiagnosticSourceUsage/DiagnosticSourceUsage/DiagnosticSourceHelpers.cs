using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiagnosticSourceUsage
{
    internal static class DiagnosticSourceHelpers
    {
        internal static bool IsDiagnosticSourceMethodInvocation(this InvocationExpressionSyntax invocationExpression,
            string methodName, SemanticModel semanticModel, out ArgumentSyntax firstArgument)
        {
            firstArgument = null;
            if (!(invocationExpression.Expression is MemberAccessExpressionSyntax expressionSyntax)) return false;

            if (expressionSyntax.Name.Identifier.Text != methodName) return false;

            if (!(semanticModel.GetSymbolInfo(expressionSyntax).Symbol is IMethodSymbol methodSymbol)) return false;

            if (methodSymbol.ReceiverType is null) return false;

            if ((methodSymbol.ReceiverType?.Name == "DiagnosticSource" || methodSymbol.ReceiverType.Name == "DiagnosticListener") &&
                methodSymbol.ReceiverType.ContainingNamespace?.Name == "Diagnostics" &&
                methodSymbol.ReceiverType.ContainingNamespace.ContainingNamespace?.Name == "System")
            {
                var arguments = invocationExpression.ArgumentList.Arguments;
                if (arguments.Count > 0)
                {
                    firstArgument = arguments[0];
                }

                return true;
            }

            return false;
        }
    }
}