﻿using System.Collections.Immutable;

namespace Jvav.CodeAnalysis;

public sealed class EvaluationResult
{
    public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, object value)
    {
        Diagnostics = diagnostics;
        Value = value;
    }

    public ImmutableArray<Diagnostic> Diagnostics { get; }
    public object Value { get; }
}
