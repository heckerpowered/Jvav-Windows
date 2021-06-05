using System;

namespace Jvav.Binding
{
    public sealed class BoundUnaryOperator
    public sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryExpressionKind operatorKind, BoundExpression right)
        {
            Left = left;
            OperatorKind = operatorKind;
            Right = right;
        }

        public override Type Type => Left.Type;
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

        public BoundExpression Left { get; }
        public BoundBinaryExpressionKind OperatorKind { get; }
        public BoundExpression Right { get; }
    }
}
