using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Urderground.ORM.Core.Translator.CastExpression;
using Urderground.ORM.Core.Translator.Syntax;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private MySqlSyntax TranslateExpressionStatement(string csFileContent,
                                                         List<SyntaxNodeOrToken> descendants,
                                                         MySqlSyntax mysqlSyntaxOut)
        {
            int closeParentheses = 0;
            MySqlSyntax mysqlExpression = new();

            int currentLevel = 0;
            List<ElevatorCastExpression> elevatorCast = new();

            foreach (var item in descendants)
            {
                string contentExpression = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                if (token.Parent != null)
                {
                    var identifierNameSyntax = token.Parent as IdentifierNameSyntax;
                    var literalExpressionSyntax = token.Parent as LiteralExpressionSyntax;
                    var castExpressionSyntax = token.Parent as CastExpressionSyntax;
                    var binaryExpressionSyntax = token.Parent as BinaryExpressionSyntax;
                    var parenthesizedExpressionSyntax = token.Parent as ParenthesizedExpressionSyntax;
                    var predefinedTypeSyntax = token.Parent as PredefinedTypeSyntax;

                    if (identifierNameSyntax != null)
                    {
                        mysqlExpression.Append($"`{token.ValueText}`");
                    }
                    else if (literalExpressionSyntax != null)
                    {
                        mysqlExpression.Append(token.Text);
                    }
                    else if (castExpressionSyntax != null)
                    {
                        if (token.ValueText == ")")
                        {
                            var predefinedType = castExpressionSyntax.Type as PredefinedTypeSyntax;

                            var (Function, Alias) = GetMysqlCastFunctionFromToken(predefinedType!.Keyword.ValueText, contentExpression);

                            elevatorCast.Add(new ElevatorCastExpression()
                            {
                                Level = currentLevel,
                                Cast = castExpressionSyntax,
                                Function = Function,
                                Alias = Alias
                            });

                            mysqlExpression.Append(Function);
                            mysqlExpression.Append("(");
                        }
                    }
                    else if (binaryExpressionSyntax != null)
                    {
                        #region Close parenthese with mysql cast conversion

                        ElevatorCastExpression lastElevator;
                        if (elevatorCast.Any() && currentLevel == (lastElevator = elevatorCast.Last()).Level)
                        {
                            mysqlExpression.AppendRange(BuildRightCastFunction(lastElevator));
                            mysqlExpression.Append(")");
                            elevatorCast.Remove(lastElevator);
                        }
                        #endregion

                        string operatorToken = binaryExpressionSyntax.OperatorToken.ValueText;

                        if (operatorToken == "==")
                        {
                            operatorToken = "=";
                        }

                        mysqlExpression.Append(operatorToken);
                    }
                    else if (parenthesizedExpressionSyntax != null)
                    {
                        string parentheseToken = token.Text;

                        if (parentheseToken == "(")
                        {
                            currentLevel++;
                        }
                        else if (parentheseToken == ")")
                        {
                            currentLevel--;

                            if (item == descendants.Last())
                            {
                                if (elevatorCast.Count > 0)
                                {
                                    closeParentheses++;
                                    continue;
                                }
                            }
                        }

                        mysqlExpression.Append(parentheseToken);
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
                mysqlExpression.Append(")");
            });
            #endregion

            while (closeParentheses-- > 0)
            {
                mysqlExpression.Append(")");
            }

            //if (elevatorCast.Count > 0)
            //{
            //    mysqlExpression.Append(")");
            //}

            return mysqlExpression;
        }

        private MySqlSyntax BuildRightCastFunction(ElevatorCastExpression lastElevator)
        {
            MySqlSyntax castFunction = ")";

            if (lastElevator.Alias != null)
            {
                castFunction = " AS ";
                castFunction.AppendRange(lastElevator.Alias);
            }

            return castFunction;
        }

        private (MySqlSyntaxItem Function, MySqlSyntax? Alias)
            GetMysqlCastFunctionFromToken(string castType, string contentDeclaration)
        {
            if (castType == "ulong")
            {
                return ("CAST", new("UNSIGNED ", "BIGINT"));
            }
            else if (castType == "long")
            {
                return ("CAST", new("SIGNED ", "BIGINT"));
            }
            else if (castType == "uint")
            {
                return ("CAST", new("UNSIGNED ", "INT"));
            }
            else if (castType == "int")
            {
                return ("CAST", new("SIGNED ", "INT"));
            }
            else if (castType == "char")
            {
                return ("CHAR", null);
            }
            else
                throw new NotImplementedException($"Tipo de conversão '{castType}' de '{contentDeclaration}' não suportada");
        }
    }
}
