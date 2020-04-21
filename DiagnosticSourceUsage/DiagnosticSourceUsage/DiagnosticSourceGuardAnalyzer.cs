using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DiagnosticSourceUsage
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DiagnosticSourceGuardAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DiagnosticSourceGuard";

        private static readonly LocalizableString NoGuardTitle = new LocalizableResourceString(
            nameof(Resources.NoGuardTitle),
            Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString NoGuardMessageFormat =
            new LocalizableResourceString(nameof(Resources.NoGuardMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString NoGuardDescription =
            new LocalizableResourceString(nameof(Resources.NoGuardDescription), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString MismatchGuardTitle = new LocalizableResourceString(
            nameof(Resources.MismatchGuardTitle),
            Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MismatchGuardMessageFormat =
            new LocalizableResourceString(nameof(Resources.MismatchGuardMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString MismatchGuardDescription =
            new LocalizableResourceString(nameof(Resources.MismatchGuardDescription), Resources.ResourceManager,
                typeof(Resources));

        private const string Category = "Performance";

        private static readonly DiagnosticDescriptor NoGuardRule = new DiagnosticDescriptor(DiagnosticId, NoGuardTitle,
            NoGuardMessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: NoGuardDescription);

        private static readonly DiagnosticDescriptor MismatchGuardRule = new DiagnosticDescriptor(DiagnosticId,
            MismatchGuardTitle, MismatchGuardMessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: MismatchGuardDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(NoGuardRule, MismatchGuardRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;
            if (!invocationExpression.IsDiagnosticSourceMethodInvocation("Write", context.SemanticModel,
                out var writeArgument)) return;

            if (IsGuarded(context, writeArgument, out var isEnabledCalled, out var mismatchIsEnabledSymbol, out var mismatchWriteSymbol)) return;

            context.ReportDiagnostic(isEnabledCalled
                ? Diagnostic.Create(MismatchGuardRule, invocationExpression.GetLocation(), DiagnosticSeverity.Warning,
                    null, null,
                    mismatchWriteSymbol, mismatchIsEnabledSymbol)
                : Diagnostic.Create(NoGuardRule, invocationExpression.GetLocation(), DiagnosticSeverity.Warning, null,
                    null));
        }

        private static bool IsGuarded(SyntaxNodeAnalysisContext context, ArgumentSyntax writeArgument, out bool isEnabledCalled,
            out string mismatchIsEnabledSymbol, out string mismatchWriteSymbol)
        {
            isEnabledCalled = false;
            mismatchIsEnabledSymbol = null;
            mismatchWriteSymbol = null;

            if (writeArgument.Expression is LiteralExpressionSyntax writeLiteral)
            {
                if (MatchingLiteralExists(context, writeLiteral, ref mismatchIsEnabledSymbol, ref mismatchWriteSymbol,
                    ref isEnabledCalled)) return true;
            }
            else
            {
                if (MatchingSymbolExists(context, writeArgument, ref mismatchIsEnabledSymbol, ref mismatchWriteSymbol,
                    ref isEnabledCalled)) return true;
            }

            return false;
        }

        private static bool MatchingSymbolExists(SyntaxNodeAnalysisContext context, ArgumentSyntax writeArgument,
            ref string mismatchIsEnabledSymbol, ref string mismatchWriteSymbol, ref bool isEnabledCalled)
        {
            var writeSymbol = context.SemanticModel.GetSymbolInfo(writeArgument.Expression).Symbol;

            for (var parent = context.Node.Parent; !parent.IsMethodOrPropertyDeclaration(); parent = parent.Parent)
            {
                if (!parent.IsKind(SyntaxKind.IfStatement)) continue;

                foreach (var ifInvocation in parent.ChildNodes().OfType<InvocationExpressionSyntax>())
                {
                    if (!ifInvocation.IsDiagnosticSourceMethodInvocation("IsEnabled", context.SemanticModel,
                        out var isEnabledArgument)) continue;

                    var isEnabledSymbol = context.SemanticModel.GetSymbolInfo(isEnabledArgument.Expression).Symbol;
                    if (writeSymbol.Equals(isEnabledSymbol)) return true;
                    if (mismatchIsEnabledSymbol == null)
                    {
                        mismatchIsEnabledSymbol = isEnabledSymbol.Name;
                        mismatchWriteSymbol = writeSymbol.Name;
                    }

                    isEnabledCalled = true;
                }
            }

            return false;
        }

        private static bool MatchingLiteralExists(SyntaxNodeAnalysisContext context, LiteralExpressionSyntax writeLiteral,
            ref string mismatchIsEnabledSymbol, ref string mismatchWriteSymbol, ref bool isEnabledCalled)
        {
            for (var parent = context.Node.Parent; !parent.IsMethodOrPropertyDeclaration(); parent = parent.Parent)
            {
                if (!parent.IsKind(SyntaxKind.IfStatement)) continue;

                foreach (var ifInvocation in parent.ChildNodes().OfType<InvocationExpressionSyntax>())
                {
                    if (!ifInvocation.IsDiagnosticSourceMethodInvocation("IsEnabled", context.SemanticModel,
                        out var isEnabledArgument)) continue;

                    if (isEnabledArgument.Expression is LiteralExpressionSyntax isEnabledLiteral)
                    {
                        if (writeLiteral.Token.ValueText == isEnabledLiteral.Token.ValueText)
                        {
                            return true;
                        }

                        if (mismatchIsEnabledSymbol == null)
                        {
                            mismatchIsEnabledSymbol = isEnabledLiteral.Token.ValueText;
                            mismatchWriteSymbol = writeLiteral.Token.ValueText;
                        }
                    }

                    isEnabledCalled = true;
                }
            }

            return false;
        }
    }
}