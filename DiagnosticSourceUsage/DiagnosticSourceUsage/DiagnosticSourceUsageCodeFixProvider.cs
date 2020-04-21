using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace DiagnosticSourceUsage
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DiagnosticSourceUsageCodeFixProvider)), Shared]
    public class DiagnosticSourceUsageCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add IsEnabled guard";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticSourceGuardAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null) return;

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf()
                .OfType<ExpressionStatementSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => AddIsEnabledGuardAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Solution> AddIsEnabledGuardAsync(Document document, ExpressionStatementSyntax expression,
            CancellationToken cancellationToken)
        {
            var memberAccessExpression = expression.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First();
            if (memberAccessExpression.Expression is IdentifierNameSyntax identifierNameSyntax)
            {
                if (expression.Expression is InvocationExpressionSyntax invocation)
                {
                    var leadingTrivia = expression.GetLeadingTrivia();
                    var arg = invocation.ArgumentList.Arguments.First();
                    var source = identifierNameSyntax.Identifier.Text;
                    var isEnabled =
                        SyntaxFactory.ParseExpression($"{source}.IsEnabled()") as InvocationExpressionSyntax;
                    isEnabled = isEnabled.ReplaceNode(isEnabled.ArgumentList, isEnabled.ArgumentList.AddArguments(arg));
                    SyntaxFactory.Token(SyntaxKind.IfKeyword);
                    var ifStatement = SyntaxFactory.IfStatement(
                        SyntaxFactory.Token(SyntaxKind.IfKeyword),
                        SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                        isEnabled,
                        SyntaxFactory.Token(SyntaxKind.CloseParenToken).WithTrailingTrivia(NewLine),
                        Indent(expression), null).WithLeadingTrivia(leadingTrivia);

                    // Produce a new solution that has all references to that type renamed, including the declaration.
                    var originalSolution = document.Project.Solution;
                    var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
                    syntaxRoot = syntaxRoot.ReplaceNode(expression, ifStatement);
                    var newSolution = originalSolution.WithDocumentSyntaxRoot(document.Id, syntaxRoot);
                    return newSolution;
                }
            }

            return document.Project.Solution;
        }

        private static T Indent<T>(T node) where T : SyntaxNode
        {
            var trivia = node.GetLeadingTrivia().AddRange(FourSpaces);
            return node.WithLeadingTrivia(trivia);
        }

        private static readonly SyntaxTrivia[] FourSpaces = Enumerable.Repeat(SyntaxFactory.Space, 4).ToArray();
        private static readonly SyntaxTriviaList NewLine = SyntaxFactory.ParseTrailingTrivia(Environment.NewLine);
    }
}