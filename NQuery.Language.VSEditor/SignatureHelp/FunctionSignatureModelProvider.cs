using System.ComponentModel.Composition;
using System.Linq;

using NQuery.Language.Symbols;

namespace NQuery.Language.VSEditor.SignatureHelp
{
    // TODO: Could we introduce a common base class between FunctionSignatureModelProvider and MethodSignatureModelProvider?

    [Export(typeof(ISignatureModelProvider))]
    internal sealed class FunctionSignatureModelProvider : ISignatureModelProvider
    {
        public SignatureHelpModel GetModel(SemanticModel semanticModel, int position)
        {
            var syntaxTree = semanticModel.Compilation.SyntaxTree;
            var token = syntaxTree.Root.FindTokenTouched(position, descendIntoTrivia: true);
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
            var selected = signatures.FirstOrDefault();
            var parameterIndex = functionInvocation.ArgumentList.GetParameterIndex(position);

            return new SignatureHelpModel(span, signatures, selected, parameterIndex);
        }
    }
}