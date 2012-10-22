using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using NQuery.Language.VSEditor.Document;

namespace NQuery.Language.VSEditor
{
    internal sealed class NQuerySyntaxErrorTagger : NQueryErrorTagger
    {
        private readonly INQueryDocument _document;

        public NQuerySyntaxErrorTagger(INQueryDocument document)
            : base(PredefinedErrorTypeNames.SyntaxError)
        {
            _document = document;
            _document.SyntaxTreeInvalidated += DocumentOnSyntaxTreeInvalidated;
            InvalidateTags();
        }

        private void DocumentOnSyntaxTreeInvalidated(object sender, EventArgs eventArgs)
        {
            InvalidateTags();
        }

        protected override async Task<Tuple<ITextSnapshot, IEnumerable<Diagnostic>>> GetRawTagsAsync()
        {
            var syntaxTree = await _document.GetSyntaxTreeAsync();
            var snapshot = _document.GetTextSnapshot(syntaxTree);
            return Tuple.Create(snapshot, syntaxTree.GetDiagnostics());
        }
    }
}