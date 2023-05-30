using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MySqlOrm.Core.Translator
{
    public class ElevatorCast
    {
        public string Function { get; set; }

        public string? Alias { get; set; }

        public int Level { get; set; }

        public CastExpressionSyntax Cast { get; set; }
    }
}