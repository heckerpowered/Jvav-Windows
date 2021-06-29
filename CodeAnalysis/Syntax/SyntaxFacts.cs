using System;

namespace Jvav.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind) => kind switch
        {
            SyntaxKind.PlusToken or SyntaxKind.MinusToken or SyntaxKind.BangToken => 6,
            _ => 0,
        };
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind) => kind switch
        {
            SyntaxKind.MultiplicationToken or SyntaxKind.SlashToken => 5,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 4,
            SyntaxKind.BangEqualsToken or SyntaxKind.EqualsEqualsToken => 3,
            SyntaxKind.AmpersandAmpersandToken => 2,
            SyntaxKind.PipePipeToken => 1,
            _ => 0,
        };

        public static SyntaxKind GetKeywordKind(string text)
        {
            return text switch
            {
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                _ => SyntaxKind.IdentifierToken,
            };
        }

        public static string GetText(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.PlusToken => "+",
                SyntaxKind.MinusToken => "-",
                SyntaxKind.MultiplicationToken => "*",
                SyntaxKind.SlashToken => "/",
                SyntaxKind.BangToken => "!",
                SyntaxKind.EqualsToken => "=",
                SyntaxKind.AmpersandAmpersandToken => "&&",
                SyntaxKind.PipePipeToken => "||",
                SyntaxKind.EqualsEqualsToken => "==",
                SyntaxKind.BangEqualsToken => "!=",
                SyntaxKind.OpenParenthesisToken => "(",
                SyntaxKind.CloseParenthesisToken => ")",
                SyntaxKind.FalseKeyword => "false",
                SyntaxKind.TrueKeyword => "true",
                _ => null
            };
        }
    }
}
