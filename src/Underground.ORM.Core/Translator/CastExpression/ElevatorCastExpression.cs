using Microsoft.CodeAnalysis.CSharp.Syntax;
using Urderground.ORM.Core.Translator.List;

namespace Urderground.ORM.Core.Translator.CastExpression
{
    public class ElevatorCastExpression
    {
        public MySqlSyntaxItem Function { get; set; }

        public MySqlSyntaxList? Alias { get; set; }

        public int Level { get; set; }

        public CastExpressionSyntax Cast { get; set; }
    }
}