using System;

using NQuery.Binding;
using NQuery.Symbols;

namespace NQuery.BoundNodes
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        private readonly object _value;

        public BoundLiteralExpression(object value)
        {
            _value = value;
        }

        public override BoundNodeKind Kind
        {
            get { return BoundNodeKind.LiteralExpression; }
        }

        public override Type Type
        {
            get
            {
                return _value == null
                           ? TypeFacts.Null
                           : _value.GetType();
            }
        }
    }
}