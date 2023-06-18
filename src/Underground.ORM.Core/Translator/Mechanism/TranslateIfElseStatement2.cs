﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Syntax;
using Underground.ORM.Core.Syntax.Token.Block;
using Underground.ORM.Core.Syntax.Token.Operator;

namespace Underground.ORM.Core.Translator
{
    public partial class SqlTranslator
    {
        private void TranslateIfElseStatement(string csFileContent,
                                              IEnumerable<SyntaxNodeOrToken> csharpTokens,
                                              SyntaxBase mysqlSyntaxOut)
        {
            foreach (var item in csharpTokens)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var ifStatementSyntax = node as IfStatementSyntax;
                var expressionSyntax = node as ExpressionSyntax;

                if (ifStatementSyntax != null)
                {
                    mysqlSyntaxOut.Append("IF", new OpenParenthesisToken());

                    var condition = ifStatementSyntax.Condition;
                    var conditionDescendants = condition.DescendantNodesAndTokensAndSelf().ToList();
                    var conditionTranslated = TranslateExpressionStatement(csFileContent, 
                                                                           conditionDescendants,
                                                                           mysqlSyntaxOut);

                    mysqlSyntaxOut.AppendRange(conditionTranslated);
                    mysqlSyntaxOut.AppendLine(
                        new CloseParenthesisToken(), 
                        new BeginBlockToken("THEN"));

                    var statementSyntax = ifStatementSyntax.Statement;

                    TranslateStatement(csFileContent,
                                        statementSyntax.DescendantTokens(),
                                        contentDeclaration,
                                        mysqlSyntaxOut);

                    var elseSyntax = ifStatementSyntax?.Else;

                    if (elseSyntax != null)
                    {
                        mysqlSyntaxOut.AppendLine("ELSE");

                        TranslateStatement(csFileContent,
                                            elseSyntax.DescendantTokens(),
                                            contentDeclaration,
                                            mysqlSyntaxOut);

                        mysqlSyntaxOut.AppendLine(new EndBlockToken(), "IF");

                    }
                }
                else if (expressionSyntax != null)
                {

                }
                else
                {
                    //if ((token.Parent as IfStatementSyntax) != null) continue;

                    //mysqlStatement.Add(token.ValueText);
                }
            }
        }
    }
}