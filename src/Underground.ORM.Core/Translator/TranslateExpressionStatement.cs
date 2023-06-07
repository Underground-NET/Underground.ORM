using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using Underground.ORM.Core.Translator.Expression;
using Underground.ORM.Core.Translator.Syntax;
using Underground.ORM.Core.Translator.Syntax.Declaration;
using Underground.ORM.Core.Translator.Syntax.Function;
using Underground.ORM.Core.Translator.Syntax.Operator;
using Underground.ORM.Core.Translator.Syntax.Reference;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private MySqlSyntax TranslateExpressionStatement(string csFileContent,
                                                         List<SyntaxNodeOrToken> descendants,
                                                         MySqlSyntax mysqlSyntaxOut)
        {
            MySqlSyntax mysqlExpression = new();

            int currentLevel = 0;
            List<ElevatorCastExpression> elevatorCast = new();

            foreach (var item in descendants)
            {
                var currentElevatorCast = elevatorCast!.FirstOrDefault(x => x.Level == currentLevel);
                string contentExpression = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                if (token.Parent != null)
                {
                    var identifierNameSyntax = token.Parent as IdentifierNameSyntax;
                    var qualifiedNameSyntax = token.Parent as QualifiedNameSyntax;
                    var literalExpressionSyntax = token.Parent as LiteralExpressionSyntax;
                    var castExpressionSyntax = token.Parent as CastExpressionSyntax;
                    var binaryExpressionSyntax = token.Parent as BinaryExpressionSyntax;
                    var parenthesizedExpressionSyntax = token.Parent as ParenthesizedExpressionSyntax;
                    var predefinedTypeSyntax = token.Parent as PredefinedTypeSyntax;

                    var kind = token.Parent.Kind();

                    if (qualifiedNameSyntax != null)
                    {
                        if (currentElevatorCast != null) continue;
                    }
                    else if (identifierNameSyntax != null)
                    {
                        if (currentElevatorCast != null) continue;

                        mysqlExpression.Append(new VariableReferenceToken($"`{token.ValueText}`"));
                    }
                    else if (literalExpressionSyntax != null)
                    {
                        if (kind == SyntaxKind.StringLiteralExpression)
                        {
                            mysqlExpression.Append(new StringToken(token.Text));
                        }
                        else
                            mysqlExpression.Append(token.Text);
                    }
                    else if (castExpressionSyntax != null)
                    {
                        if (token.ValueText == "(")
                        {
                            var predefinedType = castExpressionSyntax.Type as PredefinedTypeSyntax;
                            var identifierNameSyntaxCastExpression = castExpressionSyntax.Type as IdentifierNameSyntax;
                            var qualifiedNameSyntaxCastExpression = castExpressionSyntax.Type as QualifiedNameSyntax;

                            string variableTypeName;
                            if (qualifiedNameSyntaxCastExpression != null)
                            {
                                variableTypeName = csFileContent[qualifiedNameSyntaxCastExpression!.Span.Start..qualifiedNameSyntaxCastExpression.Span.End];
                            }
                            else if (identifierNameSyntaxCastExpression != null)
                            {
                                variableTypeName = csFileContent[identifierNameSyntaxCastExpression!.Span.Start..identifierNameSyntaxCastExpression.Span.End];
                            }
                            else
                                variableTypeName = predefinedType!.Keyword.ValueText;

                            var (Function, Alias) = BuildLeftCastFunctionFromToken(variableTypeName, contentExpression);

                            elevatorCast.Add(new ElevatorCastExpression()
                            {
                                Level = currentLevel,
                                CastExpression = castExpressionSyntax,
                                Function = Function,
                                Alias = Alias
                            });

                            mysqlExpression.Append(Function);
                            mysqlExpression.Append(new OpenParenthesisToken("("));
                        }
                    }
                    else if (binaryExpressionSyntax != null)
                    {
                        CloseParenthesesFromCastFunctions(currentLevel, elevatorCast, mysqlExpression);

                        string operatorToken = binaryExpressionSyntax.OperatorToken.ValueText;

                        if (operatorToken == "==")
                        {
                            mysqlExpression.Append(new AttributionToken("="));
                        }
                        else
                            mysqlExpression.Append(operatorToken);
                    }
                    else if (parenthesizedExpressionSyntax != null)
                    {
                        string parentheseToken = token.Text;

                        if (parentheseToken == "(")
                        {
                            currentLevel++;

                            mysqlExpression.Append(new OpenParenthesisToken("("));
                        }
                        else if (parentheseToken == ")")
                        {
                            CloseParenthesesFromCastFunctions(currentLevel, elevatorCast, mysqlExpression);

                            currentLevel--;

                            mysqlExpression.Append(new CloseParenthesisToken(")"));
                        }
                        else
                            throw new NotImplementedException(parentheseToken);
                    }
                    else
                    {
                        if (predefinedTypeSyntax?.Parent as CastExpressionSyntax != null)
                        {
                            continue;
                        }

                        // Tokens não tratados podem ser adicionados à expressão
                        mysqlExpression.Append(token.Text);
                    }
                }
            }

            #region Close parentheses with mysql cast conversion
            elevatorCast.ForEach(lastElevator =>
            {
                mysqlExpression.AppendRange(BuildRightCastFunction(lastElevator));
                mysqlExpression.Append(new CloseParenthesisToken(")"));
            });
            #endregion

            mysqlExpression.UpdateReferences(mysqlSyntaxOut);

            ReplacePlusOperatorStringToCommaConcat(mysqlExpression, mysqlSyntaxOut);
            InsertCoalesceToAllVariablesReference(mysqlExpression, mysqlSyntaxOut);

            return mysqlExpression;
        }

        private void ReplacePlusOperatorStringToCommaConcat(MySqlSyntax expression,
                                                            MySqlSyntax mysqlSyntaxOut)
        {
            for (int i = 0; i < expression.Count; i++)
            {
                MySqlSyntaxToken item = expression[i];

                if (item == "+")
                {
                    bool isStringLeft = item.Previous!.IsString ||
                        (item.Previous!.Reference is not null && item.Previous!.Reference.IsVar &&
                        ((VariableToken)item.Previous.Reference).DbType == System.Data.DbType.String);

                    bool isStringRight = item.Next!.IsString ||
                        (item.Next!.Reference is not null && item.Next!.Reference.IsVar &&
                        ((VariableToken)item.Next.Reference).DbType == System.Data.DbType.String);

                    if (isStringLeft || isStringRight)
                    {
                        expression.ReplaceAt(i, new CommaConcatToken(", "));
                    }
                }
            }

            var levels = expression.GetLevels();

            foreach (var level in levels)
            {
                var groups = expression.GetGroupsItemsFromLevel(level);

                foreach (var items in groups)
                {
                    var isCommaConcat = items.Exists(x => x is CommaConcatToken);

                    var first = items.First();
                    var last = items.Last();

                    if (isCommaConcat)
                    {
                        if (first is OpenParenthesisToken && 
                            last is CloseParenthesisToken)
                        {
                            var idxFirst = expression.IndexOf(first);
                            var idxLast = expression.IndexOf(last);

                            expression.AppendAt(idxFirst, new ConcatFunctionToken("CONCAT"));
                        }
                        else
                        {
                            expression.AppendAt(0, new ConcatFunctionToken("CONCAT"));
                            expression.AppendAt(1, new OpenParenthesisToken("("));
                            expression.Append(new CloseParenthesisToken(")"));
                        }
                    }
                }
            }
        }

        private void InsertCoalesceToAllVariablesReference(MySqlSyntax expression,
                                                           MySqlSyntax mysqlSyntaxOut)
        {
            for (int i = 0; i < expression.Count; i++)
            {
                MySqlSyntaxToken item = expression[i];
                
                if (item.IsString)
                {
                    expression.AppendAt(i, new CoalesceFunctionToken("COALESCE"));
                    expression.AppendAt(i + 1, new OpenParenthesisToken("("));
                    expression.AppendAt(i + 3, new CommaConcatToken(","));
                    expression.AppendAt(i + 4, new StringToken("''"));
                    expression.AppendAt(i += 5, new CloseParenthesisToken(")"));
                }
            }
        }

        private void CloseParenthesesFromCastFunctions(int currentLevel,
                                                       List<ElevatorCastExpression> elevatorCast,
                                                       MySqlSyntax mysqlExpression)
        {
            ElevatorCastExpression lastElevator;
            if (elevatorCast.Any() && currentLevel == (lastElevator = elevatorCast.Last()).Level)
            {
                mysqlExpression.AppendRange(BuildRightCastFunction(lastElevator));
                mysqlExpression.Append(new CloseParenthesisToken(")"));
                elevatorCast.Remove(lastElevator);
            }
        }

        private (MySqlSyntaxToken Function, MySqlSyntax? Alias)
            BuildLeftCastFunctionFromToken(string castType, string contentDeclaration)
        {
            if (castType == "ulong" ||
                castType == "UInt64" ||
                castType == "System.UInt64")
            {
                return (new CastFunctionToken("CAST"), new DbTypeToken("UNSIGNED", DbType.UInt64));
            }
            else if (
                castType == "long" ||
                castType == "Int64" ||
                castType == "System.Int64")
            {
                return (new CastFunctionToken("CAST"), new DbTypeToken("SIGNED", DbType.Int64));
            }
            else if (
                castType == "int" ||
                castType == "Int32" ||
                castType == "System.Int32")
            {
                return (
                    new CastFunctionToken("CAST"), new(
                        new DbTypeToken("SIGNED ", DbType.Int32), 
                        new DbTypeToken("INT", DbType.Int32))
                    );
            }
            else if (
                castType == "uint" ||
                castType == "UInt32" ||
                castType == "System.UInt32")
            {
                return (
                    new CastFunctionToken("CAST"), new (
                        new DbTypeToken("UNSIGNED ", DbType.UInt32), 
                        new DbTypeToken("INT", DbType.UInt32))
                    );
            }
            else if (
                castType == "short" ||
                castType == "Int16" ||
                castType == "System.Int16")
            {
                return (new CastFunctionToken("CAST"), new DbTypeToken("SIGNED", DbType.Int16));
            }
            else if (
                castType == "ushort" ||
                castType == "UInt16" ||
                castType == "System.UInt16")
            {
                return (new CastFunctionToken("CAST"), new DbTypeToken("UNSIGNED", DbType.UInt16));
            }
            else if (castType == "char")
            {
                return (new CastFunctionToken("CHAR"), null);
            }
            else
                throw new NotImplementedException($"Cast type '{castType}' of '{contentDeclaration}' not supported");
        }

        private MySqlSyntax BuildRightCastFunction(ElevatorCastExpression lastElevator)
        {
            MySqlSyntax castFunction = new CloseParenthesisToken(")");

            if (lastElevator.Alias != null)
            {
                castFunction = " AS ";
                castFunction.AppendRange(lastElevator.Alias);
            }

            return castFunction;
        }
    }
}
