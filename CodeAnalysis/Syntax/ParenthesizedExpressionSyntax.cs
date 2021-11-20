using System.Collections.Generic;

namespace Jvav.CodeAnalysis.Syntax;

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
}
