using System.Collections.Generic;
using System.Linq;

namespace Jvav.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndToken = endToken;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndToken { get; }
        public static SyntaxTree Parse(string text)
        {
            Parser parser = new(text);
            return parser.Parse();
        }
    }
}
