namespace Jvav.CodeAnalysis.Syntax;

public sealed class ElseCaluseSyntax : SyntaxNode
{
    public ElseCaluseSyntax(SyntaxToken elseKeyword,StatementSyntax elseStatement)
    {
        ElseKeyword = elseKeyword;
        ElseStatement = elseStatement;
    }
    public override SyntaxKind Kind => SyntaxKind.ElseClause;

    public SyntaxToken ElseKeyword { get; }
    public StatementSyntax ElseStatement { get; }
}