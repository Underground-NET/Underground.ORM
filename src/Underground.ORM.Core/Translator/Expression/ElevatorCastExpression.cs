using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Syntax;

namespace Underground.ORM.Core.Translator.Expression
{
    public class ElevatorCastExpression
    {
        public SyntaxTokenBase Function { get; set; }

        public SyntaxBase? Alias { get; set; }

        public int Level { get; set; }

        public CastExpressionSyntax CastExpression { get; set; }
    }
}