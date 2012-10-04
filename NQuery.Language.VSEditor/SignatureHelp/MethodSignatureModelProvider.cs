using System.ComponentModel.Composition;
using System.Linq;

namespace NQuery.Language.VSEditor.SignatureHelp
{
    [Export(typeof(ISignatureModelProvider))]
    internal sealed class MethodSignatureModelProvider : ISignatureModelProvider
    {
        public SignatureHelpModel GetModel(SemanticModel semanticModel, int position)
        {
            var syntaxTree = semanticModel.Compilation.SyntaxTree;
            var token = syntaxTree.Root.FindTokenTouched(position, descendIntoTrivia: true);
            var methodInvocation = token.Parent
                                        .AncestorsAndSelf()
                                        .OfType<MethodInvocationExpressionSyntax>()
                                        .FirstOrDefault(m => m.IsBetweenParentheses(position));

            if (methodInvocation == null)
                return null;

            // TODO: We need to use the resolved symbol as the selected one.

            var targetType = semanticModel.GetExpressionType(methodInvocation.Target);
            var name = methodInvocation.Name;
            var signatures = semanticModel.LookupMethods(targetType)
                                          .Where(m => name.Matches(m.Name))
                                          .OrderBy(f => f.Parameters.Count)
                                          .ToSignatureItems()
                                          .ToArray();
            if (signatures.Length == 0)
                return null;

            var span = methodInvocation.Span;
            var selected = signatures.FirstOrDefault();
            var parameterIndex = methodInvocation.ArgumentList.GetParameterIndex(position);

            return new SignatureHelpModel(span, signatures, selected, parameterIndex);
        }
    }
}