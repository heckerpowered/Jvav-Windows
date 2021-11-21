using Jvav.CodeAnalysis.Binding;

using System;
using System.Collections.Generic;

namespace Jvav.CodeAnalysis;

public class Evaluator
{
    private readonly BoundStatement _root;
    private readonly Dictionary<VariableSymbol, object> _variables;

    private object _lastValue;

    public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;
    }
    public object Evaluate()
    {
        EvaluateStatement(_root);
        return _lastValue;

    }
    private void EvaluateStatement(BoundStatement node)
    {
        switch (node.Kind)
        {
            case BoundNodeKind.BlockStatement:
                EvaluateBlockStatement((BoundBlockStatement)node);
                break;
            case BoundNodeKind.ExpressionStatement:
                EvaluateExpressionStatement((BoundExpressionStatement)node);
                break;
            case BoundNodeKind.VariableDeclaration:
                EvaluateVariableDeclaration((BoundVariableDeclaration)node);
                break;
            case BoundNodeKind.IfStatement:
                EvaluateIfStatement((BoundIfStatement)node);
                break;
            case BoundNodeKind.WhileStatement:
                EvaluateWhileStatement((BoundWhileStatement)node);
                break;
            case BoundNodeKind.ForStatement:
                EvaluateForStatement((BoundForStatement)node);
                break;
            default:
                throw new Exception($"Unexpected node {node.Kind}");
        }
    }

    private void EvaluateForStatement(BoundForStatement node)
    {
        var lowerBound = (int)EvaluateExpression(node.LowerBound);
        var upperBound = (int)EvaluateExpression(node.UpperBound);

        for(int i = lowerBound; i <= upperBound; i++)
        {
            _variables[node.Variable] = i;
            EvaluateStatement(node.Body);   
        }
    }

    private void EvaluateWhileStatement(BoundWhileStatement node)
    {
        while ((bool)EvaluateExpression(node.Condition))
        {
            EvaluateStatement(node.Body);
        }
    }

    private void EvaluateIfStatement(BoundIfStatement node)
    {
        var condition = (bool)EvaluateExpression(node.Condition);
        if (condition)
        {
            EvaluateStatement(node.ThenStatement);
        }
        else if (node.ElseStatement != null)
        {
            EvaluateStatement(node.ElseStatement);
        }
    }

    private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
    {
        var value = EvaluateExpression(node.Initializer);
        _variables[node.Variable] = value;
        _lastValue = value;
    }

    private void EvaluateBlockStatement(BoundBlockStatement node)
    {
        foreach (var statement in node.Statements)
            EvaluateStatement(statement);
    }
    private void EvaluateExpressionStatement(BoundExpressionStatement node)
    {
        _lastValue = EvaluateExpression(node.Expression);
    }
    private object EvaluateExpression(BoundExpression node)
    {
        return node switch
        {
            BoundLiteralExpression boundLiteralExpression => EvaluateLiteralExpression(boundLiteralExpression),
            BoundVariableExpression boundVariableExpression => EvaluateVariableExpression(boundVariableExpression),
            BoundAssignmentExpression boundAssignmentExpression => EvaluateAssignmentExpression(boundAssignmentExpression),
            BoundUnaryExpression boundUnaryExpression => EvaluateUnaryExpression(boundUnaryExpression),
            BoundBinaryExpression boundBinaryExpression => EvaluateBinaryExpression(boundBinaryExpression),
            BoundParenthesizedExpression boundParenthsizedExpression => EvaluateParenthsizedExpression(boundParenthsizedExpression),
            _ => throw new Exception($"Unexpected node '{node.Kind}'")
        };
    }

    private object EvaluateParenthsizedExpression(BoundParenthesizedExpression boundParenthsizedExpression)
    {
        return EvaluateExpression(boundParenthsizedExpression.Expression);
    }

    private object EvaluateBinaryExpression(BoundBinaryExpression boundBinaryExpression)
    {
        var left = EvaluateExpression(boundBinaryExpression.Left);
        var right = EvaluateExpression(boundBinaryExpression.Right);
        return boundBinaryExpression.Op.Kind switch
        {
            BoundBinaryOperatorKind.Addition => (int)left + (int)right,
            BoundBinaryOperatorKind.Subtraction => (int)left - (int)right,
            BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
            BoundBinaryOperatorKind.Division => (int)left / (int)right,
            BoundBinaryOperatorKind.LogicalAnd => (bool)left && (bool)right,
            BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
            BoundBinaryOperatorKind.Equals => Equals(left, right),
            BoundBinaryOperatorKind.NotEquals => Equals(left, right),
            BoundBinaryOperatorKind.Less => (int)left < (int)right,
            BoundBinaryOperatorKind.LessOrEquals => (int)left <= (int)right,
            BoundBinaryOperatorKind.Greater => (int)left > (int)right,
            BoundBinaryOperatorKind.GreaterOrEquals => (int)left >= (int)right,
            _ => throw new Exception($"Unexpected binary operator '{boundBinaryExpression.Op}'")
        };
    }

    private object EvaluateUnaryExpression(BoundUnaryExpression u)
    {
        var operand = EvaluateExpression(u.Operand);
        return u.Op.Kind switch
        {
            BoundUnaryOperatorKind.Identity => (int)operand,
            BoundUnaryOperatorKind.Negation => -(int)operand,
            BoundUnaryOperatorKind.LogicalNegation => !(bool)operand,
            _ => throw new Exception($"Unexpected unary operator '{u.Op}'")
        };
    }

    private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
    {
        var value = EvaluateExpression(a.Expression);
        _variables[a.Variable] = value;
        return value;
    }

    private object EvaluateVariableExpression(BoundVariableExpression v)
    {
        return _variables[v.Variable];
    }

    private static object EvaluateLiteralExpression(BoundLiteralExpression n)
    {
        return n.Value;
    }
}
