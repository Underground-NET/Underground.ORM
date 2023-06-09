﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Translator.Syntax;

namespace Underground.ORM.Core.Translator.Expression
{
    public class ElevatorCastExpression
    {
        public MySqlSyntaxToken Function { get; set; }

        public MySqlSyntax? Alias { get; set; }

        public int Level { get; set; }

        public CastExpressionSyntax CastExpression { get; set; }
    }
}