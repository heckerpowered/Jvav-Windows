using System;

namespace Jvav.CodeAnalysis
{
    public struct TextSpan
    {
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;
    }

    public sealed class VariableSymbol
    {
        public VariableSymbol(string name,Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public Type Type { get; }
    }
}
