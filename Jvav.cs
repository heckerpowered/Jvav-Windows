using Jvav.Binding;
using Jvav.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jvav
{
    public class Jvav
    {
        public static object Evaluate(string text)
        {
            Binder binder = new();
            SyntaxTree syntaxTree = SyntaxTree.Parse(text);
            BoundExpression expression = binder.BindExpression(syntaxTree.Root);
            Evaluator e = new(expression);
            return e.Evaluate();
        }
    }
}
