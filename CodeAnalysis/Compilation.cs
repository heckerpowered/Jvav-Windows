using Jvav.CodeAnalysis.Binding;
using Jvav.CodeAnalysis.Syntax;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace Jvav.CodeAnalysis;

public sealed class Compilation
{
    private BoundGlobalScope _globalScope;
    public Compilation(SyntaxTree syntaxTree) : this(null, syntaxTree)
    {

    }
    private Compilation(Compilation previous, SyntaxTree syntaxTree)
    {
        Previous = previous;
        SyntaxTree = syntaxTree;
    }
    public BoundGlobalScope GlobalScope
    {
        get
        {
            if (_globalScope == null)
            {
                BoundGlobalScope globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                Interlocked.CompareExchange(ref _globalScope, globalScope, null);
            }

            return _globalScope;
        }
    }

    public Compilation ContinueWith(SyntaxTree syntaxTree)
    {
        return new(this, syntaxTree);
    }

    public Compilation Previous { get; }
    public SyntaxTree SyntaxTree { get; }
    public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
    {
        var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
        if (diagnostics.Any())
            return new EvaluationResult(diagnostics, null);

        var evaluator = new Evaluator(GlobalScope.Statement, variables);
        var value = evaluator.Evaluate();
        return new(ImmutableArray<Diagnostic>.Empty, value);
    }

    public void EmitTree(TextWriter writer) 
    {
        GlobalScope.Statement.WriteTo(writer);
    }
}
