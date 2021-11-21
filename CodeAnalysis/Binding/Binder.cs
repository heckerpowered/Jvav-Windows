using Jvav.CodeAnalysis.Syntax;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Jvav.CodeAnalysis.Binding;

public sealed class Binder
{
    private readonly DiagnosticBag _diagnostic = new();
    private BoundScope _scope;

    public Binder(BoundScope parent)
    {
        _scope = new(parent);
    }

    public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
    {
        var parentScope = CreateParentScopes(previous);
        Binder binder = new(parentScope);
        var expression = binder.BindStatement(syntax.Statement);
        ImmutableArray<VariableSymbol> variables = binder._scope.GetDeclaredVariables();
        ImmutableArray<Diagnostic> diagnostics = binder.Diagnostic.ToImmutableArray();

        if (previous != null)
            diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

        return new(previous, diagnostics, variables, expression);
    }

    private static BoundScope CreateParentScopes(BoundGlobalScope previous)
    {
        Stack<BoundGlobalScope> stack = new();
        while (previous != null)
        {
            stack.Push(previous);
            previous = previous.Previous;
        }

        BoundScope parent = null;

        while (stack.Count > 0)
        {
            previous = stack.Pop();
            BoundScope scope = new(parent);
            foreach (VariableSymbol v in previous.Variables)
                scope.TryDeclare(v);

            parent = scope;
        }

        return parent;
    }
    public DiagnosticBag Diagnostic => _diagnostic;

    public BoundStatement BindStatement(StatementSyntax syntax)
    {
        return syntax.Kind switch
        {
            SyntaxKind.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
            SyntaxKind.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)syntax),
            SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)syntax),
            SyntaxKind.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax),
            SyntaxKind.WhileStatement => BindWhileStatement((WhileStatementSyntax)syntax),
            SyntaxKind.ForStatement => BindForStatement((ForStatementSyntax)syntax),
            _ => throw new Exception($"Unexcepted syntax {syntax.Kind}"),
        };
    }

    private BoundForStatement BindForStatement(ForStatementSyntax syntax)
    {
        var lowerBound = BindExpression(syntax.LowerBound, typeof(int));
        var upperBound = BindExpression(syntax.UpperBound, typeof(int));

        _scope = new BoundScope(_scope);

        var name = syntax.Identifier.Text;
        var variable = new VariableSymbol(name,true,typeof(int));
        if (!_scope.TryDeclare(variable))
        {
            _diagnostic.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
        }

        var body = BindStatement(syntax.Body);

        _scope = _scope.Parent;

        return new BoundForStatement(variable, lowerBound, upperBound, body);
    }

    private BoundWhileStatement BindWhileStatement(WhileStatementSyntax syntax)
    {
        var condition = BindExpression(syntax.Condition,typeof(bool));
        var body = BindStatement(syntax.Body);
        return new BoundWhileStatement(condition, body);
    }

    private BoundIfStatement BindIfStatement(IfStatementSyntax syntax)
    {
        var condition = BindExpression(syntax.Condition,typeof(bool));
        var thenStatement = BindStatement(syntax.ThenStatement);
        var elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement);
        return new BoundIfStatement(condition, thenStatement, elseStatement);
    }

    private BoundExpression BindExpression(ExpressionSyntax syntax, Type targetType)
    {
        var result = BindExpression(syntax);
        if(result.Type != targetType)
        {
            _diagnostic.ReportCannotConvert(syntax.Span, result.Type, targetType);
        }

        return result;
    }

    private BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax syntax)
    {
        var name = syntax.Identifier.Text;
        var isReadOnly = syntax.Keyword.Kind == SyntaxKind.LetKeyword;
        var initializer = BindExpression(syntax.Initializer);
        var variable = new VariableSymbol(name, isReadOnly, initializer.Type);

        if (!_scope.TryDeclare(variable))
        {
            _diagnostic.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
        }

        return new BoundVariableDeclaration(variable, initializer);
    }

    public BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        return syntax.Kind switch
        {
            SyntaxKind.ParenthesizedExpression => BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),
            SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
            SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
            SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
            SyntaxKind.NameExpression => BindNameExpression((NameExpressionSyntax)syntax),
            SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
            _ => throw new Exception($"Unexcepted syntax {syntax.Kind}"),
        };
    }
    private BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax)
    {
        ImmutableArray<BoundStatement>.Builder statements = ImmutableArray.CreateBuilder<BoundStatement>();
        _scope = new BoundScope(_scope);

        foreach (StatementSyntax statementSyntax in syntax.Statements)
        {
            BoundStatement statement = BindStatement(statementSyntax);
            statements.Add(statement);
        }

        _scope = _scope.Parent;

        return new BoundBlockStatement(statements.ToImmutable());
    }

    private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
    {
        var expression = BindExpression(syntax.Expression);
        return new BoundExpressionStatement(expression);
    }

    private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
    {
        string name = syntax.IdentifierToken.Text;
        BoundExpression boundExpression = BindExpression(syntax.Expression);

        if (!_scope.TryLookup(name, out VariableSymbol variable))
        { 
            _diagnostic.ReportUndefinedName(syntax.IdentifierToken.Span,name);
            return boundExpression;
        }

        if (variable.IsReadOnly)
        {
            _diagnostic.ReportCannotAssign(syntax.EqualsToken.Span, name);
        }

        if (boundExpression.Type != variable.Type)
        {
            _diagnostic.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
            return boundExpression;
        }

        return new BoundAssignmentExpression(variable, boundExpression);
    }

    private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
    {
        string name = syntax.IdentifierToken.Text;

        if (!_scope.TryLookup(name, out VariableSymbol variable))
        {
            _diagnostic.ReportUndefinedName(syntax.IdentifierToken.Span, name);
            return new BoundLiteralExpression(0);
        }

        return new BoundVariableExpression(variable);
    }

    private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
    {
        var boundExpression = BindExpression(syntax.Expression);
        return new BoundParenthesizedExpression(boundExpression);
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

        if (boundOperator == null)
        {
            _diagnostic.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type); ;
            return boundLeft;
        }

        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }

    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

        if (boundOperator == null)
        {
            _diagnostic.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
            return boundOperand;
        }

        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    private static BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }
}
