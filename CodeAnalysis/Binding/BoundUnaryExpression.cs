using System;

namespace Jvav.CodeAnalysis.Binding;

public sealed class BoundUnaryExpression : BoundExpression
{
    public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
    {
        Op = op;
        Operand = operand;
    }

    public override Type Type => Op.ResultType;
    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public BoundUnaryOperator Op { get; }
    public BoundExpression Operand { get; }
}
