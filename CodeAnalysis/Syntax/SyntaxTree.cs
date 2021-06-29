using System.Collections.Generic;
using System.Linq;

using Jvav.CodeAnalysis;

namespace Jvav.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(DiagnosticBag diagnostics, ExpressionSyntax root, SyntaxToken endToken)
        {
            Diagnostics = diagnostics;
            Root = root;
            EndToken = endToken;
        }

        public DiagnosticBag Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndToken { get; }
        public static SyntaxTree Parse(string text)
        {
            Parser parser = new(text);
            return parser.Parse();
        }
    }
}
