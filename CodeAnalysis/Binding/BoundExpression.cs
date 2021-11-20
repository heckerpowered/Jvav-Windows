using System;

namespace Jvav.CodeAnalysis.Binding;

public abstract class BoundExpression : BoundNode
{
    public abstract Type Type { get; }
}
