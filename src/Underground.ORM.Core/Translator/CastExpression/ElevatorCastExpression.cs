using Microsoft.CodeAnalysis.CSharp.Syntax;
using Urderground.ORM.Core.Translator.Syntax;

namespace Urderground.ORM.Core.Translator.CastExpression
{
    public class ElevatorCastExpression
    {
        public MySqlSyntaxItem Function { get; set; }

        public MySqlSyntax? Alias { get; set; }

        public int Level { get; set; }

        public CastExpressionSyntax Cast { get; set; }
    }
}