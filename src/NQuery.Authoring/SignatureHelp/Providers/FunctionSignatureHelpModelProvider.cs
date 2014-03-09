using System;
using System.Linq;

using NQuery.Symbols;
using NQuery.Syntax;

namespace NQuery.Authoring.SignatureHelp
{
    // TODO: Could we introduce a common base class between FunctionSignatureHelpModelProvider and MethodSignatureHelpModelProvider?

    internal sealed class FunctionSignatureHelpModelProvider : ISignatureHelpModelProvider
    {
        public SignatureHelpModel GetModel(SemanticModel semanticModel, int position)
        {
            var syntaxTree = semanticModel.Compilation.SyntaxTree;
            var token = syntaxTree.Root.FindTokenOnLeft(position);
            var functionInvocation = token.Parent
                                          .AncestorsAndSelf()
                                          .OfType<FunctionInvocationExpressionSyntax>()
                                          .FirstOrDefault(f => f.IsBetweenParentheses(position));

            if (functionInvocation == null)
                return null;

            // TODO: We need to use the resolved symbol as the currently selected one.

            var name = functionInvocation.Name;
            var functionSignatures = semanticModel.LookupSymbols(name.Span.Start)
                                                  .OfType<FunctionSymbol>()
                                                  .Where(f => name.Matches(f.Name))
                                                  .ToSignatureItems();

            var aggregateSignatures = semanticModel.LookupSymbols(name.Span.Start)
                                                   .OfType<AggregateSymbol>()
                                                   .Where(f => name.Matches(f.Name))
                                                   .ToSignatureItems();

            var signatures = functionSignatures.Concat(aggregateSignatures).OrderBy(s => s.Parameters.Count).ToArray();

            if (signatures.Length == 0)
                return null;

            var span = functionInvocation.Span;
            var parameterIndex = functionInvocation.ArgumentList.GetParameterIndex(position);
            var selected = signatures.FirstOrDefault(s => s.Parameters.Count > parameterIndex);

            return new SignatureHelpModel(span, signatures, selected, parameterIndex);
        }
    }
}