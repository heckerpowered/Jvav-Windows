using Jvav.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jvav
{
    public class Diagnostic
    {
        public static string UnexpectToken(SyntaxKind unexpectKind, SyntaxKind expectKind)
        {
            return $"J2000: Unexpected token '{unexpectKind}', expected '{expectKind}'";
        }
        public static string InvalidIntegralConstant(string text)
        {
            return $"J2001: Invalid integral constant '{text}'";
        }
    }
}
