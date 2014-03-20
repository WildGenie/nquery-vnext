﻿using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NQuery.Authoring.QuickInfo;
using NQuery.Symbols;
using NQuery.Syntax;

namespace NQuery.Authoring.UnitTests.QuickInfo.Providers
{
    [TestClass]
    public class VariableExpressionQuickInfoModelProviderTests : QuickInfoModelProviderTests
    {
        protected override IQuickInfoModelProvider CreateProvider()
        {
            return new VariableExpressionQuickInfoModelProvider();
        }

        protected override QuickInfoModel CreateExpectedModel(SemanticModel semanticModel)
        {
            var syntaxTree = semanticModel.Compilation.SyntaxTree;
            var syntax = syntaxTree.Root.DescendantNodes().OfType<VariableExpressionSyntax>().Single();
            var span = syntax.Span;
            var symbol = semanticModel.GetSymbol(syntax);
            var markup = SymbolMarkup.ForSymbol(symbol);
            return new QuickInfoModel(semanticModel, span, NQueryGlyph.Variable, markup);
        }

        [TestMethod]
        public void VariableExpressionQuickInfoModelProvider_MatchesInName()
        {
            var query = @"
                SELECT  *
                FROM    Employees e
                WHERE   e.EmployeeId = @{EmployeeId}
             ";

            AssertIsMatch(query, dc => dc.AddVariables(new VariableSymbol("EmployeeId", typeof(int))));
        }

        [TestMethod]
        public void VariableExpressionQuickInfoModelProvider_MatchesInAt()
        {
            var query = @"
                SELECT  *
                FROM    Employees e
                WHERE   e.EmployeeId = {@}EmployeeId
            ";

            AssertIsMatch(query, dc => dc.AddVariables(new VariableSymbol("EmployeeId", typeof(int))));
        }

        [TestMethod]
        public void VariableExpressionQuickInfoModelProvider_DoesNotMatchForUnresolved()
        {
            var query = @"
                SELECT  *
                FROM    Employees e
                WHERE   e.EmployeeId = {@EmployeeId}
            ";

            AssertIsNotMatch(query);
        }
    }
}