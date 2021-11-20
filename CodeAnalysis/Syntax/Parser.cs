using System.Collections.Generic;
using System.Collections.Immutable;

namespace Jvav.CodeAnalysis.Syntax
{
    public partial class Parser
    {
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private int _position;
        private readonly DiagnosticBag _diagnostics = new();
        private readonly SourceText _text;

        public Parser(SourceText text)
        {
            List<SyntaxToken> tokens = new();
            Lexer lexer = new(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                if (token.Kind != SyntaxKind.WhitespaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndToken);

            _text = text;
            _tokens = tokens.ToImmutableArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }
        private SyntaxToken Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _tokens.Length)
            {
                return _tokens[^1];
            }
            return _tokens[index];
        }
        private SyntaxToken Current => Peek(0);

        public DiagnosticBag Diagnostics => _diagnostics;

        private SyntaxToken NextToken()
        {
            SyntaxToken current = Current;
            _position++;
            return current;
        }
        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            Diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, null, null);
        }
        public CompilationUnitSyntax ParseCompilatioUnit()
        {
            StatementSyntax statement = ParseStatement();
            SyntaxToken endToken = MatchToken(SyntaxKind.EndToken);
            return new(statement, endToken);
        }
        private StatementSyntax ParseStatement()
        {
            if(Current.Kind == SyntaxKind.OpenBraceToken)
                return ParseBlockStatement();

            return ParseExpressionStatement();
        }
        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            ExpressionSyntax expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }
        private StatementSyntax ParseBlockStatement()
        {
            ImmutableArray<StatementSyntax>.Builder statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            SyntaxToken openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);
            while(Current.Kind != SyntaxKind.EndToken &&
                Current.Kind != SyntaxKind.CloseBraceToken)
            {
                StatementSyntax statement = ParseStatement();
                statements.Add(statement);
            }

            SyntaxToken closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }
        private ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }
        private ExpressionSyntax ParseAssignmentExpression()
        {
            if (Peek(0).Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }
        public ExpressionSyntax ParseTerm()
        {
            ExpressionSyntax left = ParseFactor();
            while (Current.Kind == SyntaxKind.PlusToken ||
                Current.Kind == SyntaxKind.MinusToken)
            {
                SyntaxToken operatorToken = NextToken();
                ExpressionSyntax right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }
        public ExpressionSyntax ParseFactor()
        {
            ExpressionSyntax left = ParsePrimaryExpression();
            while (Current.Kind == SyntaxKind.MultiplicationToken ||
                Current.Kind == SyntaxKind.SlashToken)
            {
                SyntaxToken operatorToken = NextToken();
                ExpressionSyntax right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }
        private ExpressionSyntax ParsePrimaryExpression()
        {
            return Current.Kind switch
            {
                SyntaxKind.OpenParenthesisToken => ParseParenthsizedExpression(),
                SyntaxKind.FalseKeyword or SyntaxKind.TrueKeyword => ParseBooleanLiteral(),
                SyntaxKind.LiteralToken => ParseNumberLiteral(),
                _ => ParseNameExpression(),
            };
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
            SyntaxToken numberToken = MatchToken(SyntaxKind.LiteralToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseParenthsizedExpression()
        {
            SyntaxToken left = NextToken();
            ExpressionSyntax expression = ParseExpression();
            SyntaxToken right = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        private ExpressionSyntax ParseBooleanLiteral()
        {
            var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            var keywordToken = MatchToken(isTrue ? SyntaxKind.TrueKeyword : SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(keywordToken, isTrue);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifierToken);
        }
    }
}
