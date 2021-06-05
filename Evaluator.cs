using Jvav.Binding;
using Jvav.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jvav.Syntax
{
    public class Evaluator
    {
        private readonly BoundExpression _root;
        public Evaluator(BoundExpression root)
        {
            _root = root;
        }
        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }
        private object EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression n)
                return n.Value;

            if(node is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);
                return u.OperatorKind switch
                {
                    BoundUnaryOperatorKind.Identity => (int)operand,
                    BoundUnaryOperatorKind.Negation => -(int)operand,
                    BoundUnaryOperatorKind.LogicalNegation => !(bool)operand,
                    _ => throw new Exception($"Unexpected unary operator '{u.OperatorKind}'")
                };
            }

            if(node is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);
                return b.OperatorKind switch
                {
                    BoundBinaryExpressionKind.Addition => (int)left + (int)right,
                    BoundBinaryExpressionKind.Subtraction => (int)left - (int)right,
                    BoundBinaryExpressionKind.Multiplication => (int)left * (int)right,
                    BoundBinaryExpressionKind.Division => (int)left / (int)right,
                    BoundBinaryExpressionKind.LogicalAnd => (bool)left && (bool)right,
                    BoundBinaryExpressionKind.LogicalOr => (bool)left || (bool)right,
                    _ => throw new Exception($"Unexpected binary operator '{b.OperatorKind}'")
                };
            }

            throw new Exception($"Unexpected node '{node.Kind}'");
        }
    }
}
