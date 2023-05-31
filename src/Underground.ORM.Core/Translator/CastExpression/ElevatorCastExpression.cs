using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Urderground.ORM.Core.Translator.CastExpression
{
    public class ElevatorCastExpression
    {
        public string Function { get; set; }

        public string? Alias { get; set; }

        public int Level { get; set; }

        public CastExpressionSyntax Cast { get; set; }
    }
}