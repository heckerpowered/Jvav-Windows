
namespace Jvav.Syntax
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

        //Expressions
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression,
        LiteralExpression,

        //Keywords
        TrueKeyword,
        FalseKeyword,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
    }
}