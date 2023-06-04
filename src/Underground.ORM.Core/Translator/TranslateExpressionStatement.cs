﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Translator.Expression;
using Underground.ORM.Core.Translator.Syntax;

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

                    if (qualifiedNameSyntax != null)
                    {
                        if (currentElevatorCast != null) continue;
                    }
                    else if (identifierNameSyntax != null)
                    {
                        if (currentElevatorCast != null) continue;

                        mysqlExpression.Append($"`{token.ValueText}`");
                    }
                    else if (literalExpressionSyntax != null)
                    {
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
                        CloseParenthesesFromCastFunctions(currentLevel, elevatorCast, mysqlExpression);

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
                            CloseParenthesesFromCastFunctions(currentLevel, elevatorCast, mysqlExpression);

                            currentLevel--;
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

            return mysqlExpression;
        }

        private void CloseParenthesesFromCastFunctions(int currentLevel,
                                                       List<ElevatorCastExpression> elevatorCast,
                                                       MySqlSyntax mysqlExpression)
        {
            ElevatorCastExpression lastElevator;
            if (elevatorCast.Any() && currentLevel == (lastElevator = elevatorCast.Last()).Level)
            {
                mysqlExpression.AppendRange(BuildRightCastFunction(lastElevator));
                mysqlExpression.Append(")");
                elevatorCast.Remove(lastElevator);
            }
        }

        private (MySqlSyntaxItem Function, MySqlSyntax? Alias)
            BuildLeftCastFunctionFromToken(string castType, string contentDeclaration)
        {
            if (castType == "ulong")
            {
                return ("CAST", new("UNSIGNED"));
            }
            else if (castType == "long")
            {
                return ("CAST", new("SIGNED"));
            }
            else if (
                castType == "int" ||
                castType == "Int32" ||
                castType == "System.Int32")
            {
                return ("CAST", new("SIGNED ", "INT"));
            }
            else if (
                castType == "uint" ||
                castType == "UInt32" ||
                castType == "System.UInt32")
            {
                return ("CAST", new("UNSIGNED ", "INT"));
            }
            else if (castType == "short")
            {
                return ("CAST", new("SIGNED"));
            }
            else if (castType == "ushort")
            {
                return ("CAST", new("UNSIGNED"));
            }
            else if (castType == "char")
            {
                return ("CHAR", null);
            }
            else
                throw new NotImplementedException($"Cast type '{castType}' of '{contentDeclaration}' not supported");
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
    }
}
