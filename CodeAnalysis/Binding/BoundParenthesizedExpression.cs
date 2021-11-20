using System;
using System.Linq.Expressions;

namespace Jvav.CodeAnalysis.Binding;

internal class BoundParenthesizedExpression : BoundExpression
{
    public BoundParenthesizedExpression(BoundExpression expression)
    {
        Expression = expression;
    }

    public override Type Type => Expression.Type;

    public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;

    public BoundExpression Expression { get; }
}
