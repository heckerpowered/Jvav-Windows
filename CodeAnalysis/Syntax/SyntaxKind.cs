namespace Jvav.CodeAnalysis.Syntax;

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
    CloseBraceToken,
    OpenBraceToken,
    LessToken,
    LessOrEqualsToken,
    GreaterToken,
    GreaterOrEqualsToken,
    AmpersandToken,
    PipeToken,
    TildeToken,
    HatToken,

    //Nodes
    CompilationUnit,
    ElseClause,

    //Statements
    BlockStatement,
    ExpressionStatement,
    VariableDeclaration,
    IfStatement,
    WhileStatement,

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
    LetKeyword,
    VarKeyword,
    IfKeyword,
    ElseKeyword,
    WhileKeyword,
    ForKeyword, 
    ForStatement,
    ToKeyword,
}
