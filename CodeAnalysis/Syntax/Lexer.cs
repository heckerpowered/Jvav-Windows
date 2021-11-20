using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jvav.CodeAnalysis.Text;

namespace Jvav.CodeAnalysis.Syntax;

public class Lexer
{
    private readonly SourceText _text;
    private readonly DiagnosticBag _diagnostics = new();

    private int _position;

    private int _start;
    private SyntaxKind _kind;
    private object _value;
    public Lexer(SourceText text)
    {
        _text = text;
    }
    public DiagnosticBag Diagnostics => _diagnostics;
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
        _start = _position;
        _kind = SyntaxKind.BadToken;
        _value = null;

        switch (Current)
        {
            case '\0':
                _kind = SyntaxKind.EndToken;
                break;
            case '+':
                _kind = SyntaxKind.PlusToken;
                _position++;
                break;
            case '-':
                _kind = SyntaxKind.MinusToken;
                _position++;
                break;
            case '*':
                _kind = SyntaxKind.MultiplicationToken;
                _position++;
                break;
            case '/':
                _kind = SyntaxKind.SlashToken;
                _position++;
                break;
            case '(':
                _kind = SyntaxKind.OpenParenthesisToken;
                _position++;
                break;
            case ')':
                _kind = SyntaxKind.CloseParenthesisToken;
                _position++;
                break;
            case '{':
                _kind = SyntaxKind.OpenBraceToken;
                _position++;
                break;
            case '}':
                _kind = SyntaxKind.CloseBraceToken;
                _position++;
                break;
            case '&':
                if (Lookahead == '&')
                {
                    _kind = SyntaxKind.AmpersandAmpersandToken;
                    _position += 2;
                    break;
                }
                break;
            case '|':
                if (Lookahead == '|')
                {
                    _kind = SyntaxKind.PipePipeToken;
                    _position += 2;
                    break;
                }
                break;
            case '=':
                if (Lookahead == '=')
                {
                    _kind = SyntaxKind.EqualsEqualsToken;
                    _position += 2;
                }
                else
                {
                    _kind = SyntaxKind.EqualsToken;
                    _position += 2;
                }
                break;
            case '!':
                _position++;
                if (Current == '=')
                {
                    _kind = SyntaxKind.BangEqualsToken;
                    _position++;
                }
                else
                {
                    _kind = SyntaxKind.BangToken;
                }
                break;
            case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                ReadNumberToken();
                break;
            case ' ' or '\t' or '\n' or '\n':
                ReadWhiteSpace();
                break;
            default:
                if (char.IsDigit(Current))
                {
                    ReadNumberToken();
                }
                else if (char.IsWhiteSpace(Current))
                {
                    ReadWhiteSpace();
                }
                else if (char.IsLetter(Current))
                {
                    ReadIdentifierOrKeywork();
                }
                else
                {
                    _diagnostics.ReportBadCharacter(_position, Current);
                    _position++;
                }
                break;
        }

        var length = _position - _start;
        var text = SyntaxFacts.GetText(_kind);
        if (text == null)
            text = _text.ToString(_start, length);

        return new(_kind, _start, text, _value);
    }

    private void ReadIdentifierOrKeywork()
    {
        while (char.IsLetter(Current))
            Next();

        int length = _position - _start;
        var text = _text.ToString(_start, length);
        _kind = SyntaxFacts.GetKeywordKind(text);
    }

    private void ReadWhiteSpace()
    {
        while (char.IsWhiteSpace(Current))
            Next();

        _kind = SyntaxKind.WhitespaceToken;
    }

    private void ReadNumberToken()
    {
        while (char.IsDigit(Current))
            Next();

        int length = _position - _start;
        var text = _text.ToString(_start, length);
        if (!int.TryParse(text, out var value))
            _diagnostics.ReportInvalidNumber(new(_start, length), text, typeof(int));

        _value = value;
        _kind = SyntaxKind.LiteralToken;
    }
}
