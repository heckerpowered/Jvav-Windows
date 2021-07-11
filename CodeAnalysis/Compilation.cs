using Jvav.CodeAnalysis.Binding;
using Jvav.CodeAnalysis.Syntax;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Jvav.CodeAnalysis
{
    public sealed class Compilation
    {
        public Compilation(SyntaxTree syntax)
        {
            Syntax = syntax;
        }

        public SyntaxTree Syntax { get; }
        public EvaluationResult Evaluate(Dictionary<VariableSymbol,object> variables)
        {
            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(Syntax.Root.Expression);

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostic).ToImmutableArray();
            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics, null);
            }

            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();
            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}
