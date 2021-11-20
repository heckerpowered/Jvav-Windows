using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Jvav.CodeAnalysis.Text;

namespace Jvav.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }
    public virtual TextSpan Span
    {
        get
        {
            var children = GetChildren();
            var first = children.First().Span;
            var last = children.Last().Span;
            return TextSpan.FromBounds(first.Start, last.Start);
        }
    }
    public IEnumerable<SyntaxNode> GetChildren()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
            {
                var child = (SyntaxNode)property.GetValue(this);
                yield return child;
            }
            else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
            {
                var children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                foreach (var child in children)
                    yield return child;
            }
        }
    }

    public void WriteTo(TextWriter writer) => PrettyPrint(writer, this);
    private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
    {
        bool isToConsole = writer == Console.Out;
        string marker = isLast ? "└───" : "├───";

        writer.Write(indent);

        if (isToConsole)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            writer.Write(marker);
            Console.ResetColor();
        }

        if (isToConsole)
            Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Blue : ConsoleColor.Cyan;


        writer.Write(node.Kind);

        if (node is SyntaxToken t && t.Value != null)
        {
            writer.Write(" ");
            writer.Write(t.Value);
        }

        if (isToConsole)
            Console.ResetColor();

        writer.WriteLine();

        indent += isLast ? "    " : "|   ";

        IEnumerable<SyntaxNode> children = node.GetChildren();
        SyntaxNode lastChild = children.LastOrDefault();

        foreach (var child in children)
            PrettyPrint(writer, child, indent, child == lastChild);
    }

    public override string ToString()
    {
        using StringWriter writer = new();
        WriteTo(writer);
        return writer.ToString();
    }
}
