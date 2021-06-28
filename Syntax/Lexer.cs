using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jvav.Syntax
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;
        private readonly List<string> _diagnostics = new();
        public Lexer(string text)
        {
            _text = text;
        }
        public IEnumerable<string> Diagnostics => _diagnostics;
        private char Current => Peek(0);
        private char Lookahead => Peek(1);
        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _text.Length)
                return '\0';

            return _text[index];
        }
        private int Next()
        {
            return _position++;
        }
        public SyntaxToken Lex()
        {
            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndToken, _position, "\0", null);
            }

            if (char.IsDigit(Current))
            {
                int start = _position;
                while (char.IsDigit(Current))
                    Next();
                int length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out int value))
                    _diagnostics.Add(Diagnostic.InvalidIntegralConstant(text));
                return new SyntaxToken(SyntaxKind.LiteralToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                int start = _position;
                while (char.IsWhiteSpace(Current))
                    Next();
                int length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }

            if (char.IsLetter(Current))
            {
                int start = _position;
                while (char.IsLetter(Current))
                    Next();
                int length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, Next(), "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, Next(), "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.MultiplicationToken, Next(), "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, Next(), "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, Next(), "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, Next(), ")", null);
                case '&':
                    if (Lookahead == '&')
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, _position += 2, "&&", null);
                    break;
                case '|':
                    if (Lookahead == '|')
                        return new SyntaxToken(SyntaxKind.PipePipeToken, _position += 2, "||", null);
                    break;
                case '=':
                    if (Lookahead == '=')
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, _position += 2, "==", null);
                    break;
                case '!':
                    if (Lookahead == '=')
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, _position += 2, "!=", null);
                    else
                        return new SyntaxToken(SyntaxKind.BangToken, Next(), "!", null);
            };
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}
