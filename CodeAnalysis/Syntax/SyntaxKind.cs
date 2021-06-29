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
        EqualsToken,

        //Expressions
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        LiteralExpression,
        NameExpression,
        AssignmentExpression,

        //Keywords
        TrueKeyword,
        FalseKeyword,
        
    }
}