using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Translator.Syntax;
using Underground.ORM.Core.Translator.Syntax.Token.Block;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private void TranslateIfElseStatement(string csFileContent,
                                              List<SyntaxNodeOrToken> descendants,
                                              MySqlSyntax mysqlSyntaxOut)
        {
            foreach (var item in descendants)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var ifStatementSyntax = node as IfStatementSyntax;
                var expressionSyntax = node as ExpressionSyntax;

                if (ifStatementSyntax != null)
                {
                    mysqlSyntaxOut.Append("IF", new OpenParenthesisToken("("));

                    var condition = ifStatementSyntax.Condition;
                    var conditionDescendants = condition.DescendantNodesAndTokensAndSelf().ToList();
                    var conditionTranslated = TranslateExpressionStatement(csFileContent, 
                                                                           conditionDescendants,
                                                                           mysqlSyntaxOut);

                    mysqlSyntaxOut.AppendRange(conditionTranslated);
                    mysqlSyntaxOut.AppendLine(
                        new CloseParenthesisToken(")"), 
                        new BeginBlockToken("THEN"));

                    var statementSyntax = ifStatementSyntax.Statement;

                    TranslateStatements(csFileContent,
                                        statementSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                        mysqlSyntaxOut);

                    var elseSyntax = ifStatementSyntax?.Else;

                    if (elseSyntax != null)
                    {
                        mysqlSyntaxOut.AppendLine("ELSE");

                        TranslateStatements(csFileContent,
                                            elseSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                            mysqlSyntaxOut);

                        mysqlSyntaxOut.AppendLine(new EndBlockToken("END"), "IF");

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
