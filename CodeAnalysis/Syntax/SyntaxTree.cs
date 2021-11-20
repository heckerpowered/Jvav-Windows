using System.Collections.Generic;
using System.Collections.Immutable;

using Jvav.CodeAnalysis.Text;

namespace Jvav.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
    private SyntaxTree(SourceText text)
    {
        Parser parser = new(text);
        var root = parser.ParseCompilatioUnit();
        ImmutableArray<Diagnostic> diagnostics = parser.Diagnostics.ToImmutableArray();
        Text = text;
        Diagnostics = diagnostics;
        Root = root;
    }

    public SourceText Text { get; }
    public ImmutableArray<Diagnostic> Diagnostics { get; }
    public CompilationUnitSyntax Root { get; }
    public static SyntaxTree Parse(string text)
    {
        var sourceText = SourceText.From(text);
        return Parse(sourceText);
    }
    public static SyntaxTree Parse(SourceText text)
    {
        return new SyntaxTree(text);
    }
    public static IEnumerable<SyntaxToken> ParseTokens(string text)
    {
        var sourceText = SourceText.From(text);
        return ParseTokens(sourceText);
    }
    public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
    {
        var lexer = new Lexer(text);
        while (true)
        {
            var token = lexer.Lex();
            if (token.Kind == SyntaxKind.EndToken)
                break;

            yield return token;
        }
    }
}
