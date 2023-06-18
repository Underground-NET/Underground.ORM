using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Translator.Syntax;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;
using Underground.ORM.Core.Translator.Syntax.Token.Statement;

namespace Urderground.ORM.Core.Translator
{
    public partial class ___MySqlTranslator
    {
        private MySqlSyntax TranslateReturnStatement(string csFileContent,
                                                     List<SyntaxNodeOrToken> descendants,
                                                     MySqlSyntax mysqlSyntaxOut)
        {
            MySqlSyntax mysqlStatement = new();

            foreach (var item in descendants)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var returnStatementSyntax = node as ReturnStatementSyntax;
                var expressionSyntax = node as ExpressionSyntax;

                if (returnStatementSyntax != null)
                {
                    mysqlStatement.Append(new ReturnStatementToken("RETURN "));
                }
                else if (expressionSyntax != null)
                {
                    var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();

                    var expTranslated = TranslateExpressionStatement(csFileContent, 
                                                                     descendantExpression,
                                                                     mysqlSyntaxOut);

                    mysqlStatement.AppendRange(expTranslated);

                    break;
                }
            }

            mysqlSyntaxOut.AppendRange(mysqlStatement);
            mysqlSyntaxOut.AppendLine(new SemicolonToken(";"));

            return mysqlStatement;
        }
    }
}
