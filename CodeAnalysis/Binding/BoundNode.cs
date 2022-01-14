using Jvav.CodeAnalysis.Syntax;
using Jvav.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using System.Linq;

namespace Jvav.CodeAnalysis.Binding;

public abstract class BoundNode
{
    public abstract BoundNodeKind Kind { get; }
    public IEnumerable<BoundNode> GetChildren()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
            {
                var child = (BoundNode)property.GetValue(this);
                if (child != null)
                    yield return child;
            }
            else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
            {
                var children = (IEnumerable<BoundNode>)property.GetValue(this);
                foreach (var child in children)
                {
                    if (child != null)
                        yield return child;
                }
            }
        }
    }

    public void WriteTo(TextWriter writer) => PrettyPrint(writer, this);
    private static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
    {
        bool isToConsole = writer == Console.Out;
        string marker = isLast ? "└───" : "├───";

        writer.Write(indent);

        if (isToConsole)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        writer.Write(indent);
        writer.Write(marker);

        WriteNode(writer, node);

        if (isToConsole)
            Console.ResetColor();


        writer.Write(node.Kind);

        if (isToConsole)
            Console.ResetColor();

        writer.WriteLine();

        indent += isLast ? "    " : "|   ";

        IEnumerable<BoundNode> children = node.GetChildren();
        BoundNode lastChild = children.LastOrDefault();

        foreach (var child in children)
            PrettyPrint(writer, child, indent, child == lastChild);
    }

    private static void WriteNode(TextWriter writer, BoundNode node)
    {
        //TODO: Handle bianry and unary expressions
        //TODO: Change colors
        writer.Write(' ');
        writer.Write(node.Kind);
    }

    public override string ToString()
    {
        using StringWriter writer = new();
        WriteTo(writer);
        return writer.ToString();
    }
}






