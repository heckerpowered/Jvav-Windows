using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Jvav.CodeAnalysis;

namespace Jvav.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endToken)
        {
            Diagnostics = diagnostics;
            Root = root;
            EndToken = endToken;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndToken { get; }
        public static SyntaxTree Parse(string text)
        {
            Parser parser = new(text);
            return parser.Parse();
        }
        public static IEnumerable<SyntaxToken> ParseTokens(string text)
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
}
