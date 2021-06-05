using Jvav.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jvav.Binding
{
    public sealed class Binder
    {
        private readonly List<string> _diagnostic = new();
        public IEnumerable<string> Diagnostic => _diagnostic;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax.Kind switch
            {
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
                SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
                SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
                _ => throw new Exception($"Unexcepted syntax {syntax.Kind}"),
            };
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperatorKind = BindBinaryOperatorKind(syntax.OperatorToken.Kind,boundLeft.Type,boundRight.Type);

            if (boundOperatorKind == null)
            {
                _diagnostic.Add($"Binary operator '{syntax.OperatorToken.Text}' is not defined for types '{boundLeft.Type}' and '{boundLeft.Type}'.");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorKind.Value, boundRight);
        }

        private BoundBinaryExpressionKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            if (leftType == typeof(int) && rightType == typeof(int))
                return kind switch
                {
                    SyntaxKind.PlusToken => BoundBinaryExpressionKind.Addition,
                    SyntaxKind.MinusToken => BoundBinaryExpressionKind.Subtraction,
                    SyntaxKind.MultiplicationToken => BoundBinaryExpressionKind.Multiplication,
                    SyntaxKind.SlashToken => BoundBinaryExpressionKind.Division,
                    _ => null
                };
            if(leftType == typeof(bool) && rightType == typeof(bool))
            {
                return kind switch
                {
                    SyntaxKind.AmpersandAmpersandToken => BoundBinaryExpressionKind.LogicalAnd,
                    SyntaxKind.BangToken => BoundBinaryExpressionKind.LogicalOr,
                    _ => null
                };
            }
            return null;
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorKind = BindUnaryOperatorKind(syntax.OperatorToken.Kind, boundOperand.Type);

            if(boundOperatorKind == null)
            {
                _diagnostic.Add($"Unary operator '{syntax.OperatorToken.Text}' is not defined for type '{boundOperand.Type}'.");
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperatorKind.Value, boundOperand);
        }

        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type type)
        {
            if (type == typeof(int))
                return kind switch
                {
                    SyntaxKind.PlusToken => BoundUnaryOperatorKind.Identity,
                    SyntaxKind.MinusToken => BoundUnaryOperatorKind.Negation,
                    _ => throw new Exception($"Unexpected unary operator {kind}"),
                };
            if (type == typeof(bool))
                switch (kind) {
                    case SyntaxKind.BangToken:
                        return BoundUnaryOperatorKind.LogicalNegation;
                };
            return null;
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }
    }
}
