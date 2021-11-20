namespace Jvav.CodeAnalysis.Binding;

public abstract class BoundStatement : BoundNode
{

}

public sealed class BoundVariableDeclaration : BoundStatement
{
    public BoundVariableDeclaration(VariableSymbol variable,BoundExpression initializer)
    {
        Variable = variable;
        Initializer = initializer;
    }
    public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;

    public VariableSymbol Variable { get; }
    public BoundExpression Initializer { get; }
}   