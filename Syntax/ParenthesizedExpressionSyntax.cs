using System.Collections.Generic;

namespace Jvav.Syntax
{
    public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthsisToken, ExpressionSyntax expression, SyntaxToken closeParenthsisToken)
        {
            OpenParenthsisToken = openParenthsisToken;
            Expression = expression;
            CloseParenthsisToken = closeParenthsisToken;
        }
        public SyntaxToken OpenParenthsisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthsisToken { get; }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthsisToken;
            yield return Expression;
            yield return CloseParenthsisToken;
        }
    }
}
