using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jvav.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
