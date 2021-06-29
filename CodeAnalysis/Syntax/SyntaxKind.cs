namespace Jvav.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        //Tokens
        LiteralToken,
        WhitespaceToken,
        SlashToken,
        MinusToken,
        MultiplicationToken,
        PlusToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndToken,
        IdentifierToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EqualsEqualsToken,
        BangEqualsToken,

        //Expressions
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        LiteralExpression,

        //Keywords
        TrueKeyword,
        FalseKeyword,
        NameExpression,
        AssignmentExpression,
        EqualsToken
    }
}